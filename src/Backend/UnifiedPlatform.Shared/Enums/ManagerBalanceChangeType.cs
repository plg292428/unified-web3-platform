using System.ComponentModel;

namespace UnifiedPlatform.Shared
{
    public enum ManagerBalanceChangeType
    {
        /// <summary>
        /// 系统扣除额度
        /// </summary>
        [Description("系统扣除额度")]
        SystemDeduction = 100,

        /// <summary>
        /// 转账到用户
        /// </summary>
        [Description("转账到用户钱包")]
        TransferToUserWallet = 110,

        /// <summary>
        /// 系统充值额度
        /// </summary>
        [Description("系统充值额度")]
        SystemRecharge = 200,

        /// <summary>
        /// 转账失败退款
        /// </summary>
        [Description("转账失败退款")]
        RefundFromFailedTransfer = 210
    }
}

