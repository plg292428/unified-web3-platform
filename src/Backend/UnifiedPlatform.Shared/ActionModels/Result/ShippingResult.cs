namespace UnifiedPlatform.Shared.ActionModels.Result
{
    /// <summary>
    /// 物流信息结果
    /// </summary>
    public class ShippingResult
    {
        /// <summary>
        /// 物流ID
        /// </summary>
        public long ShippingId { get; set; }

        /// <summary>
        /// 订单ID
        /// </summary>
        public long OrderId { get; set; }

        /// <summary>
        /// 物流公司名称
        /// </summary>
        public string ShippingCompany { get; set; } = string.Empty;

        /// <summary>
        /// 物流单号
        /// </summary>
        public string TrackingNumber { get; set; } = string.Empty;

        /// <summary>
        /// 物流状态
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// 发货时间
        /// </summary>
        public DateTime? ShippedTime { get; set; }

        /// <summary>
        /// 预计送达时间
        /// </summary>
        public DateTime? EstimatedDeliveryTime { get; set; }
    }
}

