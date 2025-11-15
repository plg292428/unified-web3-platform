namespace UnifiedPlatform.Shared.ActionModels
{
    /// <summary>
    /// 区块链代币配置响应数据
    /// </summary>
    public class CommonChainTokenConfigResult
    {
        /// <summary>
        /// 代币ID
        /// </summary>
        public int TokenId { get; set; }

        /// <summary>
        /// 链ID
        /// </summary>
        public int ChainId { get; set; }

        /// <summary>
        /// 代币名称
        /// </summary>
        public string? TokenName { get; set; }

        /// <summary>
        /// 缩写代币名称
        /// </summary>
        public string? AbbrTokenName { get; set; }

        /// <summary>
        /// 图标路径
        /// </summary>
        public string? IconPath { get; set; }

        /// <summary>
        /// 合约地址
        /// </summary>
        public string? ContractAddress { get; set; }

        /// <summary>
        /// 授权ABI方法名称
        /// </summary>
        public string? ApproveAbiFunctionName { get; set; }

        /// <summary>
        /// 小数精度
        /// </summary>
        public int Decimals { get; set;}
    }
}

