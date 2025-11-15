using System.Collections.Generic;

namespace UnifiedPlatform.Shared.ActionModels.Result
{
    /// <summary>
    /// 准备 Web3 支付签名响应。
    /// </summary>
    public class StoreOrderPreparePaymentResult
    {
        /// <summary>
        /// 支付签名原文（用于钱包签名）。
        /// </summary>
        public string PaymentSignaturePayload { get; set; } = string.Empty;

        /// <summary>
        /// 支付金额。
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 币种。
        /// </summary>
        public string Currency { get; set; } = "USDT";

        /// <summary>
        /// 链ID。
        /// </summary>
        public int ChainId { get; set; }

        /// <summary>
        /// 链名称。
        /// </summary>
        public string? ChainName { get; set; }

        /// <summary>
        /// 代币ID（如果使用代币支付）。
        /// </summary>
        public int? TokenId { get; set; }

        /// <summary>
        /// 代币合约地址（如果使用代币支付）。
        /// </summary>
        public string? TokenContractAddress { get; set; }

        /// <summary>
        /// 代币符号。
        /// </summary>
        public string? TokenSymbol { get; set; }

        /// <summary>
        /// 收款地址。
        /// </summary>
        public string PaymentAddress { get; set; } = string.Empty;

        /// <summary>
        /// 支付过期时间（Unix 时间戳）。
        /// </summary>
        public long PaymentExpiresAt { get; set; }

        /// <summary>
        /// 订单编号。
        /// </summary>
        public string OrderNumber { get; set; } = string.Empty;

        /// <summary>
        /// 支持的代币列表。
        /// </summary>
        public List<PaymentTokenInfo>? SupportedTokens { get; set; }
    }

    /// <summary>
    /// 支付代币信息
    /// </summary>
    public class PaymentTokenInfo
    {
        /// <summary>
        /// 代币ID
        /// </summary>
        public int TokenId { get; set; }

        /// <summary>
        /// 代币名称
        /// </summary>
        public string TokenName { get; set; } = string.Empty;

        /// <summary>
        /// 代币符号
        /// </summary>
        public string TokenSymbol { get; set; } = string.Empty;

        /// <summary>
        /// 图标路径
        /// </summary>
        public string? IconPath { get; set; }

        /// <summary>
        /// 精度
        /// </summary>
        public byte Decimals { get; set; }

        /// <summary>
        /// 合约地址
        /// </summary>
        public string? ContractAddress { get; set; }
    }
}

