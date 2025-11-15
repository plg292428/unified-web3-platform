namespace UnifiedPlatform.Shared.ActionModels.Result
{
    /// <summary>
    /// 资产统计结果
    /// </summary>
    public class AssetStatisticsResult
    {
        /// <summary>
        /// 总充值
        /// </summary>
        public decimal TotalDeposit { get; set; }

        /// <summary>
        /// 总提现
        /// </summary>
        public decimal TotalWithdraw { get; set; }

        /// <summary>
        /// 总AI交易奖励
        /// </summary>
        public decimal TotalAiTradingRewards { get; set; }

        /// <summary>
        /// 总挖矿奖励
        /// </summary>
        public decimal TotalMiningRewards { get; set; }

        /// <summary>
        /// 总邀请奖励
        /// </summary>
        public decimal TotalInvitationRewards { get; set; }

        /// <summary>
        /// 总系统奖励
        /// </summary>
        public decimal TotalSystemRewards { get; set; }

        /// <summary>
        /// 当前余额
        /// </summary>
        public decimal CurrentBalance { get; set; }

        /// <summary>
        /// 当前锁定
        /// </summary>
        public decimal CurrentLocking { get; set; }

        /// <summary>
        /// 当前可用
        /// </summary>
        public decimal CurrentAvailable { get; set; }

        /// <summary>
        /// 统计开始日期
        /// </summary>
        public DateTime PeriodStartDate { get; set; }

        /// <summary>
        /// 统计结束日期
        /// </summary>
        public DateTime PeriodEndDate { get; set; }
    }
}

