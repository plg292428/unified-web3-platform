using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace HFastKit.AspNetCore.Services.Jwt
{
    /// <summary>
    /// JWT 服务拓展
    /// </summary>
    public static class JwtServiceExtensions
    {
        /// <summary>
        /// 添加 JWT 服务
        /// </summary>
        /// <param name="services">服务</param>
        /// <param name="options">配置</param>
        public static void AddJwt(this IServiceCollection services, Action<JwtOptions> options)
        {
            // 注入配置
            services.Configure(options);

            // 获取已注入的配置
            IOptions<JwtOptions> jwtOptions;
            using (var serviceProvider = services.BuildServiceProvider())
            {
                jwtOptions = serviceProvider.GetRequiredService<IOptions<JwtOptions>>();
            }

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    // 是否认证私钥
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Value.SecurityKey)),

                    // 是否认证发行人
                    ValidateIssuer = true,
                    ValidIssuer = jwtOptions.Value.Issuer,

                    // 是否认证订阅人
                    ValidateAudience = true,
                    ValidAudience = jwtOptions.Value.Audience,

                    // 是否认证时间
                    ValidateLifetime = true,

                    // 认证时间偏移
                    ClockSkew = TimeSpan.Zero,

                    // 是否要求Token的Claims中必须包含Expires
                    RequireExpirationTime = true
                };
            });
        }
    }
}