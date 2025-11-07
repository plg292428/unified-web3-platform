using HFastKit.AspNetCore.Shared;

namespace SmallTarget.Shared.ActionModels
{
    /// <summary>
    /// 管理端查询用户登录日志请求
    /// </summary>
    public class ManagementQueryUserLoginLogsRequest : QueryByPagingAndDateRequest
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
        /// 链网络
        /// </summary>
        public ChainNetwork? ChainNetwork { get; set; }

        /// <summary>
        /// 查询用户类型
        /// </summary>
        public ManagementQueryUserType? QueryUserType { get; set; }
    }
}
