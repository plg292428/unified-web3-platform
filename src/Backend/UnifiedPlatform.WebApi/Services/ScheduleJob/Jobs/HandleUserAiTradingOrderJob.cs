using HFastKit.AspNetCore.Shared.Extensions;
using Microsoft.EntityFrameworkCore;
using Quartz;
using UnifiedPlatform.DbService.Entities;
using UnifiedPlatform.Shared;

namespace UnifiedPlatform.WebApi.Services.ScheduleJob.Jobs
{
    /// <summary>
    /// 处理用户 AI 合约交易
    /// </summary>
    [DisallowConcurrentExecution]
    public class HandleUserAiTradingOrderJob : IJob
    {
        private readonly ILogger<HandleUserAiTradingOrderJob> _logger;
        private readonly StDbContext _dbContext;
        private readonly ITempCaching _tempCaching;

        public HandleUserAiTradingOrderJob(ILogger<HandleUserAiTradingOrderJob> logger, StDbContext dbContext, ITempCaching tempCaching)
        {
            _logger = logger;
            _dbContext = dbContext;
            _tempCaching = tempCaching;
        }

        public Task Execute(IJobExecutionContext context)
        {
            var now = DateTime.UtcNow;
            var userAiTradingOrders = _dbContext.UserAiTradingOrders
                .Include(o => o.UidNavigation)
                    .ThenInclude(o => o.UserAsset)
                .AsNoTracking()
                .Where(o => o.UidNavigation.UserAsset != null && o.OrderEndTime <= now && o.Status == (int)UserAiTradingOrderStatus.Trading)
                .ToList();
            if (userAiTradingOrders.Count < 1)
            {
                return Task.CompletedTask;
            }

            foreach (var tradingOrder in userAiTradingOrders)
            {
                var user = tradingOrder.UidNavigation;
                var userAssets = user.UserAsset;
                if (userAssets is null)
                {
                    _logger.LogError($"{now:yyyy-MM-dd HH:mm:ss} - HandleUserAiTradingOrderJob Invalid user assets : {user.Uid}");
                    continue;
                }

                // 异常、封停、逻辑删除用户
                if (user.Anomaly || user.Blocked || user.Deleted)
                {
                    tradingOrder.Status = (int)UserAiTradingOrderStatus.Failed;
                    tradingOrder.RewardRate = 0;
                    tradingOrder.Reward = 0;
                    userAssets.BlackHoleAssets += tradingOrder.Amount;
                    _dbContext.UserAiTradingOrders.Update(tradingOrder);
                    SaveChanges();
                    continue;
                }

                // 计算奖励
                var levelConfig = _tempCaching.UserLevelConfigs.First(o => o.UserLevel == user.UserLevel);
                var minAiTradingRewardRate = levelConfig.MinEachAiTradingRewardRate;
                var maxAiTradingRewardRate = levelConfig.MaxEachAiTradingRewardRate;
                var realAiTradingRewardRate = (decimal)(Random.Shared.NextDouble() * ((double)maxAiTradingRewardRate - (double)minAiTradingRewardRate) + (double)minAiTradingRewardRate);
                realAiTradingRewardRate = realAiTradingRewardRate.FixedToZero();
                var aiTradingReward = tradingOrder.Amount * realAiTradingRewardRate;
                aiTradingReward = aiTradingReward.FixedToZero();
                if (aiTradingReward <= Web3Provider.MinTokenDecimalValue)
                {
                    tradingOrder.Status = (int)UserAiTradingOrderStatus.Failed;
                    tradingOrder.RewardRate = 0;
                    tradingOrder.Reward = 0;
                    userAssets.BlackHoleAssets += tradingOrder.Amount;
                    userAssets.LockingAssets -= tradingOrder.Amount;
                    _dbContext.UserAiTradingOrders.Update(tradingOrder);
                    SaveChanges();
                    _logger.LogError($"{now:yyyy-MM-dd HH:mm:ss} - HandleUserAiTradingOrderJob Invalid user ai trading order : {tradingOrder.Id}");
                    continue;
                }
                tradingOrder.RewardRate = realAiTradingRewardRate;
                tradingOrder.Reward = aiTradingReward;
                var chanegAmount = aiTradingReward + tradingOrder.Amount;

                // 链上资产账变
                UserOnChainAssetsChange aiTradingRewardChange = new()
                {
                    Uid = user.Uid,
                    ChangeType = (int)UserOnChainAssetsChangeType.AiContractTradingIncome,
                    Change = chanegAmount,
                    Before = userAssets.OnChainAssets,
                    After = userAssets.OnChainAssets + chanegAmount,
                    Comment = "Principal and income from AI contract trading."
                };
                _dbContext.UserOnChainAssetsChanges.Add(aiTradingRewardChange);

                userAssets.OnChainAssets += chanegAmount;
                userAssets.LockingAssets -= tradingOrder.Amount;
                userAssets.TotalAiTradingRewards += aiTradingReward;

                tradingOrder.Status = (int)UserAiTradingOrderStatus.Completed;
                _dbContext.UserAiTradingOrders.Update(tradingOrder);
                SaveChanges();

                // 上级奖励
                var parentUsersPathNodes = _dbContext.UserPathNodes
                    .Include(o => o.UidNavigation)
                        .ThenInclude(o => o.UserAsset)
                    .AsNoTracking()
                    .Where(o => o.SubUserUid == user.Uid && o.SubUserLayer <= 2)
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
                    decimal invitationReward = (aiTradingReward * invitationRewardRate).FixedToZero();
                    if (invitationReward <= Web3Provider.MinTokenDecimalValue)
                    {
                        continue;
                    }
                    var parentUser = userPathNode.UidNavigation;

                    // 邀请奖励记录
                    UserInvitationRewardRecord invitationRewardRecord = new()
                    {
                        Uid = parentUser.Uid,
                        SubUserUid = user.Uid,
                        SubUserLayer = userPathNode.SubUserLayer,
                        SubUserReward = aiTradingReward,
                        SubUserRewardType = (int)UserInvitationRewardType.AiContractTrading,
                        RewardRate = invitationRewardRate,
                        Reward = invitationReward,
                        Comment = $"Income from sub user(layer-{userPathNode.SubUserLayer}) Ai contract trading.",
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
                        Comment = $"Income from sub user(layer-{userPathNode.SubUserLayer}) stake-free mining."
                    };
                    _dbContext.UserOnChainAssetsChanges.Add(invitationRewardChange);

                    parentUser.UserAsset.OnChainAssets += invitationReward;
                    parentUser.UserAsset.TotalInvitationRewards += invitationReward;
                    _dbContext.UserAssets.Update(parentUser.UserAsset);
                    SaveChanges();
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
                _logger.LogWarning($"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} - HandleUserAiTradingOrderJob save changes failed, error {e.Message}");
            }
        }
    }
}

