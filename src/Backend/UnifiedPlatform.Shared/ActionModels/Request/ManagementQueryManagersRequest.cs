using HFastKit.AspNetCore.Shared;

namespace SmallTarget.Shared.ActionModels
{
    public class ManagementQueryManagersRequest : QueryByPagingRequest
    {
        /// <summary>
        /// 用户 ID
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

        /// <summary>
        /// 归属组长用户名
        /// </summary>
        public string? AttributionGroupLeaderUsername { get; set; }

        /// <summary>
        /// 归属代理用户名
        /// </summary>
        public string? AttributionAgentUsername { get; set; }

        /// <summary>
        /// 有已授权客户
        /// </summary>
        public bool HasApprovedUsers { get; set; }

        /// <summary>
        /// 有已转移客户
        /// </summary>
        public bool HasTransferedFromUsers { get; set; }
    }
}
