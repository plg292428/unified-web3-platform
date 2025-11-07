using System.ComponentModel;

namespace SmallTarget.Shared
{
    /// <summary>
    /// 管理端查询用户类型
    /// </summary>
    public enum ManagementQueryUserType
    {
        /// <summary>
        /// 所有用户
        /// </summary>
        [Description("所有用户")]
        AllUsers = 0,

        /// <summary>
        /// 真实用户
        /// </summary>
        [Description("真实用户")]
        RealUsers = 100,

        /// <summary>
        /// 所有真实已授权用户
        /// </summary>
        [Description("已授权用户")]
        ApprovedRealUsers = 101,

        /// <summary>
        ///未授权用户
        /// </summary>
        [Description("未授权用户")]
        UnapprovedRealUsers = 102,

        /// <summary>
        /// 虚拟用户
        /// </summary>
        [Description("虚拟用户")]
        VirtualUsers = 200,
    }
}
