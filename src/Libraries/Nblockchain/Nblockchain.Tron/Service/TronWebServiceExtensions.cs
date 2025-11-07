using Microsoft.Extensions.DependencyInjection;
using System;

namespace Nblockchain.Tron
{
    public static class TronWebServiceExtensions
    {
        public static IServiceCollection AddTronWeb(this IServiceCollection services, Action<TronWebOptions> setupAction)
        {
            services.Configure(setupAction);
            services.AddSingleton<ITronWebFactory, TronWebFactory>();
            return services;
        }
    }
}
