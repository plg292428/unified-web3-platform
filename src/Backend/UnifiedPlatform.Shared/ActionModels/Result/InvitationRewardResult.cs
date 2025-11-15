namespace UnifiedPlatform.Shared.ActionModels.Result
{
    /// <summary>
    /// 邀请奖励结果
    /// </summary>
    public class InvitationRewardResult
    {
        /// <summary>
        /// 记录ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 下级用户ID
        /// </summary>
        public int SubUserUid { get; set; }

        /// <summary>
        /// 下级用户钱包地址
        /// </summary>
        public string SubUserWalletAddress { get; set; } = string.Empty;

        /// <summary>
        /// 下级用户层级
        /// </summary>
        public int SubUserLayer { get; set; }

        /// <summary>
        /// 下级用户奖励
        /// </summary>
        public decimal SubUserReward { get; set; }

        /// <summary>
        /// 下级用户奖励类型
        /// </summary>
        public int SubUserRewardType { get; set; }

        /// <summary>
        /// 奖励比例
        /// </summary>
        public decimal RewardRate { get; set; }

        /// <summary>
        /// 奖励金额
        /// </summary>
        public decimal Reward { get; set; }

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

