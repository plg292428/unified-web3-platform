using UnifiedPlatform.Shared;

namespace UnifiedPlatform.WebApi.Services
{
    /// <summary>
    /// Web3 提供方索引
    /// </summary>
    public struct Web3ProviderIndex
    {
        /// <summary>
        /// 组ID
        /// </summary>
        public int GroupId { get; set; }

        /// <summary>
        /// 链网络
        /// </summary>
        public ChainNetwork ChainNetwork { get; set; }
    }
}

