namespace UnifiedPlatform.Shared.ActionModels.Request
{
    /// <summary>
    /// 资产历史记录查询请求
    /// </summary>
    public class AssetHistoryRequest
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
        /// 变化类型
        /// </summary>
        public int? ChangeType { get; set; }

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

