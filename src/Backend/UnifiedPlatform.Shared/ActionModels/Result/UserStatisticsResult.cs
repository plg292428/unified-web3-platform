namespace UnifiedPlatform.Shared.ActionModels.Result
{
    /// <summary>
    /// 用户统计结果
    /// </summary>
    public class UserStatisticsResult
    {
        /// <summary>
        /// 总邀请用户数
        /// </summary>
        public int TotalInvitedUsers { get; set; }

        /// <summary>
        /// 一级成员数
        /// </summary>
        public int Layer1Members { get; set; }

        /// <summary>
        /// 二级成员数
        /// </summary>
        public int Layer2Members { get; set; }

        /// <summary>
        /// 总链上交易数
        /// </summary>
        public int TotalChainTransactions { get; set; }

        /// <summary>
        /// 总AI交易订单数
        /// </summary>
        public int TotalAiTradingOrders { get; set; }

        /// <summary>
        /// 总提现订单数
        /// </summary>
        public int TotalWithdrawOrders { get; set; }

        /// <summary>
        /// 账户年龄（天）
        /// </summary>
        public int AccountAge { get; set; }

        /// <summary>
        /// 总奖励
        /// </summary>
        public decimal TotalRewards { get; set; }
    }
}

