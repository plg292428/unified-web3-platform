namespace UnifiedPlatform.Shared.ActionModels
{
    /// <summary>
    /// Dapp 用户系统短消息详情响应数据
    /// </summary>
    public class DappUserSystemMessageDetailsResult : DappUserSystemMessageResult
    {
        /// <summary>
        /// 激活码
        /// </summary>
        public string? ActivationCodeGuid { get; set; }

        /// <summary>
        /// 激活码过期时间
        /// </summary>
        public DateTime? ActivationCodeExpirationTime { get; set; }
    }
}

