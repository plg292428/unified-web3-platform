namespace SmallTarget.WebApi.Services;

/// <summary>
/// 临时签名令牌服务
/// </summary>
public static class TempSignTokenServiceExtensions
{
    /// <summary>
    /// 添加临时签名令牌服务
    /// </summary>
    /// <param name="services">服务</param>
    public static void AddTempSignTokenService(this IServiceCollection services)
    {
        services.AddSingleton<ITempSignToken, TempSignToken>();
    }
}
