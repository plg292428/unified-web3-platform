using System;
using UnifiedPlatform.Shared.Enums;

namespace UnifiedPlatform.DbService.Entities
{
    public partial class OrderPaymentLog
    {
        public long OrderPaymentLogId { get; set; }

        public long OrderId { get; set; }

        public StorePaymentStatus PaymentStatus { get; set; }

        public string EventType { get; set; } = "status_change";

        public string? Message { get; set; }

        public string? RawData { get; set; }

        public DateTime CreateTime { get; set; }

        public virtual Order Order { get; set; } = null!;
    }
}
