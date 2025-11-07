namespace SmallTarget.Shared.ActionModels.Result
{
    /// <summary>
    /// 管理端已登录员工响应数据
    /// </summary>
    public class ManagementSigninedManagerResult
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public int Uid { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public required string Username { get; set; }

        /// <summary>
        /// 员工类型
        /// </summary>
        public int ManagerType { get; set; }

        /// <summary>
        /// 员工类型名称
        /// </summary>
        public required string ManagerTypeName {get; set;}

        /// <summary>
        /// 余额资产
        /// </summary>
        public decimal BalanceAssets { get; set; }

        /// <summary>
        /// 等待确认提款订单数
        /// </summary>
        public int WaitingConfirmOrdersCount { get; set; }

        /// <summary>
        /// 邀请链接
        /// </summary>
        public List<string>? InvitationLinks { get; set; }

        /// <summary>
        /// 访问令牌
        /// </summary>
        public string? AccessToken { get; set; }
    }
}
