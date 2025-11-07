using System.ComponentModel;

namespace SmallTarget.Shared
{
    /// <summary>
    /// 区块链交易状态
    /// </summary>
    public enum ChainTransactionStatus
    {
        /// <summary>
        /// 异常
        /// </summary>
        [Description("Error")]
        Error = -1,

        /// <summary>
        /// 没有信息
        /// </summary>
        [Description("None")]
        None = 0,

        /// <summary>
        /// 处理中
        /// </summary>
        [Description("Pending")]
        Pending = 1,

        /// <summary>
        /// 已失败
        /// </summary>
        [Description("Failed")]
        Failed = 2,

        /// <summary>
        /// 已成功
        /// </summary>
        [Description("Succeed")]
        Succeed = 200,
    }
}
