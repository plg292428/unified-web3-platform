using System.Collections.Concurrent;

namespace HFastKit.AspNetCore.Services.Captcha
{
    /// <summary>
    /// 验证码工厂接口
    /// </summary>
    public interface ICaptchaFactory
    {
        /// <summary>
        /// 验证码缓存
        /// </summary>
        public ConcurrentDictionary<Guid, Captcha> Caching { get; }

        /// <summary>
        /// 创建验证码实例
        /// </summary>
        /// <param name="expirationSeconds">过期时间（秒）</param>
        /// <returns></returns>
        public Captcha Create(int? expirationSeconds = null);

        /// <summary>
        /// 验证验证码
        /// </summary>
        /// <param name="captchaText">验证码文本</param>
        /// <param name="captchaToken">验证码Token</param>
        /// <param name="caseSensitive">是否大小写敏感</param>
        /// <returns></returns>
        public CaptchaVerifyResult Verify(string captchaText, string captchaToken, bool caseSensitive = false);
    }
}

