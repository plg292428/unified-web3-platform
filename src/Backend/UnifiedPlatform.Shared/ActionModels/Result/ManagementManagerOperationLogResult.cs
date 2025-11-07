namespace SmallTarget.Shared.ActionModels
{
    /// <summary>
    /// 管理端员工操作日志响应数据
    /// </summary>
    public class ManagementManagerOperationLogResult
    {
        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 操作者用户ID
        /// </summary>
        public int OperatorUid { get; set; }

        /// <summary>
        /// 操作者用户名
        /// </summary>
        public required string OperatorUsername { get; set; }

        /// <summary>
        /// 操作者员工类型
        /// </summary>
        public ManagerType OperatorManagerType { get; set; }

        /// <summary>
        /// 操作类型
        /// </summary>
        public ManagerOperationType OperationType { get; set; }

        /// <summary>
        /// 操作类型名称
        /// </summary>
        public required string OperationTypeName { get; set; }

        /// <summary>
        /// 目标员工用户名
        /// </summary>
        public string? TargetManagerUsername { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public required string Comment { get; set; }

        /// <summary>
        /// 目标客户UID
        /// </summary>
        public int? TargetUserUid { get; set; }

        /// <summary>
        /// 账变时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}
