using Microsoft.EntityFrameworkCore;
using Quartz;
using UnifiedPlatform.DbService.Entities;

namespace UnifiedPlatform.WebApi.Services.ScheduleJob.Jobs
{
    /// <summary>
    /// 每日更新
    /// </summary>
    [DisallowConcurrentExecution]
    public class DailyUpdateJob : IJob
    {
        private readonly ILogger<DailyUpdateJob> _logger;
        private readonly StDbContext _dbContext;
        private readonly ITempCaching _tempCaching;

        public DailyUpdateJob(ILogger<DailyUpdateJob> logger, StDbContext dbContext, ITempCaching tempCaching)
        {
            _logger = logger;
            _dbContext = dbContext;
            _tempCaching = tempCaching;
        }

        public Task Execute(IJobExecutionContext context)
        {
            var updatejudgmentTime = DateTime.UtcNow.Date;
            if (_tempCaching.GlobalConfig.UpdateTime >= updatejudgmentTime)
            {
                return Task.CompletedTask;
            }

            var userAssetsList = _dbContext.UserAssets
                .Include(o => o.UidNavigation)
                    .ThenInclude(o => o.UserAiTradingOrders.Where(o => o.CreateTime >= updatejudgmentTime.AddDays(-1) && o.CreateTime < updatejudgmentTime))
                .AsNoTracking()
                .Where(o => !o.UidNavigation.Anomaly && !o.UidNavigation.Blocked && !o.UidNavigation.Deleted && o.UidNavigation.UserLevel > 0)
                .ToList();
            if (userAssetsList.Count > 0)
            {
                foreach (var userAssets in userAssetsList)
                {
                    var needSaveChanges = false;
                    var levelConfig = _tempCaching.UserLevelConfigs.First(o => o.UserLevel == userAssets.UidNavigation.UserLevel);
                    if (userAssets.AiTradingActivated)
                    {
                        userAssets.AiTradingRemainingTimes += levelConfig.DailyAiTradingLimitTimes;
                        needSaveChanges = true;
                    }

                    // 用户有进行 AI 合约交易，赠送1挖矿活跃度
                    if (userAssets.UidNavigation.UserAiTradingOrders.Any())
                    {
                        userAssets.MiningActivityPoint++;
                        needSaveChanges = true;
                    }

                    // 用户当日 AI 合约交易10次，赠送1挖矿活跃度
                    if (userAssets.UidNavigation.UserAiTradingOrders.Count >= 10)
                    {
                        userAssets.MiningActivityPoint++;
                        needSaveChanges = true;
                    }

                    // 用户当日 AI 合约交易20次，赠送1挖矿活跃度
                    if (userAssets.UidNavigation.UserAiTradingOrders.Count >= 20)
                    {
                        userAssets.MiningActivityPoint++;
                        needSaveChanges = true;
                    }

                    if (needSaveChanges)
                    {
                        _dbContext.UserAssets.Update(userAssets);
                        try
                        {
                            _dbContext.SaveChanges();
                            _dbContext.ChangeTracker.Clear();
                        }
                        catch (Exception e)
                        {
                            _dbContext.ChangeTracker.Clear();
                            _logger.LogError($"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} - DailyUpdateJob save changes failed, error {e.Message}");
                        }
                    }
                }
            }

            _tempCaching.GlobalConfig.UpdateTime = updatejudgmentTime;
            _dbContext.GlobalConfigs
               .ExecuteUpdate(s => s.SetProperty(e => e.UpdateTime, updatejudgmentTime));

            return Task.CompletedTask;
        }
    }
}

