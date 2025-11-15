namespace UnifiedPlatform.Shared.ActionModels
{
    /// <summary>
    /// 管理端修改密码请求
    /// </summary>
    public class ManagementChangePasswordRequest
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string? Username { get; set; }

        /// <summary>
        /// 旧密码
        /// </summary>
        public string? OldPassword { get; set; }

        /// <summary>
        /// 新密码
        /// </summary>
        [Password(ErrorMessage = "修改失败，新密码格式错误")]
        public required string NewPassword { get; set; }
    }
}

