using System.ComponentModel;

namespace SmallTarget.Shared
{
    /// <summary>
    /// 用户邀请奖励类型
    /// </summary>
    public enum UserInvitationRewardType
    {
        /// <summary>
        /// 无效
        /// </summary>
        [Description("Invalid")]
        Invalid = 0,

        /// <summary>
        /// 免质押挖矿
        /// </summary>
        [Description("Stake-Free Mining")]
        StakeFreeMining = 1,

        /// <summary>
        /// AI 合约交易奖励
        /// </summary>
        [Description("Ai Contract Trading")]
        AiContractTrading = 2,
    }
}
