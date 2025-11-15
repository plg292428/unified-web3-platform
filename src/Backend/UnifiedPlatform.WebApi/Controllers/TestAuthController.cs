using HFastKit.AspNetCore.Services.Jwt;
using HFastKit.AspNetCore.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using UnifiedPlatform.Shared;
using System.Security.Claims;

namespace UnifiedPlatform.WebApi.Controllers
{
    /// <summary>
    /// 测试认证控制器（仅用于开发测试）
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class TestAuthController : ControllerBase
    {
        private readonly IOptions<JwtOptions> _jwtOptions;
        private readonly ILogger<TestAuthController> _logger;

        public TestAuthController(IOptions<JwtOptions> jwtOptions, ILogger<TestAuthController> logger)
        {
            _jwtOptions = jwtOptions ?? throw new ArgumentNullException(nameof(jwtOptions));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// 生成测试JWT Token（仅用于开发测试）
        /// </summary>
        /// <param name="request">测试Token请求</param>
        /// <returns>JWT Token</returns>
        [HttpPost("generate-token")]
        [AllowAnonymous]
        public IActionResult GenerateTestToken([FromBody] GenerateTestTokenRequest? request = null)
        {
            try
            {
                // 仅在开发环境允许
                if (!HttpContext.RequestServices.GetRequiredService<IWebHostEnvironment>().IsDevelopment())
                {
                    return BadRequest(new { success = false, message = "此功能仅在开发环境可用" });
                }

                // 默认测试用户信息
                var uid = request?.Uid ?? 9999;
                var username = request?.Username ?? "testuser";
                var userType = request?.UserType ?? "Developer";

                // 创建Claims
                var claims = new List<Claim>
                {
                    new Claim(JwtClaimKeyName.Uid, uid.ToString()),
                    new Claim(JwtClaimKeyName.Username, username),
                    new Claim(JwtClaimKeyName.AccesTokenGuid, Guid.NewGuid().ToString()),
                    new Claim(JwtClaimKeyName.RequestUserType, userType),
                    new Claim(JwtClaimKeyName.RequestUserTypeName, userType)
                };

                // 生成Token
                var token = JwtHelper.CreateToken(
                    _jwtOptions.Value.Audience,
                    _jwtOptions.Value.Issuer,
                    _jwtOptions.Value.SecurityKey,
                    claims.ToArray()
                );

                _logger.LogInformation("生成测试Token: Uid={Uid}, Username={Username}", uid, username);

                return Ok(new
                {
                    success = true,
                    data = new
                    {
                        token,
                        expiresIn = 3600, // 1小时
                        userInfo = new
                        {
                            uid,
                            username,
                            userType
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "生成测试Token失败");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        /// <summary>
        /// 验证JWT Token（仅用于开发测试）
        /// </summary>
        /// <returns>Token信息</returns>
        [HttpGet("verify-token")]
        [Authorize]
        public IActionResult VerifyToken()
        {
            try
            {
                var user = HttpContext.User;
                var claims = user.Claims.Select(c => new { c.Type, c.Value }).ToList();

                return Ok(new
                {
                    success = true,
                    data = new
                    {
                        isAuthenticated = user.Identity?.IsAuthenticated ?? false,
                        claims
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "验证Token失败");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }

    /// <summary>
    /// 生成测试Token请求
    /// </summary>
    public class GenerateTestTokenRequest
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public int? Uid { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string? Username { get; set; }

        /// <summary>
        /// 用户类型（Salesman, GroupLeader, Agent, Administrator, Developer）
        /// </summary>
        public string? UserType { get; set; }
    }
}


