namespace UnifiedPlatform.Shared.ActionModels
{
    /// <summary>
    /// 管理端代理账变响应数据
    /// </summary>
    public class ManagementAgentBalanceAssetsChangeResult
    {
        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public int Uid { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public required string Username { get; set; }

        /// <summary>
        /// 账变类型
        /// </summary>
        public ManagerBalanceChangeType ChangeType { get; set; }

        /// <summary>
        /// 账变类型名称
        /// </summary>
        public required string ChangeTypeName { get; set; }

        /// <summary>
        /// 是否为增加
        /// </summary>
        public bool Increased => ChangeType >= ManagerBalanceChangeType.SystemRecharge ? true : false;

        /// <summary>
        /// 变动值
        /// </summary>
        public decimal Change { get; set; }

        /// <summary>
        /// 变动前值
        /// </summary>
        public decimal Before    { get; set; }

        /// <summary>
        /// 变动后值
        /// </summary>
        public decimal After { get; set; }

        /// <summary>
        /// 备注信息
        /// </summary>
        public string? Comment { get; set; }

        /// <summary>
        /// 账变时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}

