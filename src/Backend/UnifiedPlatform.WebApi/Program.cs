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
using SmallTarget.DbService.Entities; // TODO: Update to UnifiedPlatform.DbService.Entities
using SmallTarget.Shared; // TODO: Update to UnifiedPlatform.Shared
using SmallTarget.WebApi.Filters; // TODO: Update namespace
using SmallTarget.WebApi.Services; // TODO: Update namespace
using UnifiedPlatform.WebApi.Services.Tron;

namespace SmallTarget.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
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

            // IP地址查询服务（可选，如果ip2region.xdb文件不存在则注释）
            // builder.Services.AddIp2RegionService();

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

            // 验证码服务（可选，暂时注释）
            // builder.Services.AddCaptcha();

            // 临时签名验证服务（可选，暂时注释）
            // builder.Services.AddTempSignTokenService();

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

            // 定时任务（可选，暂时注释）
            // builder.Services.AddScheduleJobs();

            // 配置跨域资源共享
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("Any", policy =>
                {
                    policy.AllowAnyHeader();
                    policy.AllowAnyOrigin();
                    policy.AllowAnyMethod();
                });
            });

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

            app.UseCors("Any");

            // 启用HTTPS重定向（开发和生产环境都支持）
            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }))
                .WithName("HealthCheck")
                .WithOpenApi();

            app.Run();
        }
    }
}
