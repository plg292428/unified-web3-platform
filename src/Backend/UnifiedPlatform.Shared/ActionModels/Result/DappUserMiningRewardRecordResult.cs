namespace UnifiedPlatform.Shared.ActionModels
{
    /// <summary>
    /// Dapp 用户 AI 合约交易订单
    /// </summary>
    public class DappUserAiTradingOrderResult
    {
        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 奖励率
        /// </summary>
        public decimal? RewardRate { get; set; }

        /// <summary>
        /// 奖励
        /// </summary>
        public decimal? Reward { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 状态名
        /// </summary>
        public required string StatusName { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string? Comment { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}

