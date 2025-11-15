namespace UnifiedPlatform.Shared.ActionModels.Result
{
    /// <summary>
    /// 转账到链上订单结果
    /// </summary>
    public class TransferToChainOrderResult
    {
        /// <summary>
        /// 订单ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 交易ID
        /// </summary>
        public string TransactionId { get; set; } = string.Empty;

        /// <summary>
        /// 代币ID
        /// </summary>
        public int TokenId { get; set; }

        /// <summary>
        /// 代币名称
        /// </summary>
        public string TokenName { get; set; } = string.Empty;

        /// <summary>
        /// 代币符号
        /// </summary>
        public string TokenSymbol { get; set; } = string.Empty;

        /// <summary>
        /// 转账金额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 交易状态
        /// </summary>
        public int TransactionStatus { get; set; }

        /// <summary>
        /// 交易状态名称
        /// </summary>
        public string TransactionStatusName { get; set; } = string.Empty;

        /// <summary>
        /// 检查时间
        /// </summary>
        public DateTime? CheckedTime { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}

