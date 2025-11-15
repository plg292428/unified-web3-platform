namespace UnifiedPlatform.Shared.ActionModels
{
    /// <summary>
    /// 管理端用户登录日志响应数据
    /// </summary>
    public class ManagementUserLoginLogResult
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
        /// 虚拟用户
        /// </summary>
        public bool VirtualUser { get; set; }

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
        /// 授权
        /// </summary>
        public bool Approved { get; set; }

        /// <summary>
        /// 客户端IP
        /// </summary>
        public required string ClientIp { get; set; }

        /// <summary>
        /// 客户端IP归属地
        /// </summary>
        public string? ClientIpRegion { get; set; }

        /// <summary>
        /// 账变时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}

