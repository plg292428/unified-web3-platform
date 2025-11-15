using System.ComponentModel;

namespace UnifiedPlatform.Shared
{
    /// <summary>
    /// 用户AI交易订单状态
    /// </summary>
    public enum UserAiTradingOrderStatus
    {
        /// <summary>
        /// 交易中
        /// </summary>
        [Description("Trading")]
        Trading = 0,

        /// <summary>
        /// 已失败
        /// </summary>
        [Description("Failed")]
        Failed = 2,

        /// <summary>
        /// 已成功
        /// </summary>
        [Description("Completed")]
        Completed = 200,
    }
}

