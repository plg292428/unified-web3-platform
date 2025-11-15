using System.ComponentModel;

namespace UnifiedPlatform.Shared
{
    /// <summary>
    /// 邀请者类型
    /// </summary>
    public enum InviterType
    {
        /// <summary>
        /// 无效
        /// </summary>
        [Description("Invalid")]
        Invalid = 0,

        /// <summary>
        /// Dapp 用户
        /// </summary>
        [Description("Dapp User")]
        DappUser = 1,

        /// <summary>
        /// 员工
        /// </summary>
        [Description("Employee")]
        Employee = 2,
    }
}

