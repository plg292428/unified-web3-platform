using System;
using UnifiedPlatform.Shared.Enums;

namespace UnifiedPlatform.Shared.ActionModels.Result
{
    public class StoreOrderPaymentLogResult
    {
        public long OrderPaymentLogId { get; set; }

        public StorePaymentStatus PaymentStatus { get; set; }

        public string EventType { get; set; } = string.Empty;

        public string? Message { get; set; }

        public string? RawData { get; set; }

        public DateTime CreateTime { get; set; }
    }
}
