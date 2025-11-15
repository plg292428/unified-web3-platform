namespace UnifiedPlatform.Shared.ActionModels.Request
{
    /// <summary>
    /// 创建物流信息请求
    /// </summary>
    public class ShippingCreateRequest
    {
        /// <summary>
        /// 订单ID
        /// </summary>
        public long OrderId { get; set; }

        /// <summary>
        /// 物流公司名称
        /// </summary>
        public string ShippingCompany { get; set; } = string.Empty;

        /// <summary>
        /// 物流公司代码
        /// </summary>
        public string? ShippingCompanyCode { get; set; }

        /// <summary>
        /// 物流单号
        /// </summary>
        public string TrackingNumber { get; set; } = string.Empty;

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
        /// 预计送达时间
        /// </summary>
        public DateTime? EstimatedDeliveryTime { get; set; }

        /// <summary>
        /// 物流费用
        /// </summary>
        public decimal? ShippingFee { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string? Remark { get; set; }
    }
}

