namespace UnifiedPlatform.Shared.ActionModels
{
    /// <summary>
    /// 验证码响应数据
    /// </summary>
    public class CaptchaResult
    {
        /// <summary>
        /// 验证码Token
        /// </summary>
        public string? Token { get; set; }

        /// <summary>
        /// 验证码图片链接
        /// </summary>
        public string? ImageUrl { get; set; }
    }
}

