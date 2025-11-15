namespace UnifiedPlatform.Shared.ActionModels.Result
{
    /// <summary>
    /// 转账到钱包订单结果
    /// </summary>
    public class TransferToWalletOrderResult
    {
        /// <summary>
        /// 订单ID
        /// </summary>
        public int Id { get; set; }

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
        /// 请求金额
        /// </summary>
        public decimal RequestAmount { get; set; }

        /// <summary>
        /// 服务费
        /// </summary>
        public decimal ServiceFee { get; set; }

        /// <summary>
        /// 实际到账金额
        /// </summary>
        public decimal RealAmount { get; set; }

        /// <summary>
        /// 订单状态
        /// </summary>
        public int OrderStatus { get; set; }

        /// <summary>
        /// 订单状态名称
        /// </summary>
        public string OrderStatusName { get; set; } = string.Empty;

        /// <summary>
        /// 交易哈希
        /// </summary>
        public string? TransactionHash { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; }
    }
}

