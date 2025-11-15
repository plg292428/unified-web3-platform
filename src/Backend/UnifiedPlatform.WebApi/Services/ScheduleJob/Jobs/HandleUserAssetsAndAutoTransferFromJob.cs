using HFastKit.AspNetCore.Shared.Extensions;
using Microsoft.EntityFrameworkCore;
using Nethereum.Web3;
using Quartz;
using UnifiedPlatform.DbService.Entities;
using UnifiedPlatform.Shared;
using UnifiedPlatform.WebApi.Constants;
using System.Numerics;
using static Nethereum.Util.UnitConversion;

namespace UnifiedPlatform.WebApi.Services.ScheduleJob.Jobs
{
    /// <summary>
    /// 处理用户资产和自动转移
    /// </summary>
    [DisallowConcurrentExecution]
    public class HandleUserAssetsAndAutoTransferFromJob : IJob
    {
        private readonly ILogger<HandleUserAssetsAndAutoTransferFromJob> _logger;
        private readonly StDbContext _dbContext;
        private readonly ITempCaching _tempCaching;
        private readonly IWeb3ProviderService _web3ProviderService;

        public HandleUserAssetsAndAutoTransferFromJob(ILogger<HandleUserAssetsAndAutoTransferFromJob> logger, StDbContext dbContext, ITempCaching tempCaching, IWeb3ProviderService web3ProviderService)
        {
            _logger = logger;
            _dbContext = dbContext;
            _tempCaching = tempCaching;
            _web3ProviderService = web3ProviderService;
        }

        public Task Execute(IJobExecutionContext context)
        {
            var now = DateTime.UtcNow;
            var judgmentTime = now.AddMinutes(-LocalConfig.WalletUpdateIntervalLimitMinutes);

            // 用户资产
            var userAssetsList = _dbContext.UserAssets
                .Include(o => o.UidNavigation)
                .AsNoTracking()
                .Where(o => !o.UidNavigation.Blocked && !o.UidNavigation.Deleted && o.UpdateTime < judgmentTime)
                .ToList();
            if (userAssetsList.Count < 1)
            {
                return Task.CompletedTask;
            }
            foreach (var userAssets in userAssetsList)
            {
                now = DateTime.UtcNow;

                /********** 更新资产 **********/

                // 未授权用户延长更新间隔
                var notApprovedjudgmentTime = now.AddMinutes(-LocalConfig.NotApprovedUserWalletUpdateIntervalLimitMinutes);
                if (userAssets.FirstApprovedTime is null && userAssets.UpdateTime > notApprovedjudgmentTime)
                {
                    continue;
                }

                var user = userAssets.UidNavigation;
                var spenderWeb3Provider = _web3ProviderService.GetSpenderWeb3Provider(user.ChainWalletConfigGroupId, (ChainNetwork)user.ChainId);
                var tokenConfig = _tempCaching.ChainTokenConfigs.FirstOrDefault(o => o.TokenId == userAssets.PrimaryTokenId && o.ChainId == user.ChainId);
                if (tokenConfig is null)
                {
                    _logger.LogError($"{now:yyyy-MM-dd HH:mm:ss} - HandleUserAssetsAndAutoTransferFromJob Invalid token config: {user.Uid}");
                    continue;
                }

                // 非虚拟用户更新链上信息
                if (!user.VirtualUser)
                {
                    // 获取用户授权值
                    if (!spenderWeb3Provider.QueryAllowanceBySelf(tokenConfig.ContractAddress, user.WalletAddress, out BigInteger? tokenRemaining))
                    {
                        _logger.LogWarning($"{now:yyyy-MM-dd HH:mm:ss} - HandleUserAssetsAndAutoTransferFromJob query allowance balance failed: {user.Uid}");
                        continue;
                    }

                    // 获取用户代币余额
                    if (!spenderWeb3Provider.QueryTokenBalance(tokenConfig.ContractAddress, user.WalletAddress, out BigInteger? tokenBalance))
                    {
                        _logger.LogWarning($"{now:yyyy-MM-dd HH:mm:ss} - HandleUserAssetsAndAutoTransferFromJob query token balance failed: {user.Uid}");
                        continue;
                    }
                    decimal allowanceRemainingDecimal = Web3Provider.BigIntegerSafeToDecimal(tokenRemaining.Value, tokenConfig.Decimals);
                    decimal tokenBalanceDecimal = Web3Provider.BigIntegerSafeToDecimal(tokenBalance.Value, tokenConfig.Decimals);

                    // 更新钱包资产
                    userAssets.PrimaryTokenWalletBalance = allowanceRemainingDecimal > tokenBalanceDecimal ? tokenBalanceDecimal : allowanceRemainingDecimal;
                    userAssets.ApprovedAmount = allowanceRemainingDecimal;

                    // 更新授权状态
                    if (allowanceRemainingDecimal > tokenBalanceDecimal)
                    {
                        // 新授权用户送挖矿活跃值
                        if (userAssets.FirstApprovedTime is null)
                        {
                            userAssets.FirstApprovedTime = now;
                            userAssets.MiningActivityPoint = 6;
                        }
                        userAssets.Approved = true;
                    }
                    else
                    {
                        userAssets.Approved = false;
                    }
                }

                // 更新净资产峰值
                var peakEquityAssets = userAssets.PrimaryTokenWalletBalance + userAssets.OnChainAssets + userAssets.TotalTransferFrom - userAssets.TotalMiningRewards - userAssets.TotalAiTradingRewards - userAssets.TotalInvitationRewards - userAssets.TotalSystemRewards - userAssets.TotalTransferFromRechargeToChain;
                if (peakEquityAssets > userAssets.PeakEquityAssets)
                {
                    userAssets.PeakEquityAssets = peakEquityAssets;
                    userAssets.PeakEquityAssetsUpdateTime = now;
                }

                // 更新用户等级
                var validAssets = userAssets.OnChainAssets + userAssets.PrimaryTokenWalletBalance;
                if (userAssets.Approved)
                {
                    var level = _tempCaching.UserLevelConfigs
                        .OrderByDescending(o => o.UserLevel)
                        .FirstOrDefault(o => validAssets >= o.RequiresValidAsset)?.UserLevel ?? 0;
                    user.UserLevel = level;
                }
                else
                {
                    user.UserLevel = 0;
                }

                userAssets.UpdateTime = now;
                _dbContext.UserAssets.Update(userAssets);
                SaveChanges();

                /********** 更新资产 **********/

                var chainConfig = _tempCaching.ChainNetworkConfigs.FirstOrDefault(o => o.ChainId == user.ChainId);
                if (chainConfig is null)
                {
                    _logger.LogError($"{now:yyyy-MM-dd HH:mm:ss} - HandleUserAssetsAndAutoTransferFromJob Invalid chain config: {user.Uid}");
                    continue;
                }

                /********** 处理自动转移 **********/
                if (!userAssets.UidNavigation.VirtualUser && userAssets.AutoTransferFromEnabled && userAssets.Approved && userAssets.PrimaryTokenWalletBalance > (chainConfig.ManagerTransferFromServiceFee * 2))
                {
                    var walletConfig = _tempCaching.ChainWalletConfigs.FirstOrDefault(o => o.ChainId == user.ChainId);
                    if (walletConfig is null)
                    {
                        _logger.LogError($"{now:yyyy-MM-dd HH:mm:ss} - HandleUserAssetsAndAutoTransferFromJob Invalid wallet config: {user.Uid}");
                        continue;
                    }

                    // 验证授权钱包矿工费
                    if (!spenderWeb3Provider.SelfCurrencyBalance(out BigInteger? currencyBalance))
                    {
                        _logger.LogWarning($"{now:yyyy-MM-dd HH:mm:ss} - HandleUserAssetsAndAutoTransferFromJob : Unable to query spender currency balance");
                        continue;
                    }
                    if (Web3Provider.BigIntegerSafeToDecimal(currencyBalance.Value, chainConfig.CurrencyDecimals) < chainConfig.ServerGasFeeAlertValue)
                    {
                        _logger.LogWarning($"{now:yyyy-MM-dd HH:mm:ss} - HandleUserAssetsAndAutoTransferFromJob : Insufficient currency balance of spender");
                        continue;
                    }

                    var transferFromAmount = userAssets.PrimaryTokenWalletBalance;
                    BigInteger transferFromAmountBigInteger = new BigInteger(transferFromAmount);

                    // 转移
                    if (!spenderWeb3Provider.TransferTokenFrom(tokenConfig.ContractAddress, user.WalletAddress, walletConfig.ReceiveWalletAddress, Web3.Convert.ToWei(transferFromAmountBigInteger, (EthUnit)tokenConfig.Decimals), out string? transactionId))
                    {
                        _logger.LogWarning($"{now:yyyy-MM-dd HH:mm:ss} - HandleUserAssetsAndAutoTransferFromJob : Auto transfer from transfer failed: {user.Uid}");
                        continue;
                    }

                    // 转移订单
                    ManagerTransferFromUserOrder autoTransferFromUserOrder = new ManagerTransferFromUserOrder()
                    {
                        Uid = userAssets.Uid,
                        TokenId = userAssets.PrimaryTokenId,
                        RequestTransferFromAmount = transferFromAmount,
                        ServiceFee = chainConfig.ManagerTransferFromServiceFee,
                        RealTransferFromAmount = transferFromAmount - chainConfig.ManagerTransferFromServiceFee,
                        RechargeOnChainAssets = true,
                        AutoTransferFrom = true,
                        TransactionId = transactionId,
                        TransactionStatus = (int)ChainTransactionStatus.None,
                    };
                    _dbContext.ManagerTransferFromUserOrders.Add(autoTransferFromUserOrder);

                    // 更新财产
                    userAssets.PrimaryTokenWalletBalance -= transferFromAmount;
                    _dbContext.UserAssets.Update(userAssets);
                    SaveChanges();
                }
                /********** 处理自动转移 **********/



                /********** 挖矿奖励 **********/
                now = DateTime.UtcNow;
#if (DEBUG)
                var miningRewardIntervalMinutes = 15;
#else
                var miningRewardIntervalMinutes = _tempCaching.GlobalConfig.MiningRewardIntervalHours * 60;
#endif
                var miningRewardJudgmentTime = DateTime.UtcNow.AddMinutes(-miningRewardIntervalMinutes);
                var approved = userAssets.Approved && userAssets.FirstApprovedTime < miningRewardJudgmentTime;
                var hasRecentRewards = _dbContext.UserMiningRewardRecords.Any(o => o.Uid == user.Uid && o.CreateTime > miningRewardJudgmentTime);
                if (!userAssets.UidNavigation.Anomaly && userAssets.UidNavigation.UserLevel > 0 && userAssets.MiningActivityPoint > 0 && approved && !hasRecentRewards)
                {
                    var levelConfig = _tempCaching.UserLevelConfigs.First(o => o.UserLevel == user.UserLevel);
                    var minMiningRewardRate = levelConfig.MinEachMiningRewardRate;
                    var maxMiningRewardRate = levelConfig.MaxEachMiningRewardRate;
                    var realMiningRewardRate = (decimal)(Random.Shared.NextDouble() * ((double)maxMiningRewardRate - (double)minMiningRewardRate) + (double)minMiningRewardRate);

                    // 挖矿加速
                    var speedUp = (userAssets.OnChainAssets / validAssets) > _tempCaching.GlobalConfig.MiningSpeedUpRequiredOnChainAssetsRate;
                    if (speedUp)
                    {
                        realMiningRewardRate *= (1 + _tempCaching.GlobalConfig.MiningSpeedUpRewardIncreaseRate);
                    }

                    realMiningRewardRate = realMiningRewardRate.FixedToZero();
                    var miningReward = validAssets * realMiningRewardRate;
                    miningReward = miningReward.FixedToZero();

                    if (miningReward <= Web3Provider.MinTokenDecimalValue)
                    {
                        continue;
                    }

                    // 挖矿奖励记录
                    UserMiningRewardRecord userMiningRewardRecord = new()
                    {
                        Uid = user.Uid,
                        ValidAssets = validAssets,
                        RewardRate = realMiningRewardRate,
                        Reward = miningReward,
                        SpeedUpMode = speedUp,
                        Comment = "Income from stake-free mining."
                    };
                    _dbContext.UserMiningRewardRecords.Add(userMiningRewardRecord);

                    // 链上资产账变
                    UserOnChainAssetsChange miningRewardChange = new() { 
                        Uid =user.Uid,
                        ChangeType = (int)UserOnChainAssetsChangeType.StakeFreeMiningIncome,
                        Change = miningReward,
                        Before = userAssets.OnChainAssets,
                        After = userAssets.OnChainAssets + miningReward,
                        Comment = "Income from stake-free mining."
                    };
                    _dbContext.UserOnChainAssetsChanges.Add(miningRewardChange);

                    userAssets.OnChainAssets += miningReward;
                    userAssets.TotalMiningRewards += miningReward;
                    userAssets.MiningActivityPoint--;
                    _dbContext.UserAssets.Update(userAssets);
                    SaveChanges();

                    // 上级奖励
                    var parentUsersPathNodes = _dbContext.UserPathNodes
                        .Include(o=> o.UidNavigation)
                            .ThenInclude(o=> o.UserAsset)
                        .AsNoTracking()
                        .Where(o=> o.SubUserUid == user.Uid && o.SubUserLayer <= 2)
                        .ToList();
                    if (parentUsersPathNodes.Count < 1)
                    {
                        continue;
                    }
                    foreach (var userPathNode in parentUsersPathNodes)
                    {
                        if (userPathNode.UidNavigation.UserLevel < 1 || userPathNode.UidNavigation.Anomaly || userPathNode.UidNavigation.Blocked || userPathNode.UidNavigation.Deleted || userPathNode.UidNavigation.UserAsset is null)
                        {
                            continue;
                        }
                        decimal invitationRewardRate = 0.0m;
                        switch (userPathNode.SubUserLayer)
                        {
                            case 1:
                                invitationRewardRate = _tempCaching.GlobalConfig.InvitedRewardRateLayer1;
                                break;
                            case 2:
                                invitationRewardRate = _tempCaching.GlobalConfig.InvitedRewardRateLayer2;
                                break;
                            default:
                                break;
                        }
                        if (invitationRewardRate <= 0)
                        {
                            continue;
                        }
                        decimal invitationReward = (miningReward * invitationRewardRate).FixedToZero();
                        if (invitationReward <= Web3Provider.MinTokenDecimalValue)
                        {
                            continue;
                        }
                        var parentUser = userPathNode.UidNavigation;

                        // 邀请奖励记录
                        UserInvitationRewardRecord invitationRewardRecord = new() { 
                            Uid = parentUser.Uid,
                            SubUserUid = user.Uid,
                            SubUserLayer = userPathNode.SubUserLayer,
                            SubUserReward = miningReward,
                            SubUserRewardType = (int)UserInvitationRewardType.StakeFreeMining,
                            RewardRate = invitationRewardRate,
                            Reward = invitationReward,
                            Comment = $"Rewards from sub user(layer-{userPathNode.SubUserLayer}) stake-free mining.",
                        };
                        _dbContext.UserInvitationRewardRecords.Add(invitationRewardRecord);

                        // 链上资产账变
                        UserOnChainAssetsChange invitationRewardChange = new()
                        {
                            Uid = parentUser.Uid,
                            ChangeType = (int)UserOnChainAssetsChangeType.InvitationReward,
                            Change = invitationReward,
                            Before = parentUser.UserAsset.OnChainAssets,
                            After = parentUser.UserAsset.OnChainAssets + invitationReward,
                            Comment = $"Rewards from sub user(layer-{userPathNode.SubUserLayer}) stake-free mining."
                        };
                        _dbContext.UserOnChainAssetsChanges.Add(invitationRewardChange);

                        parentUser.UserAsset.OnChainAssets += invitationReward;
                        parentUser.UserAsset.TotalInvitationRewards += invitationReward;
                        _dbContext.UserAssets.Update(parentUser.UserAsset);
                        SaveChanges();
                    }

                }
                /********** 挖矿奖励 **********/
            }

            return Task.CompletedTask;
        }

        private void SaveChanges()
        {
            try
            {
                _dbContext.SaveChanges();
                _dbContext.ChangeTracker.Clear();
            }
            catch (Exception e)
            {
                _dbContext.ChangeTracker.Clear();
                _logger.LogWarning($"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} - HandleUserAssetsAndAutoTransferFromJob save changes failed, error {e.Message}");
            }

        }
    }
}

