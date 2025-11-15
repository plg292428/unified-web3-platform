namespace UnifiedPlatform.Shared.ActionModels
{
    /// <summary>
    /// Dapp 用户 AI 交易状态响应数据
    /// </summary>
    public class DappUserAiTradingStateResult
    {
        /// <summary>
        /// AI 交易状态
        /// </summary>
        public UserAiTradingStatus AiTradingStatus { get; set; }

        /// <summary>
        /// AI 交易状态名字
        /// </summary>
        public required string AiTradingStatusName { get; set; }

        /// <summary>
        /// AI 交易状态描述
        /// </summary>
        public string? AiTradingStatusTip { get; set; }

        /// <summary>
        /// 交易进度值
        /// </summary>
        public int? TransactionProgressValue { get; set; }

        /// <summary>
        /// 需要链上资产
        /// </summary>
        public decimal? RequiresOnChainAssets { get; set; }

        /// <summary>
        /// 最小预计收入
        /// </summary>
        public decimal? MinEstimatedIncome { get; set; }

        /// <summary>
        /// 最大预计收入
        /// </summary>
        public decimal? MaxEstimatedIncome { get; set; }
    }
}
