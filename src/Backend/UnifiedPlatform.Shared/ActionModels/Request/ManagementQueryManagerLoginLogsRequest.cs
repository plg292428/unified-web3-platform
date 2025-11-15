using HFastKit.AspNetCore.Shared;

namespace UnifiedPlatform.Shared.ActionModels
{
    /// <summary>
    /// 管理端查询员工登录日志请求
    /// </summary>
    public class ManagementQueryManagerLoginLogsRequest : QueryByPagingAndDateRequest
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public int? Uid { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string? Username { get; set; }

        /// <summary>
        /// 员工类型
        /// </summary>
        public ManagerType? ManagerType { get; set; }
    }
}

