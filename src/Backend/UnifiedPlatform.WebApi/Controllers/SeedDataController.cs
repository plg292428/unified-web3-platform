using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UnifiedPlatform.WebApi.Services;

namespace UnifiedPlatform.WebApi.Controllers
{
    /// <summary>
    /// 种子数据控制器（仅开发环境）
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class SeedDataController : ControllerBase
    {
        private readonly SeedDataService _seedDataService;
        private readonly ILogger<SeedDataController> _logger;
        private readonly IWebHostEnvironment _environment;

        public SeedDataController(
            SeedDataService seedDataService,
            ILogger<SeedDataController> logger,
            IWebHostEnvironment environment)
        {
            _seedDataService = seedDataService;
            _logger = logger;
            _environment = environment;
        }

        /// <summary>
        /// 手动触发添加商品数据（仅开发环境）
        /// </summary>
        [HttpPost("products")]
        [AllowAnonymous]
        public async Task<IActionResult> SeedProducts()
        {
            if (!_environment.IsDevelopment())
            {
                return BadRequest(new { message = "此功能仅在开发环境中可用" });
            }

            try
            {
                await _seedDataService.SeedProductsAsync();
                return Ok(new { message = "商品数据添加完成" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "手动添加商品数据失败");
                return StatusCode(500, new { message = "添加商品数据失败", error = ex.Message });
            }
        }
    }
}

