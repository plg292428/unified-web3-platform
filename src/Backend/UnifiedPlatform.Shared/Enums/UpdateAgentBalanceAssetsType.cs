using System.ComponentModel;

namespace UnifiedPlatform.Shared
{
    /// <summary>
    /// 更新代理余额资产类型
    /// </summary>
    public enum UpdateAgentBalanceAssetsType
    {
        /// <summary>
        /// 增加
        /// </summary>
        [Description("Increase")]
        Increase = 100,

        /// <summary>
        /// 扣除
        /// </summary>
        [Description("Deduction")]
        Deduction = 200,
    }
}

