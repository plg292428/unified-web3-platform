using HFastKit.AspNetCore.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UnifiedPlatform.DbService.Entities;
using UnifiedPlatform.Shared;
using UnifiedPlatform.Shared.ActionModels.Request;
using UnifiedPlatform.Shared.ActionModels.Result;
using UnifiedPlatform.WebApi.Constants;
using UnifiedPlatform.WebApi.Services;

namespace UnifiedPlatform.WebApi.Controllers
{
    /// <summary>
    /// 库存管理控制器（管理端）
    /// </summary>
    [Consumes("application/json")]
    [Route("[controller]/[action]")]
    [Authorize(Policy = AuthorizationPolicyName.Manager)]
    [ApiController]
    public class InventoryManagementController : ApiControllerBase
    {
        private readonly StDbContext _dbContext;

        public InventoryManagementController(StDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// 更新商品库存
        /// </summary>
        [HttpPut("{productId:long}")]
        public async Task<WrappedResult<InventoryResult>> UpdateInventory(long productId, [FromBody] InventoryUpdateRequest request)
        {
            if (Manager is null)
            {
                return WrappedResult.Failed("无法获取管理员信息");
            }

            var product = await _dbContext.Products
                .Include(p => p.Inventory)
                .FirstOrDefaultAsync(p => p.ProductId == productId);

            if (product is null)
            {
                return WrappedResult.Failed("商品不存在");
            }

            var now = DateTime.UtcNow;

            if (product.Inventory is null)
            {
                // 创建库存记录
                product.Inventory = new ProductInventory
                {
                    ProductId = productId,
                    QuantityAvailable = request.QuantityAvailable ?? 0,
                    QuantityReserved = request.QuantityReserved ?? 0,
                    UpdateTime = now
                };
                await _dbContext.ProductInventories.AddAsync(product.Inventory);
            }
            else
            {
                // 更新库存
                if (request.QuantityAvailable.HasValue)
                {
                    product.Inventory.QuantityAvailable = request.QuantityAvailable.Value;
                }
                if (request.QuantityReserved.HasValue)
                {
                    product.Inventory.QuantityReserved = request.QuantityReserved.Value;
                }
                product.Inventory.UpdateTime = now;
            }

            await _dbContext.SaveChangesAsync();

            var result = new InventoryResult
            {
                ProductId = productId,
                QuantityAvailable = product.Inventory.QuantityAvailable,
                QuantityReserved = product.Inventory.QuantityReserved,
                UpdateTime = product.Inventory.UpdateTime
            };

            return WrappedResult.Ok(result);
        }

        /// <summary>
        /// 调整库存（增加或减少）
        /// </summary>
        [HttpPost("{productId:long}/adjust")]
        public async Task<WrappedResult<InventoryResult>> AdjustInventory(long productId, [FromBody] InventoryAdjustRequest request)
        {
            if (Manager is null)
            {
                return WrappedResult.Failed("无法获取管理员信息");
            }

            var product = await _dbContext.Products
                .Include(p => p.Inventory)
                .FirstOrDefaultAsync(p => p.ProductId == productId);

            if (product is null)
            {
                return WrappedResult.Failed("商品不存在");
            }

            var now = DateTime.UtcNow;

            if (product.Inventory is null)
            {
                // 如果库存不存在，创建新的库存记录
                var initialQuantity = request.Adjustment >= 0 ? request.Adjustment : 0;
                product.Inventory = new ProductInventory
                {
                    ProductId = productId,
                    QuantityAvailable = initialQuantity,
                    QuantityReserved = 0,
                    UpdateTime = now
                };
                await _dbContext.ProductInventories.AddAsync(product.Inventory);
            }
            else
            {
                // 调整库存
                var newQuantity = product.Inventory.QuantityAvailable + request.Adjustment;
                if (newQuantity < 0)
                {
                    return WrappedResult.Failed("库存不足，无法减少");
                }
                product.Inventory.QuantityAvailable = newQuantity;
                product.Inventory.UpdateTime = now;
            }

            await _dbContext.SaveChangesAsync();

            var result = new InventoryResult
            {
                ProductId = productId,
                QuantityAvailable = product.Inventory.QuantityAvailable,
                QuantityReserved = product.Inventory.QuantityReserved,
                UpdateTime = product.Inventory.UpdateTime
            };

            return WrappedResult.Ok(result);
        }

        /// <summary>
        /// 预留库存（订单创建时）
        /// </summary>
        [HttpPost("{productId:long}/reserve")]
        public async Task<WrappedResult<InventoryResult>> ReserveInventory(long productId, [FromBody] InventoryReserveRequest request)
        {
            if (Manager is null)
            {
                return WrappedResult.Failed("无法获取管理员信息");
            }

            var product = await _dbContext.Products
                .Include(p => p.Inventory)
                .FirstOrDefaultAsync(p => p.ProductId == productId);

            if (product is null)
            {
                return WrappedResult.Failed("商品不存在");
            }

            if (product.Inventory is null)
            {
                return WrappedResult.Failed("商品库存不存在");
            }

            var available = product.Inventory.QuantityAvailable - product.Inventory.QuantityReserved;
            if (request.Quantity > available)
            {
                return WrappedResult.Failed($"库存不足，可用数量: {available}");
            }

            var now = DateTime.UtcNow;
            product.Inventory.QuantityReserved += request.Quantity;
            product.Inventory.UpdateTime = now;

            await _dbContext.SaveChangesAsync();

            var result = new InventoryResult
            {
                ProductId = productId,
                QuantityAvailable = product.Inventory.QuantityAvailable,
                QuantityReserved = product.Inventory.QuantityReserved,
                UpdateTime = product.Inventory.UpdateTime
            };

            return WrappedResult.Ok(result);
        }

        /// <summary>
        /// 释放预留库存（订单取消时）
        /// </summary>
        [HttpPost("{productId:long}/release")]
        public async Task<WrappedResult<InventoryResult>> ReleaseInventory(long productId, [FromBody] InventoryReleaseRequest request)
        {
            if (Manager is null)
            {
                return WrappedResult.Failed("无法获取管理员信息");
            }

            var product = await _dbContext.Products
                .Include(p => p.Inventory)
                .FirstOrDefaultAsync(p => p.ProductId == productId);

            if (product is null)
            {
                return WrappedResult.Failed("商品不存在");
            }

            if (product.Inventory is null)
            {
                return WrappedResult.Failed("商品库存不存在");
            }

            if (request.Quantity > product.Inventory.QuantityReserved)
            {
                return WrappedResult.Failed($"释放数量不能大于预留数量，预留数量: {product.Inventory.QuantityReserved}");
            }

            var now = DateTime.UtcNow;
            product.Inventory.QuantityReserved -= request.Quantity;
            product.Inventory.UpdateTime = now;

            await _dbContext.SaveChangesAsync();

            var result = new InventoryResult
            {
                ProductId = productId,
                QuantityAvailable = product.Inventory.QuantityAvailable,
                QuantityReserved = product.Inventory.QuantityReserved,
                UpdateTime = product.Inventory.UpdateTime
            };

            return WrappedResult.Ok(result);
        }

        /// <summary>
        /// 获取低库存商品列表（库存预警）
        /// </summary>
        [HttpGet("low-stock")]
        public async Task<WrappedResult<List<LowStockProductResult>>> GetLowStockProducts([FromQuery] int threshold = 10)
        {
            if (Manager is null)
            {
                return WrappedResult.Failed("无法获取管理员信息");
            }

            var lowStockProducts = await _dbContext.Products
                .AsNoTracking()
                .Include(p => p.Inventory)
                .Include(p => p.Category)
                .Where(p => p.Inventory != null && 
                    (p.Inventory.QuantityAvailable - p.Inventory.QuantityReserved) <= threshold)
                .Select(p => new LowStockProductResult
                {
                    ProductId = p.ProductId,
                    ProductName = p.Name,
                    CategoryName = p.Category.Name,
                    QuantityAvailable = p.Inventory!.QuantityAvailable,
                    QuantityReserved = p.Inventory.QuantityReserved,
                    AvailableQuantity = p.Inventory.QuantityAvailable - p.Inventory.QuantityReserved,
                    Threshold = threshold
                })
                .OrderBy(p => p.AvailableQuantity)
                .ToListAsync();

            return WrappedResult.Ok(lowStockProducts);
        }

        /// <summary>
        /// 获取库存统计
        /// </summary>
        [HttpGet("statistics")]
        public async Task<WrappedResult<InventoryStatisticsResult>> GetInventoryStatistics()
        {
            if (Manager is null)
            {
                return WrappedResult.Failed("无法获取管理员信息");
            }

            var inventories = await _dbContext.ProductInventories
                .AsNoTracking()
                .ToListAsync();

            var statistics = new InventoryStatisticsResult
            {
                TotalProducts = await _dbContext.Products.CountAsync(p => p.IsPublished),
                TotalProductsWithInventory = inventories.Count,
                TotalQuantityAvailable = inventories.Sum(i => i.QuantityAvailable),
                TotalQuantityReserved = inventories.Sum(i => i.QuantityReserved),
                TotalAvailableQuantity = inventories.Sum(i => i.QuantityAvailable - i.QuantityReserved),
                LowStockCount = inventories.Count(i => (i.QuantityAvailable - i.QuantityReserved) <= 10),
                OutOfStockCount = inventories.Count(i => (i.QuantityAvailable - i.QuantityReserved) <= 0)
            };

            return WrappedResult.Ok(statistics);
        }
    }
}

