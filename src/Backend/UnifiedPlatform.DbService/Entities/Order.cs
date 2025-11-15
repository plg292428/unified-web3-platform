using System;
using System.Collections.Generic;
using UnifiedPlatform.Shared.Enums;

namespace UnifiedPlatform.DbService.Entities
{
    public partial class Order
    {
        public long OrderId { get; set; }

        public string OrderNumber { get; set; } = null!;

        public int Uid { get; set; }

        public decimal TotalAmount { get; set; }

        public string Currency { get; set; } = "USDT";

        public StoreOrderStatus Status { get; set; } = StoreOrderStatus.PendingPayment;

        public StorePaymentMode PaymentMode { get; set; } = StorePaymentMode.Traditional;

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

        public DateTime? PaymentExpiresAt { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime? PaidTime { get; set; }

        public DateTime? CancelTime { get; set; }

        public DateTime? CompleteTime { get; set; }

        public string? Remark { get; set; }

        public virtual ChainNetworkConfig? Chain { get; set; }

        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        public virtual ICollection<OrderPaymentLog> PaymentLogs { get; set; } = new List<OrderPaymentLog>();

        public virtual ICollection<OrderShipping> Shippings { get; set; } = new List<OrderShipping>();

        public virtual ICollection<ProductReview> ProductReviews { get; set; } = new List<ProductReview>();

        public virtual User UidNavigation { get; set; } = null!;
    }
}

