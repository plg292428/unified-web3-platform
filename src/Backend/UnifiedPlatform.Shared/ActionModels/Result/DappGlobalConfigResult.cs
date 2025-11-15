namespace UnifiedPlatform.Shared.ActionModels
{
    /// <summary>
    /// Dapp 全局配置响应数据
    /// </summary>
    public class DappGlobalConfigResult
    {
        /// <summary>
        /// 挖矿奖励间隔时间（小时）
        /// </summary>
        public int MiningRewardIntervalHours { get; set; }

        /// <summary>
        /// 加速挖矿需要的链上资产比例
        /// </summary>
        public decimal MiningSpeedUpRequiredOnChainAssetsRate { get; set; }

        /// <summary>
        /// 加速挖矿提高的奖励率
        /// </summary>

        public decimal MiningSpeedUpRewardIncreaseRate { get; set; }

        /// <summary>
        /// AI合约交易最低需要时间（分钟）
        /// </summary>
        public int MinAiTradingMinutes { get; set; }

        /// <summary>
        /// AI合约交易最高需要时间（分钟）
        /// </summary>
        public int MaxAiTradingMinutes { get; set; }

        /// <summary>
        /// 1层邀请奖励比例
        /// </summary>
        public decimal InvitedRewardRateLayer1 { get; set; }

        /// <summary>
        /// 2层邀请奖励比例
        /// </summary>
        public decimal InvitedRewardRateLayer2 { get; set; }

        /// <summary>
        /// 3层邀请奖励比例
        /// </summary>
        public decimal InvitedRewardRateLayer3 { get; set; }
    }
}

