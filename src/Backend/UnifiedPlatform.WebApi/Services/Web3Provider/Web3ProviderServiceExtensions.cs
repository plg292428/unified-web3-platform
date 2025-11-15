using UnifiedPlatform.DbService.Entities;

namespace UnifiedPlatform.WebApi.Services;

/// <summary>
/// Web3 提供方服务扩展方法
/// </summary>
public static class Web3ProviderServiceExtensions
{
    /// <summary>
    /// 添加临时缓存服务
    /// </summary>
    /// <param name="services">服务</param>
    public static void AddWeb3ProviderService(this IServiceCollection services)
    {
        using (var serviceProvider = services.BuildServiceProvider())
        {
            _ = serviceProvider.GetService<ITempCaching>() ?? throw new Exception("Please inject the TempCaching service first");
        }
        services.AddSingleton<IWeb3ProviderService, Web3ProviderService>();
    }
}

