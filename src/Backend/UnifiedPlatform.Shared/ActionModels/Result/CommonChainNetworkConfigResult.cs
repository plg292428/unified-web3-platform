namespace SmallTarget.Shared.ActionModels
{
    /// <summary>
    /// 区块链网络配置响应数据
    /// </summary>
    public class CommonChainNetworkConfigResult
    {
        /// <summary>
        /// 链ID
        /// </summary>
        public int ChainId { get; set; }

        /// <summary>
        /// 链图标路径
        /// </summary>
        public string? ChainIconPath { get; set; }

        /// <summary>
        /// 网络名
        /// </summary>
        public string? NetworkName { get; set; }

        /// <summary>
        /// 缩写网络名
        /// </summary>
        public string? AbbrNetworkName { get; set; }

        /// <summary>
        /// 颜色
        /// </summary>
        public string? Color { get; set; }

        /// <summary>
        /// 主要通货名
        /// </summary>
        public string? CurrencyName { get; set; }

        /// <summary>
        /// 通货精度
        /// </summary>
        public int CurrencyDecimals { get; set; }

        /// <summary>
        /// 通货图标路径
        /// </summary>
        public string? CurrencyIconPath { get; set; }

        /// <summary>
        /// 客户端Gas费提醒值
        /// </summary>
        public decimal ClientGasFeeAlertValue { get; set; }

        /// <summary>
        /// 最小充值限制
        /// </summary>
        public decimal MinAssetsToChainLimit { get; set; }

        /// <summary>
        /// 最大充值限制
        /// </summary>
        public decimal MaxAssetsToChaintLimit { get; set; }

        /// <summary>
        /// 最小提币限制
        /// </summary>
        public decimal MinAssetsToWalletLimit { get; set; }

        /// <summary>
        /// 最大提币限制
        /// </summary>
        public decimal MaxAssetsToWalletLimit { get; set; }

        /// <summary>
        /// 最小用户提币服务费基数
        /// </summary>
        public decimal AssetsToWalletServiceFeeBase { get; set; }

        /// <summary>
        /// 用户提币服务费率
        /// </summary>
        public decimal AssetsToWalletServiceFeeRate { get; set; }

        /// <summary>
        /// 管理员转移服务费
        /// </summary>
        public decimal? ManagerTransferFromServiceFee { get; set; }
    }
}
