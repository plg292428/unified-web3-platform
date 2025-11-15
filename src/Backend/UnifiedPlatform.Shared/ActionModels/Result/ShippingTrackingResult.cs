namespace UnifiedPlatform.Shared.ActionModels.Result
{
    /// <summary>
    /// 物流跟踪结果
    /// </summary>
    public class ShippingTrackingResult
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
        /// 状态描述
        /// </summary>
        public string? StatusDescription { get; set; }

        /// <summary>
        /// 收货人姓名
        /// </summary>
        public string RecipientName { get; set; } = string.Empty;

        /// <summary>
        /// 收货人电话
        /// </summary>
        public string RecipientPhone { get; set; } = string.Empty;

        /// <summary>
        /// 收货地址
        /// </summary>
        public string RecipientAddress { get; set; } = string.Empty;

        /// <summary>
        /// 发货时间
        /// </summary>
        public DateTime? ShippedTime { get; set; }

        /// <summary>
        /// 预计送达时间
        /// </summary>
        public DateTime? EstimatedDeliveryTime { get; set; }

        /// <summary>
        /// 实际送达时间
        /// </summary>
        public DateTime? DeliveredTime { get; set; }

        /// <summary>
        /// 跟踪记录列表
        /// </summary>
        public List<ShippingTrackingLogResult> TrackingLogs { get; set; } = new();
    }

    /// <summary>
    /// 物流跟踪记录结果
    /// </summary>
    public class ShippingTrackingLogResult
    {
        /// <summary>
        /// 状态
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// 位置
        /// </summary>
        public string? Location { get; set; }

        /// <summary>
        /// 时间戳
        /// </summary>
        public DateTime Timestamp { get; set; }
    }

    /// <summary>
    /// 物流公司结果
    /// </summary>
    public class ShippingCompanyResult
    {
        /// <summary>
        /// 公司代码
        /// </summary>
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// 公司名称
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 是否支持API查询
        /// </summary>
        public bool ApiSupported { get; set; }
    }
}

