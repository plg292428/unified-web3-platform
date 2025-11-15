namespace UnifiedPlatform.Shared.ActionModels.Result
{
    /// <summary>
    /// AI交易订单结果
    /// </summary>
    public class AiTradingOrderResult
    {
        /// <summary>
        /// 订单ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 交易金额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 奖励比例
        /// </summary>
        public decimal? RewardRate { get; set; }

        /// <summary>
        /// 奖励金额
        /// </summary>
        public decimal? Reward { get; set; }

        /// <summary>
        /// 订单状态
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 订单状态名称
        /// </summary>
        public string StatusName { get; set; } = string.Empty;

        /// <summary>
        /// 订单结束时间
        /// </summary>
        public DateTime OrderEndTime { get; set; }

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

