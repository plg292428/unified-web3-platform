namespace UnifiedPlatform.Shared.ActionModels
{
    /// <summary>
    /// 管理端员工登录请求
    /// </summary>
    public class ManagementLoginRequest
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public required string Username { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public required string Password { get; set; }

        /// <summary>
        /// 验证码
        /// </summary>
        public required string Captcha { get; set; }

        /// <summary>
        /// 验证码令牌
        /// </summary>
        public required string CaptchaToken { get; set; }
    }
}

