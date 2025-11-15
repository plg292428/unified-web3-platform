using System.ComponentModel.DataAnnotations;
using UnifiedPlatform.Shared.Enums;

namespace UnifiedPlatform.Shared.ActionModels.Request
{
    public class StoreOrderListRequest
    {
        [Range(1, int.MaxValue)]
        public int Uid { get; set; }

        public StoreOrderStatus? Status { get; set; }

        public StorePaymentStatus? PaymentStatus { get; set; }

        public StorePaymentMode? PaymentMode { get; set; }

        [StringLength(32)]
        public string? OrderNumber { get; set; }
    }
}
