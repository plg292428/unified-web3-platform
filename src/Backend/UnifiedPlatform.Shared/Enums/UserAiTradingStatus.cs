using System.ComponentModel;

namespace SmallTarget.Shared
{
    /// <summary>
    /// 用户AI交易状态
    /// </summary>
    public enum UserAiTradingStatus
    {
        /// <summary>
        /// 空闲
        /// </summary>
        [Description("Error")]
        Error = -1,

        /// <summary>
        /// 空闲
        /// </summary>
        [Description("None")]
        None = 0,

        /// <summary>
        /// 交易中
        /// </summary>
        [Description("Pending")]
        Pending = 1,
    }
}
