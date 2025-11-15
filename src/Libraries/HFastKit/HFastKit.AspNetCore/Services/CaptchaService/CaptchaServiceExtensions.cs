using Microsoft.Extensions.DependencyInjection;

namespace HFastKit.AspNetCore.Services.Captcha
{
    /// <summary>
    /// 验证码服务拓展
    /// </summary>
    public static class CaptchaServiceExtensions
    {
        /// <summary>
        /// 添加验证码服务
        /// </summary>
        /// <param name="services">服务</param>
        public static void AddCaptcha(this IServiceCollection services)
        {
            services.AddSingleton<ICaptchaFactory, CaptchaFactory>();
        }
    }
}

