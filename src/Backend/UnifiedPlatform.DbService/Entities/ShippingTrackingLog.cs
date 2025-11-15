using System;

namespace UnifiedPlatform.DbService.Entities
{
    /// <summary>
    /// 物流跟踪记录
    /// </summary>
    public partial class ShippingTrackingLog
    {
        /// <summary>
        /// 跟踪记录ID
        /// </summary>
        public long TrackingLogId { get; set; }

        /// <summary>
        /// 物流ID
        /// </summary>
        public long ShippingId { get; set; }

        /// <summary>
        /// 跟踪状态
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// 状态描述
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// 位置信息
        /// </summary>
        public string? Location { get; set; }

        /// <summary>
        /// 时间戳
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        public virtual OrderShipping Shipping { get; set; } = null!;
    }
}

