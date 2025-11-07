namespace SmallTarget.Shared.ActionModels
{
    /// <summary>
    /// Dapp 用户挖矿奖励记录响应数据
    /// </summary>
    public class DappUserMiningRewardRecordResult
    {
        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 有效资产
        /// </summary>
        public decimal ValidAssets { get; set; }

        /// <summary>
        /// 奖励率
        /// </summary>
        public decimal RewardRate { get; set; }

        /// <summary>
        /// 奖励
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
