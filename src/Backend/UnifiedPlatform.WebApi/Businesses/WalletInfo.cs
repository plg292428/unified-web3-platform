using HFastKit.AspNetCore.Shared.Text;
using UnifiedPlatform.Shared;

namespace UnifiedPlatform.WebApi
{
    /// <summary>
    /// 钱包信息
    /// </summary>
    public struct WalletInfo
    {
        /// <summary>
        /// 区块链网络ID
        /// </summary>
        public ChainNetwork ChainId { get; set; }

        /// <summary>
        /// 钱包地址
        /// </summary>
        public string WalletAddress { get; set; }

        /// <summary>
        /// 钱包信息
        /// </summary>
        /// <param name="chainId">区块链网络</param>
        /// <param name="walletAddress">钱包地址</param>
        public WalletInfo(ChainNetwork chainId, string walletAddress)
        {
            if (!FormatValidate.IsEthereumAddress(walletAddress))
            {
               throw new ArgumentException(nameof(walletAddress));
            }
            WalletAddress = walletAddress.ToLower();
            ChainId = chainId;
        }
    }
}

