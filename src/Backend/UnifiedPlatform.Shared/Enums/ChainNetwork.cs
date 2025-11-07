using System.ComponentModel;

namespace SmallTarget.Shared
{
    /// <summary>
    /// 区块链网络
    /// </summary>
    public enum ChainNetwork
    {
        /// <summary>
        /// 无效的
        /// </summary>
        [Description("Invalid")]
        Invalid = 0,

        /// <summary>
        /// 以太坊
        /// </summary>
        [Description("Ethereum")]
        Ethereum = 1,

        /// <summary>
        /// 币安智能链
        /// </summary>
        [Description("Binance Smart Chain")]
        BinanceSmartChain = 56,

        /// <summary>
        /// 多边形
        /// </summary>
        [Description("Polygon")]
        Polygon = 137,

        /// <summary>
        /// 多边形
        /// </summary>
        [Description("Arbitrum One")]
        Arbitrum = 42161,
    }
}
