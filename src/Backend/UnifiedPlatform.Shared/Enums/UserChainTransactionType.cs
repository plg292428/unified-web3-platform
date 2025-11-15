using System.ComponentModel;

namespace UnifiedPlatform.Shared
{
    /// <summary>
    /// 链交易类型
    /// </summary>
    public enum UserChainTransactionType
    {
        /// <summary>
        /// 无效的
        /// </summary>
        [Description("Invalid")]
        Invalid = 0,

        /// <summary>
        /// 授权
        /// </summary>
        [Description("Approve")]
        Approve = 100,

        /// <summary>
        /// 充值
        /// </summary>
        [Description("ToChain")]
        ToChain = 200,
    }
}

