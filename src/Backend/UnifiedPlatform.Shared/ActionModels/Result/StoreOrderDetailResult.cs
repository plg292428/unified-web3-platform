using System;
using System.Collections.Generic;
using UnifiedPlatform.Shared.Enums;

namespace UnifiedPlatform.Shared.ActionModels.Result
{
    public class StoreOrderDetailResult
    {
        public long OrderId { get; set; }

        public string OrderNumber { get; set; } = string.Empty;

        public int Uid { get; set; }

        public decimal TotalAmount { get; set; }

        public string Currency { get; set; } = "USDT";

        public StoreOrderStatus Status { get; set; } = StoreOrderStatus.PendingPayment;

        public StorePaymentMode PaymentMode { get; set; } = StorePaymentMode.Web3;

        public StorePaymentStatus PaymentStatus { get; set; } = StorePaymentStatus.PendingSignature;

        public string? PaymentMethod { get; set; }

        public string? PaymentProviderType { get; set; }

        public string? PaymentProviderName { get; set; }

        public string? PaymentWalletAddress { get; set; }

        public string? PaymentWalletLabel { get; set; }

        public int? ChainId { get; set; }

        public string? PaymentTransactionHash { get; set; }

        public int? PaymentConfirmations { get; set; }

        public DateTime? PaymentSubmittedTime { get; set; }

        public DateTime? PaymentConfirmedTime { get; set; }

        public string? PaymentSignaturePayload { get; set; }

        public string? PaymentSignatureResult { get; set; }

        public string? PaymentFailureReason { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime? PaidTime { get; set; }

        public DateTime? CancelTime { get; set; }

        public DateTime? CompleteTime { get; set; }

        public string? Remark { get; set; }

        public IReadOnlyList<StoreOrderItemResult> Items { get; set; } = Array.Empty<StoreOrderItemResult>();

        public IReadOnlyList<StoreOrderPaymentLogResult> PaymentLogs { get; set; } = Array.Empty<StoreOrderPaymentLogResult>();
    }

    public class StoreOrderItemResult
    {
        public long OrderItemId { get; set; }

        public long ProductId { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public decimal UnitPrice { get; set; }

        public int Quantity { get; set; }

        public decimal Subtotal { get; set; }
    }
}

