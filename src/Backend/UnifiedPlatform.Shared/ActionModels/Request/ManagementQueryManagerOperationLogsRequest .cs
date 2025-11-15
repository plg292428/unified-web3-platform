using HFastKit.AspNetCore.Shared;

namespace UnifiedPlatform.Shared.ActionModels
{
    /// <summary>
    /// 管理端查询员工操作日志请求
    /// </summary>
    public class ManagementQueryManagerOperationLogsRequest : QueryByPagingAndDateRequest
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
        /// 操作类型
        /// </summary>
        public ManagerOperationType? OperationType { get; set; }
    }
}

