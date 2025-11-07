namespace SmallTarget.Shared.ActionModels
{
    /// <summary>
    /// 管理端员工类型配置响应数据
    /// </summary>
    public class ManagementManagerTypeConfigResult
    {
        /// <summary>
        /// 员工类型
        /// </summary>
        public int ManagerType { get; set; }

        /// <summary>
        /// 员工类型描述
        /// </summary>
        public required string ManagerTypeDescription { get; set; }
    }
}
