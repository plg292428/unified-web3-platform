using HFastKit.AspNetCore.Shared;
using System.ComponentModel.DataAnnotations;

namespace UnifiedPlatform.Shared.ActionModels
{
    /// <summary>
    /// 管理端查询统计请求
    /// </summary>
    public class ManagementQueryStatisticsRequest: QueryByDateRequest
    {
        /// <summary>
        /// 用户名
        /// </summary>
        [MaxLength(30)]
        public string? Username { get; set; }

        /// <summary>
        /// 是否包含下级
        /// </summary>
        public bool IncludeSubManagers { get; set; }
    }
}

