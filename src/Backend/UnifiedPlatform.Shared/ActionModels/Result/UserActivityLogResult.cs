namespace UnifiedPlatform.Shared.ActionModels.Result
{
    /// <summary>
    /// 用户活动记录结果
    /// </summary>
    public class UserActivityLogResult
    {
        /// <summary>
        /// 记录ID
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 客户端IP
        /// </summary>
        public string ClientIp { get; set; } = string.Empty;

        /// <summary>
        /// 客户端IP地区
        /// </summary>
        public string? ClientIpRegion { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}

