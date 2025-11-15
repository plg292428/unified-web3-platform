using System;
using UnifiedPlatform.Shared.Enums;

namespace UnifiedPlatform.Shared.ActionModels.Result
{
    public class StoreOrderSummaryResult
    {
        public long OrderId { get; set; }

        public string OrderNumber { get; set; } = string.Empty;

        public decimal TotalAmount { get; set; }

        public string Currency { get; set; } = "USDT";

        public StoreOrderStatus Status { get; set; } = StoreOrderStatus.PendingPayment;

        public StorePaymentMode PaymentMode { get; set; } = StorePaymentMode.Web3;

        public StorePaymentStatus PaymentStatus { get; set; } = StorePaymentStatus.PendingSignature;

        public string? PaymentMethod { get; set; }

        public string? PaymentProviderType { get; set; }

        public string? PaymentProviderName { get; set; }

        public string? PaymentWalletAddress { get; set; }

        public int? ChainId { get; set; }

        public string? PaymentTransactionHash { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime? PaidTime { get; set; }
    }
}

