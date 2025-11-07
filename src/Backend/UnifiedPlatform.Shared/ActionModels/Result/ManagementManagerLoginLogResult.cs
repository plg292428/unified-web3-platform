namespace SmallTarget.Shared.ActionModels
{
    /// <summary>
    /// 管理端员工登录日志响应数据
    /// </summary>
    public class ManagementManagerLoginLogResult
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
        /// 用户名
        /// </summary>
        public required string Username { get; set; }

        /// <summary>
        /// 员工类型
        /// </summary>
        public ManagerType ManagerType { get; set; }

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
