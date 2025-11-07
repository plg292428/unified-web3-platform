using Quartz;
using Quartz.AspNetCore;
using SmallTarget.WebApi.Services.ScheduleJob.Jobs;

namespace SmallTarget.WebApi.Services
{
    public static class ScheduleJobExtensions
    {
        /// <summary>
        /// 添加计划任务
        /// </summary>
        /// <param name="services">服务</param>
        public static void AddScheduleJobs(this IServiceCollection services)
        {
            services.AddQuartz(q =>
            {
                q.SchedulerId = "Scheduler Jobs";

                // 简易加载器
                q.UseSimpleTypeLoader();

                // 内存状态
                q.UseInMemoryStore();

                // 默认线程池
                q.UseDefaultThreadPool(options => { options.MaxConcurrency = 10; });

                // 处理AI合约交易 - 每分钟
                q.ScheduleJob<HandleUserAiTradingOrderJob>(trigger => trigger
                    .WithIdentity(typeof(HandleUserAiTradingOrderJob).ToString())
                    .WithDescription(typeof(HandleUserAiTradingOrderJob).ToString())
                    .WithCronSchedule("5 0/1 * * * ?")
                );

                // 处理链转账到钱包链交易、转移订单链交易、自动出款订单链交易 - 每3分钟
                q.ScheduleJob<HandleUserChainTransactionJob>(trigger => trigger
                    .WithIdentity(typeof(HandleUserChainTransactionJob).ToString())
                    .WithDescription(typeof(HandleUserChainTransactionJob).ToString())
                    .WithCronSchedule("30 0/3 * * * ?")
                );

                // 更新用户钱包、自动转移、挖矿奖励 - 每10分钟
                q.ScheduleJob<HandleUserAssetsAndAutoTransferFromJob>(trigger => trigger
                    .WithIdentity(typeof(HandleUserAssetsAndAutoTransferFromJob).ToString())
                    .WithDescription(typeof(HandleUserAssetsAndAutoTransferFromJob).ToString())
                    .WithCronSchedule("0 0/10 * * * ?")
                );

                // 日常更新 - 启动执行一次后，每日执行一次
                var handleUserRewardJobKey = new JobKey(typeof(DailyUpdateJob).ToString(), "Daily Update Job Group");
                q.AddJob<DailyUpdateJob>(handleUserRewardJobKey, j => j
                    .WithDescription(typeof(DailyUpdateJob).ToString())
                );
                q.AddTrigger(t => t
                    .ForJob(handleUserRewardJobKey)
                    .WithIdentity(typeof(DailyUpdateJob).ToString() + "Once")
                    .WithDescription(typeof(DailyUpdateJob).ToString() + "Once")
                    .StartAt(DateBuilder.FutureDate(5, IntervalUnit.Second))
                );
                q.AddTrigger(t => t
                    .ForJob(handleUserRewardJobKey)
                    .WithIdentity(typeof(DailyUpdateJob).ToString())
                    .WithDescription(typeof(DailyUpdateJob).ToString())
                    .WithCronSchedule("30 0 0 1/1 * ?")
                );
            });

            services.AddQuartzServer(options =>
            {
                options.WaitForJobsToComplete = true;
            });
        }
    }
}
