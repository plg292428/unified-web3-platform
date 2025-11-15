using Microsoft.Extensions.DependencyInjection;

namespace UnifiedPlatform.WebApi.Services.Polygon
{
    /// <summary>
    /// Polygon 服务扩展方法
    /// </summary>
    public static class PolygonServiceExtensions
    {
        /// <summary>
        /// 添加 Polygon 服务
        /// </summary>
        public static IServiceCollection AddPolygonService(this IServiceCollection services)
        {
            services.AddScoped<IPolygonService, PolygonService>();
            return services;
        }
    }
}

