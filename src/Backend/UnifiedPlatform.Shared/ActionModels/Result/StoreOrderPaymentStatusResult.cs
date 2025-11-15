using System;
using UnifiedPlatform.Shared.Enums;

namespace UnifiedPlatform.Shared.ActionModels.Result
{
    /// <summary>
    /// 支付状态查询响应。
    /// </summary>
    public class StoreOrderPaymentStatusResult
    {
        public long OrderId { get; set; }

        public string OrderNumber { get; set; } = string.Empty;

        public StorePaymentStatus PaymentStatus { get; set; }

        public StoreOrderStatus OrderStatus { get; set; }

        public string? PaymentTransactionHash { get; set; }

        public int? PaymentConfirmations { get; set; }

        public DateTime? PaymentSubmittedTime { get; set; }

        public DateTime? PaymentConfirmedTime { get; set; }

        public DateTime? PaymentExpiresAt { get; set; }

        public string? PaymentFailureReason { get; set; }

        /// <summary>
        /// 是否已过期。
        /// </summary>
        public bool IsExpired { get; set; }

        /// <summary>
        /// 剩余有效时间（秒），如果已过期则为 0。
        /// </summary>
        public long RemainingSeconds { get; set; }
    }
}

