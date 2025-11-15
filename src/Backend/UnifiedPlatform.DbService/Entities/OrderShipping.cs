using System;

namespace UnifiedPlatform.DbService.Entities
{
    /// <summary>
    /// 订单物流信息
    /// </summary>
    public partial class OrderShipping
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
        /// 物流公司代码（用于API查询）
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
        /// 物流状态（pending, shipped, in_transit, delivered, exception）
        /// </summary>
        public string Status { get; set; } = "pending";

        /// <summary>
        /// 物流状态描述
        /// </summary>
        public string? StatusDescription { get; set; }

        /// <summary>
        /// 物流费用
        /// </summary>
        public decimal ShippingFee { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string? Remark { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; }

        public virtual Order Order { get; set; } = null!;

        public virtual ICollection<ShippingTrackingLog> TrackingLogs { get; set; } = new List<ShippingTrackingLog>();
    }
}

