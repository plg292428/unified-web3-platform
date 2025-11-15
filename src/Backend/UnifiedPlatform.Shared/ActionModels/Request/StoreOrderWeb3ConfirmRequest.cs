using System;
using System.ComponentModel.DataAnnotations;
using UnifiedPlatform.Shared.Enums;

namespace UnifiedPlatform.Shared.ActionModels.Request
{
    public class StoreOrderWeb3ConfirmRequest
    {
        [Range(1, int.MaxValue)]
        public int Uid { get; set; }

        [MaxLength(128)]
        public string? PaymentTransactionHash { get; set; }

        public StorePaymentStatus PaymentStatus { get; set; } = StorePaymentStatus.AwaitingOnChainConfirmation;

        public int? PaymentConfirmations { get; set; }

        public DateTime? PaymentConfirmedTime { get; set; }

        public string? PaymentSignatureResult { get; set; }

        public string? PaymentFailureReason { get; set; }

        public string? RawData { get; set; }
    }
}
