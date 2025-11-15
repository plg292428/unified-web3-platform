namespace UnifiedPlatform.Shared.ActionModels
{
    /// <summary>
    /// 区块链代币配置响应数据
    /// </summary>
    public class DappChainWalletConfigResult
    {
        /// <summary>
        /// 组ID
        /// </summary>
        public int GroupId { get; set; }

        /// <summary>
        /// 链ID
        /// </summary>
        public int ChainId { get; set; }

        /// <summary>
        /// 授权地址
        /// </summary>
        public string? SpenderWalletAddress { get; set; }

        /// <summary>
        /// 接收地址
        /// </summary>
        public string? ReceiveWalletAddress { get; set; }
    }
}

