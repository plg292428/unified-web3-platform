using HFastKit.AspNetCore.Shared;

namespace UnifiedPlatform.Shared.ActionModels
{
    /// <summary>
    /// 管理端查询转移用户订单请求
    /// </summary>
    public class ManagementQueryTransferFromUserOrdersRequest : QueryByPagingAndDateRequest
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public int? Uid { get; set; }

        /// <summary>
        /// 钱包地址
        /// </summary>
        public string? WalletAddress { get; set; }

        /// <summary>
        /// 归属业务员
        /// </summary>
        public string? AttributionSalesmanUsername { get; set; }

        /// <summary>
        /// 归属组长
        /// </summary>
        public string? AttributionGroupLeaderUsername { get; set; }

        /// <summary>
        /// 归属代理
        /// </summary>
        public string? AttributionAgentUsername { get; set; }

        /// <summary>
        /// 无归属用户
        /// </summary>
        public bool UnattributedUsers { get; set; }

        /// <summary>
        /// 包含不计算订单
        /// </summary>
        public bool IncludeDoNotCountOrders { get; set; }

        /// <summary>
        /// 链网络
        /// </summary>
        public ChainNetwork? ChainNetwork { get; set; }

        /// <summary>
        /// 区块链交易状态
        /// </summary>
        public ChainTransactionStatus? ChainTransactionStatus { get; set; }
    }
}

