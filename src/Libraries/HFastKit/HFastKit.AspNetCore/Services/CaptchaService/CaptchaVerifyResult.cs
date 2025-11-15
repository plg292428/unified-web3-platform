namespace HFastKit.AspNetCore.Services.Captcha
{
    /// <summary>
    /// 验证码验证结果
    /// </summary>
    public class CaptchaVerifyResult
    {
        /// <summary>
        /// 是否验证成功
        /// </summary>
        public bool IsVerified { get; set; } = false;

        /// <summary>
        /// 错误消息
        /// </summary>
        public string ErrorMessage { get; set; } = string.Empty;
    }
}

