namespace SmallTarget.Shared.ActionModels
{
    /// <summary>
    /// Dapp 用户邀请奖励记录响应数据
    /// </summary>
    public class DappUserInvitationRewardRecordResult
    {
        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 下级用户层级
        /// </summary>
        public int SubUserLayer { get; set; }

        /// <summary>
        /// 下级用户奖励类型
        /// </summary>
        public int SubUserRewardType { get; set; }

        /// <summary>
        /// 下级用户奖励类型名称
        /// </summary>
        public required string SubUserRewardTypeName { get; set; }

        /// <summary>
        /// 奖励率
        /// </summary>
        public decimal? RewardRate { get; set; }

        /// <summary>
        /// 奖励
        /// </summary>
        public decimal? Reward { get; set; }

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
