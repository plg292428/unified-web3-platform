namespace SmallTarget.Shared.ActionModels
{
    /// <summary>
    /// Dapp 用户财产响应数据
    /// </summary>
    public class DappUserAssetsResult
    {
        /// <summary>
        /// 主要代币ID
        /// </summary>
        public int PrimaryTokenId { get; set; }

        /// <summary>
        /// 通货钱包余额
        /// </summary>
        public decimal CurrencyWalletBalance { get; set; }

        /// <summary>
        /// 主要代币钱包余额
        /// </summary>
        public decimal PrimaryTokenWalletBalance { get; set; }

        /// <summary>
        /// 链上资产（平台余额）
        /// </summary>
        public decimal OnChainAssets { get; set; }

        /// <summary>
        /// 有效资产
        /// </summary>
        public decimal ValidAssets => OnChainAssets + PrimaryTokenWalletBalance;

        /// <summary>
        /// 已授权
        /// </summary>
        public bool Approved { get; set; }

        /// <summary>
        /// 已激活AI合约交易功能
        /// </summary>
        public bool AITradingActivated { get; set; }


        /// <summary>
        /// 剩余AI交易次数
        /// </summary>
        public int AiTradingRemainingTimes { get; set; }


        /// <summary>
        /// 剩余挖矿活跃值
        /// </summary>
        public int MiningActivityPoint { get; set; }


        /// <summary>
        /// 合计转入链上（仅用于统计）
        /// </summary>
        public decimal TotalToChain { get; set; }


        /// <summary>
        /// 合计转出钱包（仅用于统计）
        /// </summary>
        public decimal TotalToWallet { get; set; }


        /// <summary>
        /// 合计AI合约交易奖励
        /// </summary>
        public decimal TotalAiTradingRewards { get; set; }

        /// <summary>
        /// 合计挖矿奖励（仅用于统计）
        /// </summary>
        public decimal TotalMiningRewards { get; set; }


        /// <summary>
        /// 合计邀请奖励（仅用于统计）
        /// </summary>
        public decimal TotalInvitationRewards { get; set; }


        /// <summary>
        /// 合计系统奖励（仅用于统计）
        /// </summary>
        public decimal TotalSystemRewards { get; set; }
    }
}
