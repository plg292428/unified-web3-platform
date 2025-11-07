using HFastKit.Text;
using System.Collections.Concurrent;

namespace HFastKit.AspNetCore.Services.Captcha
{
    /// <summary>
    /// 验证码工厂
    /// </summary>
    public class CaptchaFactory : ICaptchaFactory
    {
        /// <summary>
        /// 默认验证码过期时间（单位：秒）
        /// </summary>
        private readonly int _expirationSeconds = 300;

        /// <summary>
        /// 验证码缓存
        /// </summary>
        public ConcurrentDictionary<Guid, Captcha> Caching { get; } = new();

        /// <summary>
        /// 创建验证码实例
        /// </summary>
        /// <param name="expirationSeconds">过期时间（秒）</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public Captcha Create(int? expirationSeconds = null)
        {
            // 清理过期验证码
            ClearExpiredCaching();

            TimeSpan expirationTime;
            if (!expirationSeconds.HasValue)
            {
                expirationTime = new TimeSpan(0, 0, _expirationSeconds);
            }
            else
            {
                if (expirationSeconds.Value < 1)
                {
                    throw new ArgumentOutOfRangeException(nameof(expirationSeconds));
                }
                expirationTime = new TimeSpan(0, 0, expirationSeconds.Value);
            }
            Guid guid = Guid.NewGuid();
            Captcha captcha = new(RandomText.Generate(4), expirationTime);
            Caching.TryAdd(guid, captcha);
            return captcha;
        }

        /// <summary>
        /// 验证验证码
        /// </summary>
        /// <param name="captchaText">验证码文本</param>
        /// <param name="captchaToken">验证码Token</param>
        /// <param name="caseSensitive">是否大小写敏感</param>
        /// <returns></returns>
        public CaptchaVerifyResult Verify(string captchaText, string captchaToken, bool caseSensitive = false)
        {
            // 清理过期验证码
            ClearExpiredCaching();

            CaptchaVerifyResult result = new();
            if (string.IsNullOrEmpty(captchaText) || string.IsNullOrEmpty(captchaToken))
            {
                result.ErrorMessage = "验证码错误";
                return result;
            }

            Captcha? captcha = Caching.FirstOrDefault(o => o.Value.Token == captchaToken).Value;
            if (captcha is null)
            {
                result.ErrorMessage = "验证码已过期";
                return result;
            }

            // 验证失败删除验证码
            if (caseSensitive)
            {
                if (captchaText != captcha.Text)
                {
                    Caching.TryRemove(captcha.Guid, out var _);
                    result.ErrorMessage = "验证码错误";
                    return result;
                }
            }
            else
            {
                if (captchaText.ToLower() != captcha.Text.ToLower())
                {
                    Caching.TryRemove(captcha.Guid, out var _);
                    result.ErrorMessage = "验证码错误";
                    return result;
                }
            }

            // 验证成功删除验证码
            Caching.TryRemove(captcha.Guid, out var _);
            result.IsVerified = true;
            return result;
        }

        /// <summary>
        /// 清理过期缓存
        /// </summary>
        private void ClearExpiredCaching()
        {
            if (Caching.IsEmpty)
            {
                return;
            }
            foreach (var item in Caching.Where(o => o.Value.ExpirationTime < DateTime.UtcNow))
            {
                Caching.TryRemove(item.Key, out _);
            }
        }
    }
}
