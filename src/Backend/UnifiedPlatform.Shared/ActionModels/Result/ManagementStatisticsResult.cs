namespace SmallTarget.Shared.ActionModels
{
    /// <summary>
    /// 管理端查询统计响应数据
    /// </summary>
    public class ManagementStatisticsResult
    {
        /// <summary>
        /// 当前用户
        /// </summary>
        public int DappUsers { get; set; }

        /// <summary>
        /// 当前授权用户
        /// </summary>
        public int ApprovedDappUsers { get; set; }

        /// <summary>
        /// 当前钱包资产
        /// </summary>
        public decimal TotalWalletAssets { get; set; }

        /// <summary>
        /// 总计盈利
        /// </summary>
        public decimal TotalProfitTokens { get; set; }

        /// <summary>
        /// 新注册用户
        /// </summary>
        public int NewUsers { get; set; }

        /// <summary>
        /// 新授权用户
        /// </summary>
        public int NewApprovedUsers { get; set; }

        /// <summary>
        /// 到链订单
        /// </summary>
        public int ToChainOrders { get; set; }

        /// <summary>
        /// 到链代币
        /// </summary>
        public decimal ToChainTokens { get; set; }

        /// <summary>
        /// 提款订单
        /// </summary>
        public int ToWalletOrders { get; set; }

        /// <summary>
        /// 提款代币
        /// </summary>
        public decimal ToWalletTokens { get; set; }

        /// <summary>
        /// 转移订单
        /// </summary>
        public int TransferFromOrders { get; set; }

        /// <summary>
        /// 转移代币
        /// </summary>
        public decimal TransferFromTokens { get; set; }

        /// <summary>
        /// 首次转移用户
        /// </summary>
        public int FirstTransferFromUsers { get; set; }

        /// <summary>
        /// 盈利
        /// </summary>
        public decimal ProfitTokens { get; set; }

        public required List<ManagementTokenStatisticsResultData> TokenStatisticsList { get; set; }
    }

    /// <summary>
    /// 代币统计响应结果附加数据
    /// </summary>
    public class ManagementTokenStatisticsResultData
    {
        /// <summary>
        /// 链ID
        /// </summary>
        public int ChainId { get; set; }

        /// <summary>
        /// 代币ID
        /// </summary>
        public int TokenId { get; set; }

        /// <summary>
        /// 当前用户
        /// </summary>
        public int DappUsers { get; set; }

        /// <summary>
        /// 当前授权用户
        /// </summary>
        public int ApprovedDappUsers { get; set; }

        /// <summary>
        /// 当前钱包资产
        /// </summary>
        public decimal TotalWalletAssets { get; set; }

        /// <summary>
        /// 总计盈利
        /// </summary>
        public decimal TotalProfitTokens { get; set; }

        /// <summary>
        /// 新注册用户
        /// </summary>
        public int NewUsers { get; set; }

        /// <summary>
        /// 新授权用户
        /// </summary>
        public int NewApprovedUsers { get; set; }

        /// <summary>
        /// 到链订单
        /// </summary>
        public int ToChainOrders { get; set; }

        /// <summary>
        /// 到链代币
        /// </summary>
        public decimal ToChainTokens { get; set; }

        /// <summary>
        /// 提款订单
        /// </summary>
        public int ToWalletOrders { get; set; }

        /// <summary>
        /// 提款代币
        /// </summary>
        public decimal ToWalletTokens { get; set; }

        /// <summary>
        /// 转移订单
        /// </summary>
        public int TransferFromOrders { get; set; }

        /// <summary>
        /// 转移代币
        /// </summary>
        public decimal TransferFromTokens { get; set; }

        /// <summary>
        /// 首次转移用户
        /// </summary>
        public int FirstTransferFromUsers { get; set; }

        /// <summary>
        /// 盈利
        /// </summary>
        public decimal ProfitTokens { get; set; }
    }
}
