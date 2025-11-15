using UnifiedPlatform.DbService.Entities;

namespace UnifiedPlatform.WebApi.Services;

/// <summary>
/// 临时缓存服务
/// </summary>
public static class TempCachingServiceExtensions
{
    /// <summary>
    /// 添加临时缓存服务
    /// </summary>
    /// <param name="services">服务</param>
    public static void AddTempCachingService(this IServiceCollection services)
    {
        using (var serviceProvider = services.BuildServiceProvider())
        {
            _ = serviceProvider.GetService<StDbContext>() ?? throw new Exception("Please inject the DbContext service first");
        }
        services.AddSingleton<ITempCaching, TempCaching>();
    }
}

