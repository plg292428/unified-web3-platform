using Microsoft.Extensions.DependencyInjection;
using UnifiedPlatform.WebApi.Services.Tron;

namespace UnifiedPlatform.WebApi.Services.Tron
{
    /// <summary>
    /// TRON 服务扩展方法
    /// </summary>
    public static class TronServiceExtensions
    {
        /// <summary>
        /// 添加 TRON 服务
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <returns>服务集合</returns>
        public static IServiceCollection AddTronService(this IServiceCollection services)
        {
            services.AddScoped<ITronService, TronService>();
            return services;
        }
    }
}



