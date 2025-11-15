using System.ComponentModel;

namespace UnifiedPlatform.Shared
{
    /// <summary>
    /// 主要代币状态
    /// </summary>
    public enum PrimaryTokenStatus
    {
        /// <summary>
        /// 未设置
        /// </summary>
        [Description("Not Set")]
        NotSet = 0,

        /// <summary>
        /// 处理中
        /// </summary>
        [Description("Pending")]
        Pending = 1,

        /// <summary>
        /// 已成功
        /// </summary>
        [Description("Completed")]
        Completed = 200,
    }
}

