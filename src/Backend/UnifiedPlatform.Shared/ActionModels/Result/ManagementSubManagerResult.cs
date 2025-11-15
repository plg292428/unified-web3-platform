namespace UnifiedPlatform.Shared.ActionModels
{
    /// <summary>
    /// 下级员工响应结果附加数据
    /// </summary>
    public class ManagementSubManagerResult
    {
        public int Uid { get; set; }

        public ManagerType ManagerType { get; set; }

        public required string Username { get; set; }
    }
}

