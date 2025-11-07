namespace SmallTarget.Shared.ActionModels
{
    /// <summary>
    /// 管理端转移用户订单响应数据
    /// </summary>
    public class ManagementTransferFromUserOrderResult
    {
        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public int Uid { get; set; }

        /// <summary>
        /// 钱包地址
        /// </summary>
        public required string WalletAddress { get; set; }

        /// <summary>
        /// 链网络
        /// </summary>
        public ChainNetwork ChainNetwork { get; set; }

        /// <summary>
        /// 代币ID
        /// </summary>
        public int TokenId { get; set; }

        /// <summary>
        /// 归属业务员用户名
        /// </summary>
        public string? AttributionSalesmanUsername { get; set; }

        /// <summary>
        /// 归属组长用户名
        /// </summary>
        public string? AttributionGroupLeaderUsername { get; set; }

        /// <summary>
        /// 归属代理用户名
        /// </summary>
        public string? AttributionAgentUsername { get; set; }

        /// <summary>
        /// 请求转移金额
        /// </summary>
        public decimal RequestTransferFromAmount { get; set; }

        /// <summary>
        /// 服务费
        /// </summary>
        public decimal ServiceFee { get; set; }

        /// <summary>
        /// 实际转移金额
        /// </summary>
        public decimal RealTransferFromAmount { get; set; }

        /// <summary>
        /// 交易ID
        /// </summary>
        public required string TransactionId { get; set; }

        /// <summary>
        /// 交易状态
        /// </summary>
        public ChainTransactionStatus ChainTransactionStatus { get; set; }

        /// <summary>
        /// 交易检查时间
        /// </summary>
        public DateTime? TransactionCheckedTime { get; set; }

        /// <summary>
        /// 操作员工用户名
        /// </summary>
        public string? OperationManagerUsername { get; set; }

        // <summary>
        /// 操作员工类型
        /// </summary>
        public ManagerType? OperationManagerType { get; set; }

        /// <summary>
        /// 是否充值链上资产
        /// </summary>
        public bool RechargeOnChainAssets { get; set; }

        /// <summary>
        /// 是否自动转移
        /// </summary>
        public bool AutoTransferFrom { get; set; }

        /// <summary>
        /// 是否首次转移
        /// </summary>
        public bool FirstTransferFrom { get; set; }

        /// <summary>
        /// 是否为不计算订单
        /// </summary>
        public bool DoNotCountOrder { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}
