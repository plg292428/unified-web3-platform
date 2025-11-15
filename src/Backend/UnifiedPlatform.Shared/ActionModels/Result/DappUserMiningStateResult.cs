namespace UnifiedPlatform.Shared.ActionModels
{
    /// <summary>
    /// Dapp 用户挖矿状态响应数据
    /// </summary>
    public class DappUserMiningStateResult
    {
        /// <summary>
        /// 挖矿状态
        /// </summary>
        public UserMiningStatus MiningStatus { get; set; }

        /// <summary>
        /// 挖矿状态名称
        /// </summary>
        public required string MiningStatusName { get; set; }

        /// <summary>
        /// 停止的提示
        /// </summary>
        public  string? StopedTip { get; set; }
    }
}

