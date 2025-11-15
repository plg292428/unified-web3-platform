using System.ComponentModel;

namespace UnifiedPlatform.Shared
{
    /// <summary>
    /// 用户领取订单状态
    /// </summary>
    public enum UserToWalletOrderStatus
    {
        /// <summary>
        /// 异常订单
        /// </summary>
        [Description("Error")]
        Error = -1,

        /// <summary>
        /// 等待员工处理
        /// </summary>
        [Description("Waiting")]
        Waiting = 0,

        /// <summary>
        /// 转账处理中
        /// </summary>
        [Description("Pending")]
        Pending = 1,

        /// <summary>
        /// 已失败
        /// </summary>
        [Description("Failed")]
        Failed = 2,

        /// <summary>
        /// 成功
        /// </summary>
        [Description("Succeed")]
        Succeed = 200,
    }
}

