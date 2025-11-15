using HFastKit.AspNetCore.Shared;

namespace UnifiedPlatform.Shared.ActionModels.Request
{
    /// <summary>
    /// 用户活动记录查询请求
    /// </summary>
    public class UserActivityLogRequest
    {
        /// <summary>
        /// 页码
        /// </summary>
        public int? PageIndex { get; set; }

        /// <summary>
        /// 每页大小
        /// </summary>
        public int? PageSize { get; set; }

        /// <summary>
        /// 开始日期
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// 结束日期
        /// </summary>
        public DateTime? EndDate { get; set; }
    }
}

