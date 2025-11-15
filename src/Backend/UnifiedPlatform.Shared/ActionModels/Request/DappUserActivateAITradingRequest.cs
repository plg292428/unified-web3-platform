namespace UnifiedPlatform.Shared.ActionModels
{
    /// <summary>
    /// Dapp 用户激活 AI 合约交易请求
    /// </summary>
    public class DappUserActivateAITradingRequest
    {
        /// <summary>
        /// 激活码
        /// </summary>
        public required string ActivationCode { get; set; }
    }
}

