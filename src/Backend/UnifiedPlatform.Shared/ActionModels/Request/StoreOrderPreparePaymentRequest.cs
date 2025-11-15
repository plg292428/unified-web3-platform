using System.ComponentModel.DataAnnotations;

namespace UnifiedPlatform.Shared.ActionModels.Request
{
    /// <summary>
    /// 准备 Web3 支付签名请求。
    /// </summary>
    public class StoreOrderPreparePaymentRequest
    {
        [Range(1, int.MaxValue)]
        public int Uid { get; set; }

        [MaxLength(128)]
        public string? PaymentWalletAddress { get; set; }

        [MaxLength(64)]
        public string? PaymentProviderType { get; set; }

        [MaxLength(64)]
        public string? PaymentProviderName { get; set; }

        [Range(1, int.MaxValue)]
        public int? ChainId { get; set; }

        /// <summary>
        /// 代币ID（如果使用代币支付，而不是原生币）
        /// </summary>
        [Range(1, int.MaxValue)]
        public int? TokenId { get; set; }
    }
}

