using HFastKit.AspNetCore.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UnifiedPlatform.DbService.Entities;
using UnifiedPlatform.Shared;
using UnifiedPlatform.Shared.ActionModels.Result;
using UnifiedPlatform.WebApi.Services;

namespace UnifiedPlatform.WebApi.Controllers
{
    /// <summary>
    /// 商品推荐控制器
    /// </summary>
    [Consumes("application/json")]
    [Route("api/recommendations")]
    [ApiController]
    public class RecommendationController : ApiControllerBase
    {
        private readonly StDbContext _dbContext;
        private readonly ITempCaching _tempCaching;

        public RecommendationController(StDbContext dbContext, ITempCaching tempCaching)
        {
            _dbContext = dbContext;
            _tempCaching = tempCaching;
        }

        /// <summary>
        /// 获取推荐商品（基于热门商品）
        /// </summary>
        [HttpGet("hot")]
        [AllowAnonymous]
        public async Task<WrappedResult<List<StoreProductSummaryResult>>> GetHotProducts([FromQuery] int limit = 10)
        {
            if (limit < 1) limit = 10;
            if (limit > 50) limit = 50;

            // 基于订单数量计算热门商品
            var hotProducts = await _dbContext.OrderItems
                .AsNoTracking()
                .Where(oi => oi.Order.Status == Shared.Enums.StoreOrderStatus.Completed)
                .GroupBy(oi => oi.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    OrderCount = g.Count(),
                    TotalQuantity = g.Sum(oi => oi.Quantity)
                })
                .OrderByDescending(x => x.OrderCount)
                .ThenByDescending(x => x.TotalQuantity)
                .Take(limit)
                .Select(x => x.ProductId)
                .ToListAsync();

            var products = await _dbContext.Products
                .AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.Inventory)
                .Where(p => hotProducts.Contains(p.ProductId) && p.IsPublished)
                .Select(p => new StoreProductSummaryResult
                {
                    ProductId = p.ProductId,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category.Name,
                    Name = p.Name,
                    Subtitle = p.Subtitle,
                    ThumbnailUrl = p.ThumbnailUrl,
                    Price = p.Price,
                    Currency = p.Currency,
                    ChainId = p.ChainId ?? null,
                    IsPublished = p.IsPublished,
                    InventoryAvailable = p.Inventory != null ? p.Inventory.QuantityAvailable : 0
                })
                .ToListAsync();

            // 按原始顺序排序
            var result = hotProducts
                .Select(id => products.FirstOrDefault(p => p.ProductId == id))
                .Where(p => p != null)
                .ToList()!;

            return WrappedResult.Ok(result);
        }

        /// <summary>
        /// 获取相关商品推荐（基于分类）
        /// </summary>
        [HttpGet("related/{productId:long}")]
        [AllowAnonymous]
        public async Task<WrappedResult<List<StoreProductSummaryResult>>> GetRelatedProducts(long productId, [FromQuery] int limit = 8)
        {
            if (limit < 1) limit = 8;
            if (limit > 20) limit = 20;

            var product = await _dbContext.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.ProductId == productId);

            if (product is null)
            {
                return WrappedResult.Failed("商品不存在");
            }

            // 推荐同分类的其他商品
            var relatedProducts = await _dbContext.Products
                .AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.Inventory)
                .Where(p => p.CategoryId == product.CategoryId 
                    && p.ProductId != productId 
                    && p.IsPublished)
                .OrderByDescending(p => p.UpdateTime)
                .Take(limit)
                .Select(p => new StoreProductSummaryResult
                {
                    ProductId = p.ProductId,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category.Name,
                    Name = p.Name,
                    Subtitle = p.Subtitle,
                    ThumbnailUrl = p.ThumbnailUrl,
                    Price = p.Price,
                    Currency = p.Currency,
                    ChainId = p.ChainId ?? null,
                    IsPublished = p.IsPublished,
                    InventoryAvailable = p.Inventory != null ? p.Inventory.QuantityAvailable : 0
                })
                .ToListAsync();

            return WrappedResult.Ok(relatedProducts);
        }

        /// <summary>
        /// 获取用户个性化推荐（基于用户订单历史）
        /// </summary>
        [HttpGet("personalized")]
        public async Task<WrappedResult<List<StoreProductSummaryResult>>> GetPersonalizedRecommendations([FromQuery] int? uid = null, [FromQuery] int limit = 10)
        {
            if (limit < 1) limit = 10;
            if (limit > 50) limit = 50;

            int? userId = uid;
            if (userId == null && DappUser != null)
            {
                userId = DappUser.Uid;
            }

            if (!userId.HasValue)
            {
                // 如果没有用户ID，返回热门商品
                return await GetHotProducts(limit);
            }

            // 获取用户购买过的商品分类
            var userCategories = await _dbContext.Orders
                .AsNoTracking()
                .Where(o => o.Uid == userId.Value && o.Status == Shared.Enums.StoreOrderStatus.Completed)
                .SelectMany(o => o.OrderItems)
                .Select(oi => oi.Product.CategoryId)
                .Distinct()
                .ToListAsync();

            if (!userCategories.Any())
            {
                // 如果用户没有购买历史，返回热门商品
                return await GetHotProducts(limit);
            }

            // 推荐用户购买过的分类中的其他商品
            var recommendedProducts = await _dbContext.Products
                .AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.Inventory)
                .Where(p => userCategories.Contains(p.CategoryId) && p.IsPublished)
                .OrderByDescending(p => p.UpdateTime)
                .Take(limit)
                .Select(p => new StoreProductSummaryResult
                {
                    ProductId = p.ProductId,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category.Name,
                    Name = p.Name,
                    Subtitle = p.Subtitle,
                    ThumbnailUrl = p.ThumbnailUrl,
                    Price = p.Price,
                    Currency = p.Currency,
                    ChainId = p.ChainId ?? null,
                    IsPublished = p.IsPublished,
                    InventoryAvailable = p.Inventory != null ? p.Inventory.QuantityAvailable : 0
                })
                .ToListAsync();

            return WrappedResult.Ok(recommendedProducts);
        }

        /// <summary>
        /// 获取最新商品
        /// </summary>
        [HttpGet("latest")]
        [AllowAnonymous]
        public async Task<WrappedResult<List<StoreProductSummaryResult>>> GetLatestProducts([FromQuery] int limit = 10)
        {
            if (limit < 1) limit = 10;
            if (limit > 50) limit = 50;

            var products = await _dbContext.Products
                .AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.Inventory)
                .Where(p => p.IsPublished)
                .OrderByDescending(p => p.CreateTime)
                .Take(limit)
                .Select(p => new StoreProductSummaryResult
                {
                    ProductId = p.ProductId,
                    CategoryId = p.CategoryId,
                    CategoryName = p.Category.Name,
                    Name = p.Name,
                    Subtitle = p.Subtitle,
                    ThumbnailUrl = p.ThumbnailUrl,
                    Price = p.Price,
                    Currency = p.Currency,
                    ChainId = p.ChainId ?? null,
                    IsPublished = p.IsPublished,
                    InventoryAvailable = p.Inventory != null ? p.Inventory.QuantityAvailable : 0
                })
                .ToListAsync();

            return WrappedResult.Ok(products);
        }
    }
}

