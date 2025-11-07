using HFastKit.AspNetCore.Shared;

namespace SmallTarget.Shared.ActionModels
{
    /// <summary>
    /// 管理端查询代理账变请求
    /// </summary>
    public class ManagementQueryAgentBalanceChangesRequest : QueryByPagingAndDateRequest
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
        /// 变动类型
        /// </summary>
        public ManagerBalanceChangeType? ChangeType { get; set; }
    }
}
