using System.ComponentModel;

namespace SmallTarget.Shared
{
    /// <summary>
    /// 用户链上资产变动类型
    /// </summary>
    public enum UserOnChainAssetsChangeType
    {
        /// <summary>
        /// 无效
        /// </summary>
        [Description("Invalid")]
        Invalid = 0,

        /***** 100 - 199 减少*****/

        /// <summary>
        /// 到钱包
        /// </summary>
        [Description("Transfer To Wallet")]
        ToWallet = 100,

        /// <summary>
        /// AI 合约交易扣款
        /// </summary>
        [Description("AI Contract Trading")]
        AiContractTrading = 110,

        /// <summary>
        /// 转入黑洞
        /// </summary>
        [Description("Transfer To Black Hole")]
        TransferToBlackHole = 199,

        /***** 200 - 299 增加*****/

        /// <summary>
        /// 到链上
        /// </summary>
        [Description("Transfer To Chain")]
        ToChain = 200,

        /// <summary>
        /// 充值到链上的退款
        /// </summary>
        [Description("Refund From Failed Transfer")]
        RefundFromFailedTransfer = 201,

        /// <summary>
        /// 转移后充值到链上
        /// </summary>
        [Description("Transfer To Chain")]
        TransferFromRechargeToChain = 202,

        /// <summary>
        /// AI 合约交易奖励
        /// </summary>
        [Description("Ai Contract Trading Income")]
        AiContractTradingIncome = 210,

        /// <summary>
        /// 挖矿奖励
        /// </summary>
        [Description("Stake-Free Mining Income")]
        StakeFreeMiningIncome = 211,

        /// <summary>
        /// 邀请奖励
        /// </summary>
        [Description("Invitation Reward")]
        InvitationReward = 212,

        /// <summary>
        /// 系统奖励
        /// </summary>
        [Description("System Reward")]
        SystemReward = 212,
    }
}
