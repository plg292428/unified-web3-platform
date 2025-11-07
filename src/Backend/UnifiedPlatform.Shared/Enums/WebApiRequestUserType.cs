using System.ComponentModel;

namespace SmallTarget.Shared
{
    /// <summary>
    /// 请求用户类型
    /// </summary>
    public enum WebApiRequestUserType
    {
        /// <summary>
        /// Dapp 用户
        /// </summary>
        [Description("Dapp User")]
        DappUser = 0,

        /// <summary>
        /// 业务员
        /// </summary>
        [Description("Salesman")]
        Salesman = 1,

        /// <summary>
        /// 组长
        /// </summary>
        [Description("Group Leader")]
        GroupLeader = 10,

        // 代理
        [Description("Agent")]
        Agent = 100,

        // 管理员
        [Description("Administrator")]
        Administrator = 200,

        // 开发者
        [Description("Administrator")]
        Developer = 999,
    }
}
