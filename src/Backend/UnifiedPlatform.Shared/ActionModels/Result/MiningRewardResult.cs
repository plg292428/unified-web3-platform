namespace UnifiedPlatform.Shared.ActionModels.Result
{
    /// <summary>
    /// 挖矿奖励结果
    /// </summary>
    public class MiningRewardResult
    {
        /// <summary>
        /// 记录ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 有效资产
        /// </summary>
        public decimal ValidAssets { get; set; }

        /// <summary>
        /// 奖励比例
        /// </summary>
        public decimal RewardRate { get; set; }

        /// <summary>
        /// 奖励金额
        /// </summary>
        public decimal Reward { get; set; }

        /// <summary>
        /// 加速模式
        /// </summary>
        public bool SpeedUpMode { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string? Comment { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}

