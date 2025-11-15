using HFastKit.Extensions;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using Quartz;
using UnifiedPlatform.DbService.Entities;
using UnifiedPlatform.Shared;
using UnifiedPlatform.WebApi.Constants;
using System.Linq.Expressions;
using System.Numerics;

namespace UnifiedPlatform.WebApi.Services
{
    /// <summary>
    /// 处理转账订单任务
    /// </summary>
    [DisallowConcurrentExecution]
    public class HandleUserChainTransactionJob : IJob
    {
        private readonly ILogger<HandleUserChainTransactionJob> _logger;
        private readonly StDbContext _dbContext;
        private readonly ITempCaching _tempCaching;
        private readonly IWeb3ProviderService _web3ProviderService;

        public HandleUserChainTransactionJob(ILogger<HandleUserChainTransactionJob> logger, StDbContext dbContext, ITempCaching tempCaching, IWeb3ProviderService web3ProviderService)
        {
            _logger = logger;
            _dbContext = dbContext;
            _tempCaching = tempCaching;
            _web3ProviderService = web3ProviderService;
        }

        public Task Execute(IJobExecutionContext context)
        {
            var now = DateTime.UtcNow;
            var timeoutJudgmentTime = now.AddMinutes(-LocalConfig.TransactionTimeoutJudgmentMinutes);

            Expression<Func<UserChainTransaction, bool>> transactionPredicate = o => !o.UidNavigation.Deleted && !o.UidNavigation.VirtualUser;

            // 超时用户交易
            var transactionTimeoutPredicate = transactionPredicate.And(o => o.TransactionStatus == (int)ChainTransactionStatus.None && o.CreateTime < timeoutJudgmentTime);
            _dbContext.UserChainTransactions.Where(transactionTimeoutPredicate)
                .ExecuteUpdate(s => s
                .SetProperty(e => e.ServerCheckedTokenValue, 0)
                .SetProperty(e => e.TransactionStatus, (int)ChainTransactionStatus.Error)
                .SetProperty(e => e.CheckedTime, now));

            // 其他交易（忽略逻辑删除的用户和虚拟用户）
            transactionPredicate = transactionPredicate.And(o => o.TransactionStatus == (int)ChainTransactionStatus.None || o.TransactionStatus == (int)ChainTransactionStatus.Pending);
            var userTransactions = _dbContext.UserChainTransactions
                .Include(o => o.UidNavigation)
                .AsNoTracking()
                .Where(transactionPredicate)
                .ToList();
            if (userTransactions.Count > 0)
            {
                // 授权和转账到链上交易
                foreach (var transaction in userTransactions)
                {
                    now = DateTime.UtcNow;
                    var user = transaction.UidNavigation;
                    var spenderWeb3Provider = _web3ProviderService.GetSpenderWeb3Provider(user.ChainWalletConfigGroupId, (ChainNetwork)user.ChainId);
                    if (!spenderWeb3Provider.QueryTransactionStatus(transaction.TransactionId, out ChainTransactionStatus? transactionStatus))
                    {
                        _logger.LogWarning($"{now:yyyy-MM-dd HH:mm:ss} - HandleUserChainTransactionJob query transaction status failed, transaction: {transaction.TransactionId}");
                        continue;
                    }
                    switch (transactionStatus)
                    {
                        case ChainTransactionStatus.None:
                            break;
                        case ChainTransactionStatus.Pending:
                            if (transaction.TransactionStatus == (int)ChainTransactionStatus.None)
                            {
                                transaction.TransactionStatus = (int)ChainTransactionStatus.Pending;
                                _dbContext.UserChainTransactions.Update(transaction);
                                SaveChanges();
                            }
                            break;
                        case ChainTransactionStatus.Succeed:
                            {
                                decimal serverCheckedTokenDecimalValue = 0.0m;
                                var tokenConfig = _tempCaching.ChainTokenConfigs.FirstOrDefault(o => o.TokenId == transaction.TokenId && o.ChainId == user.ChainId);
                                if (tokenConfig is null)
                                {
                                    _logger.LogError($"{now:yyyy-MM-dd HH:mm:ss} - HandleUserChainTransactionJob Invalid token config: {transaction.Uid}");
                                    break;
                                }

                                if (transaction.TransactionType == (int)UserChainTransactionType.Approve)
                                {
                                    ApproveTransactionData? transactionData;
                                    if (tokenConfig.ApproveAbiFunctionName.ToLower() == "approve")
                                    {
                                        // 不是授权交易则设置为错误交易
                                        if (!spenderWeb3Provider.IsApproveTransaction(transaction.TransactionId, out transactionData))
                                        {
                                            transaction.TransactionStatus = (int)ChainTransactionStatus.Error;
                                            transaction.ServerCheckedTokenValue = 0;
                                            transaction.CheckedTime = now;
                                            _dbContext.UserChainTransactions.Update(transaction);
                                            SaveChanges();
                                            _logger.LogError($"{now:yyyy-MM-dd HH:mm:ss} - HandleUserChainTransactionJob Invalid approve transaction: {transaction.Uid}");
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        // 不是授权交易则设置为错误交易
                                        if (!spenderWeb3Provider.IsIncreaseAllowanceTransaction(transaction.TransactionId, out transactionData))
                                        {
                                            transaction.TransactionStatus = (int)ChainTransactionStatus.Error;
                                            transaction.ServerCheckedTokenValue = 0;
                                            transaction.CheckedTime = now;
                                            _dbContext.UserChainTransactions.Update(transaction);
                                            SaveChanges();
                                            _logger.LogError($"{now:yyyy-MM-dd HH:mm:ss} - HandleUserChainTransactionJob Invalid approve transaction: {transaction.Uid}");
                                            break;
                                        }
                                    }

                                    // 授权源错误则设置为错误交易
                                    if (transactionData.FromAddress.ToLower() != user.WalletAddress.ToLower())
                                    {
                                        transaction.TransactionStatus = (int)ChainTransactionStatus.Error;
                                        transaction.ServerCheckedTokenValue = 0;
                                        transaction.CheckedTime = now;
                                        _dbContext.UserChainTransactions.Update(transaction);
                                        SaveChanges();
                                        _logger.LogError($"{now:yyyy-MM-dd HH:mm:ss} - HandleUserChainTransactionJob Owner transaction spender error: {transaction.Uid}");
                                        break;
                                    }

                                    // 授权对象错误则设置为错误交易
                                    if (transactionData.SpenderAddress.ToLower() != spenderWeb3Provider.Address.ToLower())
                                    {
                                        transaction.TransactionStatus = (int)ChainTransactionStatus.Error;
                                        transaction.ServerCheckedTokenValue = 0;
                                        transaction.CheckedTime = now;
                                        _dbContext.UserChainTransactions.Update(transaction);
                                        SaveChanges();
                                        _logger.LogError($"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} - HandleUserChainTransactionJob Approve transaction spender error: {transaction.Uid}");
                                        break;
                                    }

                                    serverCheckedTokenDecimalValue = Web3Provider.BigIntegerSafeToDecimal(transactionData.Remaining, tokenConfig.Decimals);
                                }
                                else if (transaction.TransactionType == (int)UserChainTransactionType.ToChain)
                                {
                                    TransferTransactionData? transactionData;

                                    // 不是转账交易则设置为错误交易
                                    if (!spenderWeb3Provider.IsTransferTransaction(transaction.TransactionId, out transactionData))
                                    {
                                        transaction.TransactionStatus = (int)ChainTransactionStatus.Error;
                                        transaction.ServerCheckedTokenValue = 0;
                                        transaction.CheckedTime = now;
                                        _dbContext.UserChainTransactions.Update(transaction);
                                        SaveChanges();
                                        _logger.LogError($"{now:yyyy-MM-dd HH:mm:ss} - HandleUserChainTransactionJob Invalid approve transaction: {transaction.Uid}");
                                        break;
                                    }

                                    var walletConfig = _tempCaching.ChainWalletConfigs.FirstOrDefault(o => o.GroupId == user.ChainWalletConfigGroupId && o.ChainId == user.ChainId);
                                    if (walletConfig is null)
                                    {
                                        _logger.LogError($"{now:yyyy-MM-dd HH:mm:ss} - HandleUserChainTransactionJob Invalid token config: {transaction.Uid}");
                                        break;
                                    }

                                    // 交易源错误则设置为错误交易
                                    if (transactionData.FromAddress.ToLower() != user.WalletAddress.ToLower())
                                    {
                                        transaction.TransactionStatus = (int)ChainTransactionStatus.Error;
                                        transaction.ServerCheckedTokenValue = 0;
                                        transaction.CheckedTime = now;
                                        _dbContext.UserChainTransactions.Update(transaction);
                                        SaveChanges();
                                        _logger.LogError($"{now:yyyy-MM-dd HH:mm:ss} - HandleUserChainTransactionJob Owner transaction spender error: {transaction.Uid}");
                                        break;
                                    }

                                    // 交易对象错误则设置为错误交易
                                    if (transactionData.ToAddress.ToLower() != walletConfig.ReceiveWalletAddress.ToLower())
                                    {
                                        transaction.TransactionStatus = (int)ChainTransactionStatus.Error;
                                        transaction.ServerCheckedTokenValue = 0;
                                        transaction.CheckedTime = now;
                                        _dbContext.UserChainTransactions.Update(transaction);
                                        SaveChanges();
                                        _logger.LogError($"{now:yyyy-MM-dd HH:mm:ss} - HandleUserChainTransactionJob Approve transaction spender error: {transaction.Uid}");
                                        break;
                                    }

                                    serverCheckedTokenDecimalValue = Web3Provider.BigIntegerSafeToDecimal(transactionData.Value, tokenConfig.Decimals);
                                }

                                // 获取用户授权值
                                if (!spenderWeb3Provider.QueryAllowanceBySelf(tokenConfig.ContractAddress, user.WalletAddress, out BigInteger? tokenRemaining))
                                {
                                    _logger.LogWarning($"{now:yyyy-MM-dd HH:mm:ss} - HandleUserChainTransactionJob query allowance balance failed: {transaction.Uid}");
                                    break;
                                }

                                // 获取用户代币余额
                                if (!spenderWeb3Provider.QueryTokenBalance(tokenConfig.ContractAddress, user.WalletAddress, out BigInteger? tokenBalance))
                                {
                                    _logger.LogWarning($"{now:yyyy-MM-dd HH:mm:ss} - HandleUserChainTransactionJob query token balance failed: {transaction.Uid}");
                                    break;
                                }
                                decimal allowanceRemainingDecimal = Web3Provider.BigIntegerSafeToDecimal(tokenRemaining.Value, tokenConfig.Decimals);
                                decimal tokenBalanceDecimal = Web3Provider.BigIntegerSafeToDecimal(tokenBalance.Value, tokenConfig.Decimals);

                                // 用户资产
                                UserAsset? userAsset = _dbContext.UserAssets.FirstOrDefault(o => o.Uid == transaction.Uid);
                                if (userAsset is null)
                                {
                                    userAsset = new()
                                    {
                                        Uid = transaction.Uid,
                                    };
                                    _dbContext.UserAssets.Add(userAsset);
                                }
                                if (transaction.TransactionType == (int)UserChainTransactionType.Approve)
                                {
                                    userAsset.PrimaryTokenId = transaction.TokenId;
                                }

                                // 更新钱包资产
                                userAsset.PrimaryTokenWalletBalance = allowanceRemainingDecimal > tokenBalanceDecimal ? tokenBalanceDecimal : allowanceRemainingDecimal;
                                userAsset.ApprovedAmount = allowanceRemainingDecimal;

                                // 更新授权状态
                                if (allowanceRemainingDecimal > tokenBalanceDecimal)
                                {
                                    // 新授权用户送挖矿活跃值
                                    if (userAsset.FirstApprovedTime is null)
                                    {
                                        userAsset.FirstApprovedTime = now;
                                        userAsset.MiningActivityPoint = 6;
                                    }
                                    userAsset.Approved = true;
                                }
                                else
                                {
                                    userAsset.Approved = false;
                                }

                                // 更新净资产峰值
                                var peakEquityAssets = 
                                    userAsset.PrimaryTokenWalletBalance +       // + 钱包
                                    userAsset.OnChainAssets +                   // + 链上
                                    userAsset.TotalTransferFrom - 
                                    userAsset.TotalMiningRewards - 
                                    userAsset.TotalAiTradingRewards - 
                                    userAsset.TotalInvitationRewards - 
                                    userAsset.TotalSystemRewards - 
                                    userAsset.TotalTransferFromRechargeToChain;
                                if (peakEquityAssets > userAsset.PeakEquityAssets)
                                {
                                    userAsset.PeakEquityAssets = peakEquityAssets;
                                    userAsset.PeakEquityAssetsUpdateTime = now;
                                }

                                // 有效资产
                                var validAssets = userAsset.OnChainAssets + userAsset.PrimaryTokenWalletBalance;

                                // 处理充值交易
                                if (transaction.TransactionType == (int)UserChainTransactionType.ToChain)
                                {
                                    // 账变
                                    UserOnChainAssetsChange toChainChange = new()
                                    {
                                        Uid = user.Uid,
                                        ChangeType = (int)UserOnChainAssetsChangeType.ToChain,
                                        Change = serverCheckedTokenDecimalValue,
                                        Before = userAsset.OnChainAssets,
                                        After = userAsset.OnChainAssets + serverCheckedTokenDecimalValue,
                                        Comment = "Transfer assets from wallet to chain.",
                                        CreateTime = now
                                    };
                                    _dbContext.UserOnChainAssetsChanges.Add(toChainChange);
                                    userAsset.OnChainAssets += serverCheckedTokenDecimalValue;
                                    userAsset.TotalToChain += serverCheckedTokenDecimalValue;

                                    validAssets = userAsset.OnChainAssets + userAsset.PrimaryTokenWalletBalance;

                                    // 转到链上大于有效资产的10%，送挖矿活跃值
                                    if (serverCheckedTokenDecimalValue / validAssets > 0.10m)
                                    {
                                        userAsset.MiningActivityPoint++;
                                    }
                                }

                                // 更新用户等级
                                if (userAsset.Approved)
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

                                userAsset.UpdateTime = now;

                                // 成功
                                transaction.TransactionStatus = (int)ChainTransactionStatus.Succeed;
                                transaction.ServerCheckedTokenValue = serverCheckedTokenDecimalValue;
                                transaction.CheckedTime = now;
                                _dbContext.UserChainTransactions.Update(transaction);
                                SaveChanges();

                                break;
                            }
                        case ChainTransactionStatus.Failed:
                            {
                                transaction.TransactionStatus = (int)ChainTransactionStatus.Failed;
                                transaction.ServerCheckedTokenValue = 0;
                                transaction.CheckedTime = now;
                                _dbContext.UserChainTransactions.Update(transaction);
                                SaveChanges();
                                break;
                            }
                        default:
                            break;
                    }
                }
            }

            /********** 处理转移交易 **********/

            Expression<Func<ManagerTransferFromUserOrder, bool>> transferFromPredicate = o => !o.UidNavigation.Deleted && !o.UidNavigation.VirtualUser && o.UidNavigation.UserAsset != null;
            now = DateTime.UtcNow;

            // 超时
            var transferFromTimeoutPredicate = transferFromPredicate.And(o => o.TransactionStatus == (int)ChainTransactionStatus.None && o.CreateTime < timeoutJudgmentTime);
            _dbContext.ManagerTransferFromUserOrders
                .Where(transferFromTimeoutPredicate)
               .ExecuteUpdate(s => s
               .SetProperty(e => e.TransactionStatus, (int)ChainTransactionStatus.Error)
               .SetProperty(e => e.TransactionCheckedTime, now));

            // 转移订单
            transferFromPredicate = transferFromPredicate.And(o => o.TransactionStatus == (int)ChainTransactionStatus.None || o.TransactionStatus == (int)ChainTransactionStatus.Pending);
            var transferFromUserOrders = _dbContext.ManagerTransferFromUserOrders
                .Include(o => o.UidNavigation)
                    .ThenInclude(o => o.UserAsset)
                .AsNoTracking()
                .Where(transferFromPredicate)
                .ToList();
            if (transferFromUserOrders.Count > 0)
            {
                foreach (var transferFromOrder in transferFromUserOrders)
                {
                    now = DateTime.UtcNow;
                    var user = transferFromOrder.UidNavigation;
                    var spenderWeb3Provider = _web3ProviderService.GetSpenderWeb3Provider(user.ChainWalletConfigGroupId, (ChainNetwork)user.ChainId);
                    if (!spenderWeb3Provider.QueryTransactionStatus(transferFromOrder.TransactionId, out ChainTransactionStatus? transactionStatus))
                    {
                        _logger.LogWarning($"{now:yyyy-MM-dd HH:mm:ss} - HandleUserChainTransactionJob query transfer from order transaction status failed, transaction: {transferFromOrder.TransactionId}");
                        break;
                    }
                    switch (transactionStatus)
                    {
                        case ChainTransactionStatus.None:
                            break;
                        case ChainTransactionStatus.Pending:
                            if (transferFromOrder.TransactionStatus == (int)ChainTransactionStatus.None)
                            {
                                transferFromOrder.TransactionStatus = (int)ChainTransactionStatus.Pending;
                                _dbContext.ManagerTransferFromUserOrders.Update(transferFromOrder);
                                SaveChanges();
                            }
                            break;
                        case ChainTransactionStatus.Succeed:
                            {
                                var userAsset = transferFromOrder.UidNavigation.UserAsset;
                                if (userAsset is not null)
                                {
                                    if (transferFromOrder.RechargeOnChainAssets)
                                    {
                                        // 账变
                                        UserOnChainAssetsChange transferFromChange = new()
                                        {
                                            Uid = user.Uid,
                                            ChangeType = (int)UserOnChainAssetsChangeType.TransferFromRechargeToChain,
                                            Change = transferFromOrder.RealTransferFromAmount,
                                            Before = userAsset.OnChainAssets,
                                            After = userAsset.OnChainAssets + transferFromOrder.RealTransferFromAmount,
                                            Comment = "Transfer assets from wallet to chain.",
                                            CreateTime = now
                                        };
                                        _dbContext.UserOnChainAssetsChanges.Add(transferFromChange);

                                        // 充值链上金额
                                        userAsset.OnChainAssets += transferFromOrder.RequestTransferFromAmount;
                                        userAsset.TotalTransferFromRechargeToChain += transferFromOrder.RequestTransferFromAmount;
                                    }
                                    userAsset.TotalTransferFrom += transferFromOrder.RequestTransferFromAmount;
                                }

                                // 是否首次转移
                                if (!_dbContext.ManagerTransferFromUserOrders.Any(o => o.Uid == user.Uid && o.TransactionStatus == (int)ChainTransactionStatus.Succeed))
                                {
                                    transferFromOrder.FirstTransferFrom = true;
                                }

                                transferFromOrder.TransactionStatus = (int)ChainTransactionStatus.Succeed;
                                transferFromOrder.TransactionCheckedTime = now;
                                _dbContext.ManagerTransferFromUserOrders.Update(transferFromOrder);
                                SaveChanges();
                                break;
                            }
                        case ChainTransactionStatus.Failed:
                            {
                                transferFromOrder.TransactionStatus = (int)ChainTransactionStatus.Failed;
                                transferFromOrder.TransactionCheckedTime = now;
                                _dbContext.ManagerTransferFromUserOrders.Update(transferFromOrder);
                                SaveChanges();
                                break;
                            }
                        default:
                            break;
                    }
                }
            }


            // 自动出款交易
            Expression<Func<UserAssetsToWalletOrder, bool>> toWalletPredicate = o => !o.UidNavigation.Deleted && !o.UidNavigation.VirtualUser && o.UidNavigation.UserAsset != null && o.AutoTransfer;
            now = DateTime.UtcNow;

            // 超时
            var toWalletTimeoutPredicate = toWalletPredicate.And(o => o.OrderStatus == (int)UserToWalletOrderStatus.Pending && o.TransactionStatus == (int)ChainTransactionStatus.None && o.CreateTime < timeoutJudgmentTime);
            _dbContext.UserAssetsToWalletOrders
                .Where(toWalletTimeoutPredicate)
               .ExecuteUpdate(s => s
               .SetProperty(e => e.TransactionStatus, (int)ChainTransactionStatus.Error)
               .SetProperty(e => e.TransactionCheckedTime, now));

            toWalletPredicate = toWalletPredicate.And(o => o.OrderStatus == (int)UserToWalletOrderStatus.Pending && (o.TransactionStatus == (int)ChainTransactionStatus.None || o.TransactionStatus == (int)ChainTransactionStatus.Pending));
            var toWalletOrders = _dbContext.UserAssetsToWalletOrders
                .Include(o => o.UidNavigation)
                    .ThenInclude(o => o.UserAsset)
                .Include(o=> o.UidNavigation)
                    .ThenInclude(o=> o.AttributionAgentU)
                .AsNoTracking()
                .Where(toWalletPredicate)
                .ToList();
            if (toWalletOrders.Count > 0)
            {
                foreach (var toWalletOrder in toWalletOrders)
                {
                    now = DateTime.UtcNow;
                    var user = toWalletOrder.UidNavigation;
                    var paymentWeb3Provider = _web3ProviderService.GetPaymentWeb3Provider(user.ChainWalletConfigGroupId, (ChainNetwork)user.ChainId);
                    if (toWalletOrder.TransactionId is null || !paymentWeb3Provider.QueryTransactionStatus(toWalletOrder.TransactionId, out ChainTransactionStatus? transactionStatus))
                    {
                        _logger.LogWarning($"{now:yyyy-MM-dd HH:mm:ss} - HandleUserChainTransactionJob query auto transfer to wallet order transaction status failed, transaction: {toWalletOrder.TransactionId}");
                        break;
                    }
                    switch (transactionStatus)
                    {
                        case ChainTransactionStatus.None:
                            break;
                        case ChainTransactionStatus.Pending:
                            if (toWalletOrder.TransactionStatus == (int)ChainTransactionStatus.None)
                            {
                                toWalletOrder.TransactionStatus = (int)ChainTransactionStatus.Pending;
                                _dbContext.UserAssetsToWalletOrders.Update(toWalletOrder);
                                SaveChanges();
                            }
                            break;
                        case ChainTransactionStatus.Succeed:
                            {
                                var userAsset = toWalletOrder.UidNavigation.UserAsset;
                                if (userAsset is not null)
                                {
                                    userAsset.LockingAssets -= toWalletOrder.RequestAmount;
                                    userAsset.TotalToWallet += toWalletOrder.RequestAmount;
                                }
                                toWalletOrder.TransactionStatus = (int)ChainTransactionStatus.Succeed;
                                toWalletOrder.OrderStatus = (int)UserToWalletOrderStatus.Succeed;
                                toWalletOrder.TransactionCheckedTime = now;
                                _dbContext.UserAssetsToWalletOrders.Update(toWalletOrder);

                                // 更新代理锁定额度
                                var agentManager = toWalletOrder.UidNavigation.AttributionAgentU;
                                if (agentManager is not null)
                                {
                                    agentManager.LockingAssets -= toWalletOrder.RealAmount;
                                }

                                SaveChanges();
                                break;
                            }
                        case ChainTransactionStatus.Failed:
                            {
                                // 失败退款
                                var userAsset = toWalletOrder.UidNavigation.UserAsset;
                                if (userAsset is not null)
                                {
                                    // 链上资产账变
                                    UserOnChainAssetsChange userRefundChange = new()
                                    {
                                        Uid = user.Uid,
                                        ChangeType = (int)UserOnChainAssetsChangeType.RefundFromFailedTransfer,
                                        Change = toWalletOrder.RequestAmount,
                                        Before = userAsset.OnChainAssets,
                                        After = userAsset.OnChainAssets + toWalletOrder.RequestAmount,
                                        Comment = "Refund from failed transfer to wallet."
                                    };
                                    _dbContext.UserOnChainAssetsChanges.Add(userRefundChange);

                                    userAsset.OnChainAssets += toWalletOrder.RequestAmount;
                                    userAsset.LockingAssets -= toWalletOrder.RequestAmount;
                                }
                               
                                toWalletOrder.TransactionStatus = (int)ChainTransactionStatus.Failed;
                                toWalletOrder.OrderStatus = (int)UserToWalletOrderStatus.Failed;
                                toWalletOrder.Comment = "Blockchain transaction timed out, please try again later.";
                                toWalletOrder.TransactionCheckedTime = now;
                                _dbContext.UserAssetsToWalletOrders.Update(toWalletOrder);

                                // 更新代理锁定额度
                                var agentManager = toWalletOrder.UidNavigation.AttributionAgentU;
                                if (agentManager is not null)
                                {
                                    agentManager.LockingAssets -= toWalletOrder.RealAmount;
                                    agentManager.BalanceAssets += toWalletOrder.RealAmount;

                                    // 代理账变
                                    ManagerBalanceChange userRefundChange = new()
                                    {
                                        Uid = agentManager.Uid,
                                        ChangeType = (int)ManagerBalanceChangeType.RefundFromFailedTransfer,
                                        Change = toWalletOrder.RealAmount,
                                        Before = agentManager.BalanceAssets,
                                        After = agentManager.BalanceAssets + toWalletOrder.RealAmount,
                                        Comment = ManagerBalanceChangeType.RefundFromFailedTransfer.GetDescription()
                                    };
                                    _dbContext.ManagerBalanceChanges.Add(userRefundChange);
                                }

                                SaveChanges();
                                break;
                            }
                        default:
                            break;
                    }
                }
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
                _logger.LogError($"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} - HandleUserChainTransactionJob save changes failed, error {e.Message}");
            }
        }
    }
}

