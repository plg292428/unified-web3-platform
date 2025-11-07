using System.ComponentModel;

namespace SmallTarget.Shared
{
    /// <summary>
    /// 用户挖矿状态
    /// </summary>
    public enum UserMiningStatus
    {
        /// <summary>
        /// 已停止
        /// </summary>
        [Description("Mining Stopped")]
        MiningStopped = -1,

        /// <summary>
        /// 标准挖矿中
        /// </summary>
        [Description("Standard Mining")]
        StandardMining = 10,

        /// <summary>
        /// 高速挖矿中
        /// </summary>
        [Description("High Speed Mining")]
        HighSpeedMining = 11
    }
}
