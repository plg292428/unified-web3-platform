using System.ComponentModel;

namespace UnifiedPlatform.Shared
{
    /// <summary>
    /// 管理员操作日志类型
    /// </summary>
    public enum ManagerOperationType
    {
        /// <summary>
        /// 无
        /// </summary>
        [Description("无")]
        None = 0,

        /// <summary>
        /// 创建员工
        /// </summary>
        [Description("创建员工")]
        CreateManager = 100,

        /// <summary>
        /// 修改自身登录密码
        /// </summary>
        [Description("修改自身登录密码")]
        ChangeManagerSelfPassword = 101,

        /// <summary>
        /// 修改员工登录密码
        /// </summary>
        [Description("修改员工登录密码")]
        ChangeManagerPassword = 102,

        /// <summary>
        /// 更改代理客服配置
        /// </summary>
        [Description("更改代理客服配置")]
        UpdateAgentCustomerServiceConfig = 103,

        /// <summary>
        /// 更改代理线禁用状态
        /// </summary>
        [Description("更改代理线禁用状态")]
        UpdateAgentAndSubManagersBlockState = 104,

        /// <summary>
        /// 更改管理员禁用状态
        /// </summary>
        [Description("更改管理员禁用状态")]
        UpdateAdministratorBlockState = 105,

        /// <summary>
        /// 更改代理额度
        /// </summary>
        [Description("更改代理额度")]
        UpdateAgentBalanceAssets = 110
    }
}

