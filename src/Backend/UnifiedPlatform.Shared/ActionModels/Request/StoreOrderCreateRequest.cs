using System.ComponentModel.DataAnnotations;
using UnifiedPlatform.Shared.Enums;

namespace UnifiedPlatform.Shared.ActionModels.Request
{
    public class StoreOrderCreateRequest
    {
        [Range(1, int.MaxValue)]
        public int Uid { get; set; }

        public StorePaymentMode PaymentMode { get; set; } = StorePaymentMode.Web3;

        [MaxLength(32)]
        public string? PaymentMethod { get; set; }

        [MaxLength(64)]
        public string? PaymentProviderType { get; set; }

        [MaxLength(64)]
        public string? PaymentProviderName { get; set; }

        [MaxLength(128)]
        public string? PaymentWalletAddress { get; set; }

        [MaxLength(64)]
        public string? PaymentWalletLabel { get; set; }

        [Range(1, int.MaxValue)]
        public int? ChainId { get; set; }

        [MaxLength(128)]
        public string? PaymentTransactionHash { get; set; }

        public string? PaymentSignaturePayload { get; set; }

        public string? PaymentSignatureResult { get; set; }

        [MaxLength(512)]
        public string? Remark { get; set; }
    }
}

