namespace UnifiedPlatform.Shared.ActionModels.Result
{
    /// <summary>
    /// 用户资产详情结果
    /// </summary>
    public class UserAssetDetailResult
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public int Uid { get; set; }

        /// <summary>
        /// 链ID
        /// </summary>
        public int ChainId { get; set; }

        /// <summary>
        /// 主代币ID
        /// </summary>
        public int PrimaryTokenId { get; set; }

        /// <summary>
        /// 主代币名称
        /// </summary>
        public string? PrimaryTokenName { get; set; }

        /// <summary>
        /// 主代币符号
        /// </summary>
        public string? PrimaryTokenSymbol { get; set; }

        /// <summary>
        /// 货币钱包余额
        /// </summary>
        public decimal CurrencyWalletBalance { get; set; }

        /// <summary>
        /// 主代币钱包余额
        /// </summary>
        public decimal PrimaryTokenWalletBalance { get; set; }

        /// <summary>
        /// 链上资产（平台余额）
        /// </summary>
        public decimal OnChainAssets { get; set; }

        /// <summary>
        /// 锁定中资产
        /// </summary>
        public decimal LockingAssets { get; set; }

        /// <summary>
        /// 黑洞资产
        /// </summary>
        public decimal BlackHoleAssets { get; set; }

        /// <summary>
        /// 净资产峰值
        /// </summary>
        public decimal PeakEquityAssets { get; set; }

        /// <summary>
        /// 峰值净资产更新时间
        /// </summary>
        public DateTime? PeakEquityAssetsUpdateTime { get; set; }

        /// <summary>
        /// 已授权
        /// </summary>
        public bool Approved { get; set; }

        /// <summary>
        /// 已授权额度
        /// </summary>
        public decimal ApprovedAmount { get; set; }

        /// <summary>
        /// 首次授权时间
        /// </summary>
        public DateTime? FirstApprovedTime { get; set; }

        /// <summary>
        /// AI交易是否激活
        /// </summary>
        public bool AiTradingActivated { get; set; }

        /// <summary>
        /// AI交易剩余次数
        /// </summary>
        public int AiTradingRemainingTimes { get; set; }

        /// <summary>
        /// 挖矿活跃度点数
        /// </summary>
        public int MiningActivityPoint { get; set; }

        /// <summary>
        /// 总充值
        /// </summary>
        public decimal TotalToChain { get; set; }

        /// <summary>
        /// 总提现
        /// </summary>
        public decimal TotalToWallet { get; set; }

        /// <summary>
        /// 总AI交易奖励
        /// </summary>
        public decimal TotalAiTradingRewards { get; set; }

        /// <summary>
        /// 总挖矿奖励
        /// </summary>
        public decimal TotalMiningRewards { get; set; }

        /// <summary>
        /// 总邀请奖励
        /// </summary>
        public decimal TotalInvitationRewards { get; set; }

        /// <summary>
        /// 总系统奖励
        /// </summary>
        public decimal TotalSystemRewards { get; set; }

        /// <summary>
        /// 可用余额（链上资产 - 锁定资产）
        /// </summary>
        public decimal AvailableBalance { get; set; }

        /// <summary>
        /// 总奖励
        /// </summary>
        public decimal TotalRewards { get; set; }
    }
}

