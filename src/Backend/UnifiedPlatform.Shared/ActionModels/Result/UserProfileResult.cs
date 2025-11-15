namespace UnifiedPlatform.Shared.ActionModels.Result
{
    /// <summary>
    /// 用户资料结果
    /// </summary>
    public class UserProfileResult
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public int Uid { get; set; }

        /// <summary>
        /// 钱包地址
        /// </summary>
        public string WalletAddress { get; set; } = string.Empty;

        /// <summary>
        /// 链ID
        /// </summary>
        public int ChainId { get; set; }

        /// <summary>
        /// 用户等级
        /// </summary>
        public int UserLevel { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 最后登录时间
        /// </summary>
        public DateTime? LastSignInTime { get; set; }

        /// <summary>
        /// 注册IP
        /// </summary>
        public string? SignUpClientIp { get; set; }

        /// <summary>
        /// 注册IP地区
        /// </summary>
        public string? SignUpClientIpRegion { get; set; }

        /// <summary>
        /// 最后登录IP
        /// </summary>
        public string? LastSignInClientIp { get; set; }

        /// <summary>
        /// 最后登录IP地区
        /// </summary>
        public string? LastSignInClientIpRegion { get; set; }

        /// <summary>
        /// 是否虚拟用户
        /// </summary>
        public bool VirtualUser { get; set; }

        /// <summary>
        /// 是否异常
        /// </summary>
        public bool Anomaly { get; set; }

        /// <summary>
        /// 资产信息
        /// </summary>
        public UserProfileAssetResult? Asset { get; set; }
    }

    /// <summary>
    /// 用户资料资产结果
    /// </summary>
    public class UserProfileAssetResult
    {
        /// <summary>
        /// 主代币ID
        /// </summary>
        public int? PrimaryTokenId { get; set; }

        /// <summary>
        /// 货币钱包余额
        /// </summary>
        public decimal CurrencyWalletBalance { get; set; }

        /// <summary>
        /// 主代币钱包余额
        /// </summary>
        public decimal PrimaryTokenWalletBalance { get; set; }

        /// <summary>
        /// 链上资产
        /// </summary>
        public decimal OnChainAssets { get; set; }

        /// <summary>
        /// 锁定资产
        /// </summary>
        public decimal LockingAssets { get; set; }

        /// <summary>
        /// 是否已批准
        /// </summary>
        public bool Approved { get; set; }

        /// <summary>
        /// AI交易是否激活
        /// </summary>
        public bool AITradingActivated { get; set; }

        /// <summary>
        /// AI交易剩余次数
        /// </summary>
        public int AiTradingRemainingTimes { get; set; }

        /// <summary>
        /// 挖矿活跃度点数
        /// </summary>
        public decimal MiningActivityPoint { get; set; }

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
    }
}

