using HFastKit.AspNetCore.Filters;
using HFastKit.AspNetCore.Middlewares;
using HFastKit.AspNetCore.Services;
using HFastKit.AspNetCore.Services.Captcha;
using HFastKit.AspNetCore.Services.Jwt;
using HFastKit.AspNetCore.Shared;
using HFastKit.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Nblockchain.Signer;
using Nblockchain.Tron;
using UnifiedPlatform.DbService.Entities; // TODO: Update to UnifiedPlatform.DbService.Entities
using UnifiedPlatform.Shared; // TODO: Update to UnifiedPlatform.Shared
using UnifiedPlatform.WebApi.Filters; // TODO: Update namespace
using UnifiedPlatform.WebApi.Services; // TODO: Update namespace
using UnifiedPlatform.WebApi.Services.Tron;
using UnifiedPlatform.WebApi.Services.Polygon;

namespace UnifiedPlatform.WebApi
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddMemoryCache();

            builder.Services.AddControllers(options =>
            {
                options.Filters.Add<WrapperResultFilter>();
                options.Filters.Add<UserAuthorizationFilter>();
            }).AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
                options.JsonSerializerOptions.ReadCommentHandling = System.Text.Json.JsonCommentHandling.Skip;
                options.JsonSerializerOptions.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
            });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // IP地址查询服务（依赖 ip2region.xdb）
            builder.Services.AddIp2RegionService();

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy(AuthorizationPolicyName.Any, policy => policy.RequireClaim(JwtClaimKeyName.RequestUserTypeName, RequestUserTypeName.DappUser, JwtClaimKeyName.RequestUserTypeName, RequestUserTypeName.Salesman, JwtClaimKeyName.RequestUserTypeName, RequestUserTypeName.GroupLeader, JwtClaimKeyName.RequestUserTypeName, RequestUserTypeName.Agent, JwtClaimKeyName.RequestUserTypeName, RequestUserTypeName.Administrator, JwtClaimKeyName.RequestUserTypeName, RequestUserTypeName.Developer));
                options.AddPolicy(AuthorizationPolicyName.DappUser, policy => policy.RequireClaim(JwtClaimKeyName.RequestUserTypeName, RequestUserTypeName.DappUser));
                options.AddPolicy(AuthorizationPolicyName.Manager, policy => policy.RequireClaim(JwtClaimKeyName.RequestUserTypeName, RequestUserTypeName.Salesman, JwtClaimKeyName.RequestUserTypeName, RequestUserTypeName.GroupLeader, JwtClaimKeyName.RequestUserTypeName, RequestUserTypeName.Agent, JwtClaimKeyName.RequestUserTypeName, RequestUserTypeName.Administrator, JwtClaimKeyName.RequestUserTypeName, RequestUserTypeName.Developer));
                options.AddPolicy(AuthorizationPolicyName.GroupLeader, policy => policy.RequireClaim(JwtClaimKeyName.RequestUserTypeName, RequestUserTypeName.GroupLeader, JwtClaimKeyName.RequestUserTypeName, RequestUserTypeName.Agent, JwtClaimKeyName.RequestUserTypeName, RequestUserTypeName.Administrator, JwtClaimKeyName.RequestUserTypeName, RequestUserTypeName.Developer));
                options.AddPolicy(AuthorizationPolicyName.Agent, policy => policy.RequireClaim(JwtClaimKeyName.RequestUserTypeName, RequestUserTypeName.Agent, JwtClaimKeyName.RequestUserTypeName, RequestUserTypeName.Administrator, JwtClaimKeyName.RequestUserTypeName, RequestUserTypeName.Developer));
                options.AddPolicy(AuthorizationPolicyName.Administrator, policy => policy.RequireClaim(JwtClaimKeyName.RequestUserTypeName, RequestUserTypeName.Administrator, JwtClaimKeyName.RequestUserTypeName, RequestUserTypeName.Developer));
                options.AddPolicy(AuthorizationPolicyName.Developer, policy => policy.RequireClaim(JwtClaimKeyName.RequestUserTypeName, RequestUserTypeName.Developer));
            });

            // 配置JWT认证
            string? securityKey = builder.Configuration["JwtSettings:SecurityKey"] ?? throw new ArgumentNullException("JwtSettings SecurityKey");
            securityKey = securityKey.ComputeHashToHex().DesEncrypt().Substring(0, 32);
            builder.Services.AddJwt(options =>
            {
                options.Audience = builder.Configuration["JwtSettings:Audience"] ?? throw new ArgumentNullException("JwtSettings Audiencea");
                options.Issuer = builder.Configuration["JwtSettings:Issuer"] ?? throw new ArgumentNullException("JwtSettings Issuer");
                options.SecurityKey = securityKey;
            });

            // 验证码服务
            builder.Services.AddCaptcha();

            // 临时签名验证服务
            builder.Services.AddTempSignTokenService();

            // 配置数据库上下文
            builder.Services.AddDbContextPool<StDbContext>((options) =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            // 数据库缓存服务
            builder.Services.AddTempCachingService();

            // Web3 提供者服务
            builder.Services.AddWeb3ProviderService();

            // 配置TRON服务
            var tronNetwork = builder.Configuration["TronSettings:Network"] ?? "MainNet";
            var tronApiKey = builder.Configuration["TronSettings:ApiKey"] ?? "";
            var tronRpcUrl = builder.Configuration["TronSettings:RpcUrl"] ?? "";
            builder.Services.AddTronWeb(options =>
            {
                options.Network = tronNetwork == "MainNet" ? TronNetwork.MainNet : TronNetwork.TestNet;
                options.ApiKey = tronApiKey;
                // 如果配置了RPC URL，则使用配置的URL，否则使用默认值
                if (!string.IsNullOrEmpty(tronRpcUrl))
                {
                    options.RpcUrl = tronRpcUrl;
                }
                else
                {
                    // 根据网络类型设置默认RPC URL
                    options.RpcUrl = tronNetwork == "MainNet" ? "grpc.trongrid.io:50051" : "grpc.shasta.trongrid.io:50051";
                }
            });

            // 添加TRON服务封装
            builder.Services.AddTronService();

            // 添加Polygon服务
            builder.Services.AddPolygonService();

            // 定时任务（包含订单支付验证）
            builder.Services.AddScheduleJobs();

            // 配置跨域资源共享
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("DefaultCors", policy =>
                {
                    policy
                        .WithOrigins(
                            // 生产环境域名（HTTPS，标准端口 443）
                            "https://www.a292428dsj.dpdns.org",
                            "https://a292428dsj.dpdns.org",
                            "https://unified-web3-platform.pages.dev",
                            // 本地开发域名（HTTPS，通过 hosts 文件映射）
                            "https://www.a292428dsj.dpdns.org:8443",
                            "https://a292428dsj.dpdns.org:8443",
                            // 本地开发域名（HTTP，通过 hosts 文件映射）
                            "http://www.a292428dsj.dpdns.org:5173",
                            "http://a292428dsj.dpdns.org:5173",
                            // 本地开发环境
                            "http://localhost:5173",
                            "http://localhost:5174",
                            "https://localhost:8443",
                            "http://127.0.0.1:5173",
                            "http://127.0.0.1:5174",
                            "https://127.0.0.1:8443"
                        )
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });

            // 注册种子数据服务
            builder.Services.AddScoped<Services.SeedDataService>();

            var app = builder.Build();

            // 自动异常处理
            app.UseEasyExceptionHandler();
            app.UseFailedStatusCodeResponseHandler();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // 使用静态文件
            string staticFilesDirectory = Path.Combine(Directory.GetCurrentDirectory(), "StaticFiles");
            if (Directory.Exists(staticFilesDirectory))
            {
                app.UseStaticFiles(new StaticFileOptions
                {
                    FileProvider = new PhysicalFileProvider(staticFilesDirectory),
                    RequestPath = "/StaticFiles"
                });
            }

            // CORS 必须在 UseHttpsRedirection 之前
            app.UseCors("DefaultCors");

            // 启用HTTPS重定向（开发和生产环境都支持）
            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            // 根路径重定向到 Swagger（必须在 MapControllers 之前）
            app.MapGet("/", () => Results.Redirect("/swagger"))
                .WithName("Root")
                .WithOpenApi();

            app.MapControllers();

            app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }))
                .WithName("HealthCheck")
                .WithOpenApi();

            // 在开发环境中自动添加示例商品
            if (app.Environment.IsDevelopment())
            {
                using (var scope = app.Services.CreateScope())
                {
                    var seedService = scope.ServiceProvider.GetRequiredService<Services.SeedDataService>();
                    await seedService.SeedProductsAsync();
                }
            }

            await app.RunAsync();
        }
    }
}

