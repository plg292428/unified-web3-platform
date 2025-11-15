namespace UnifiedPlatform.Shared.ActionModels.Request
{
    /// <summary>
    /// 更新物流状态请求
    /// </summary>
    public class ShippingStatusUpdateRequest
    {
        /// <summary>
        /// 物流ID
        /// </summary>
        public long ShippingId { get; set; }

        /// <summary>
        /// 物流状态
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// 状态描述
        /// </summary>
        public string? StatusDescription { get; set; }

        /// <summary>
        /// 位置信息
        /// </summary>
        public string? Location { get; set; }

        /// <summary>
        /// 时间戳
        /// </summary>
        public DateTime? Timestamp { get; set; }
    }
}

