namespace SmallTarget.Shared.ActionModels
{
    /// <summary>
    /// 管理端更新代理余额资产请求
    /// </summary>
    public class ManagementUpdateAgentBalanceAssetsRequest
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public int Uid { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        [TokenAmount(ErrorMessage = "操作金额格式错误")]
        public decimal Amount { get; set; }

        /// <summary>
        /// 更新资产类型
        /// </summary>
        public UpdateAgentBalanceAssetsType UpdateAgentBalanceAssetsType { get; set; }

        /// <summary>
        /// 备注信息
        /// </summary>
        public string? Comment { get; set; }
    }
}
