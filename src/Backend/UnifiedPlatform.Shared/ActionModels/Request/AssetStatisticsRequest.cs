namespace UnifiedPlatform.Shared.ActionModels.Request
{
    /// <summary>
    /// 资产统计请求
    /// </summary>
    public class AssetStatisticsRequest
    {
        /// <summary>
        /// 开始日期（默认30天前）
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// 结束日期（默认今天）
        /// </summary>
        public DateTime? EndDate { get; set; }
    }
}

