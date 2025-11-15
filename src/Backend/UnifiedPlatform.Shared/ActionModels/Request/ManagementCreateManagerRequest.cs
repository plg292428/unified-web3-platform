namespace UnifiedPlatform.Shared.ActionModels
{
    /// <summary>
    /// 创建员工请求模型
    /// </summary>
    public class ManagementCreateManagerRequest
    {
        /// <summary>
        /// 用户名
        /// </summary>
        [UserName(ErrorMessage = "用户名格式错误")]
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// 密码
        /// </summary>
        [Password(ErrorMessage = "密码格式错误")]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// 员工类型
        /// </summary>
        public ManagerType ManagerType { get; set; }


        /// <summary>
        /// 归属员工UID
        /// </summary>
        public int? AttributionManagerUid { get; set; }
    }
}

