namespace UnifiedPlatform.Shared.ActionModels
{
    /// <summary>
    /// 区块链代币配置响应数据
    /// </summary>
    public class DappCustomerServiceConfigResultData
    {
        /// <summary>
        /// 是否开启客户客服
        /// </summary>
        public bool CustomerServiceEnabled { get; set; }

        /// <summary>
        /// ChatWoot Key
        /// </summary>
        public string? CustomerServiceChatWootKey { get; set; }
    }
}

