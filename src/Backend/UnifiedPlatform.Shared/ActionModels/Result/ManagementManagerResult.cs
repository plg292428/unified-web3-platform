namespace SmallTarget.Shared.ActionModels
{
    /// <summary>
    /// 管理端员工响应数据
    /// </summary>
    public class ManagementManagerResult
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public int Uid { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// 员工类型
        /// </summary>
        public ManagerType ManagerType { get; set; }

        /// <summary>
        /// 员工类型名
        /// </summary>
        public string ManagerTypeName { get; set; } = string.Empty;

        /// <summary>
        /// 归属组长 UID
        /// </summary>
        public int? AttributionGroupLeaderUid { get; set; }

        /// <summary>
        /// 归属组长用户名
        /// </summary>
        public string? AttributionGroupLeaderUsername { get; set; }

        /// <summary>
        /// 归属代理UID
        /// </summary>
        public int? AttributionAgentUid { get; set; }

        /// <summary>
        /// 归属代理用户名
        /// </summary>
        public string? AttributionAgentUsername { get; set; }

        /// <summary>
        /// 下级组长数
        /// </summary>
        public int SubGroupLeaders { get; set; }

        /// <summary>
        /// 下级业务员数
        /// </summary>
        public int SubSalesmans { get; set; }

        /// <summary>
        /// 已授权客户数
        /// </summary>
        public int ApprovedDappUsers { get; set; }

        /// <summary>
        /// 账户余额
        /// </summary>
        public decimal BalanceAssets { get; set; }

        /// <summary>
        /// 是否开启客服服务
        /// </summary>
        public bool OnlineCustomerServiceEnabled { get; set; }

        /// <summary>
        /// 客服
        /// </summary>
        public string? OnlineCustomerServiceChatWootKey { get; set; }

        /// <summary>
        /// 封停状态
        /// </summary>
        public bool Blocked { get; set; }

        /// <summary>
        /// 是否仅开发者可见
        /// </summary>
        public bool? OnlyDeveloperVisible { get; set; }

        /// <summary>
        /// 首次登录IP
        /// </summary>
        public string? SignUpClientIp { get; set; }

        /// <summary>
        /// 首次登录IP归属
        /// </summary>
        public string? SignUpClientIpRegion { get; set; }

        /// <summary>
        /// 最后登录IP
        /// </summary>
        public string? LastSignInClientIp { get; set; }

        /// <summary>
        /// 最后登录IP归属
        /// </summary>
        public string? LastSignInClientIpRegion { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}
