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
    /// 商品管理控制器（管理端）
    /// </summary>
    [Consumes("application/json")]
    [Route("[controller]/[action]")]
    [Authorize(Policy = AuthorizationPolicyName.Manager)]
    [ApiController]
    public class ProductManagementController : ApiControllerBase
    {
        private readonly StDbContext _dbContext;
        private readonly ITempCaching _tempCaching;

        public ProductManagementController(StDbContext dbContext, ITempCaching tempCaching)
        {
            _dbContext = dbContext;
            _tempCaching = tempCaching;
        }

        /// <summary>
        /// 创建商品
        /// </summary>
        [HttpPost]
        public async Task<WrappedResult<StoreProductDetailResult>> CreateProduct([FromBody] ProductCreateRequest request)
        {
            if (Manager is null)
            {
                return WrappedResult.Failed("无法获取管理员信息");
            }

            // 验证分类是否存在
            var category = await _dbContext.ProductCategories
                .FirstOrDefaultAsync(c => c.CategoryId == request.CategoryId && c.IsActive);
            if (category is null)
            {
                return WrappedResult.Failed("商品分类不存在或已禁用");
            }

            // 验证链ID（如果提供）
            if (request.ChainId.HasValue)
            {
                var chainExists = _tempCaching.ChainNetworkConfigs.Any(c => c.ChainId == request.ChainId.Value);
                if (!chainExists)
                {
                    return WrappedResult.Failed("不支持的区块链网络");
                }
            }

            var now = DateTime.UtcNow;

            var product = new Product
            {
                CategoryId = request.CategoryId,
                Name = request.Name,
                Subtitle = request.Subtitle,
                Description = request.Description,
                ThumbnailUrl = request.ThumbnailUrl,
                Price = request.Price,
                Currency = request.Currency ?? "USDT",
                ChainId = request.ChainId,
                Sku = request.Sku,
                IsPublished = request.IsPublished ?? false,
                CreateTime = now,
                UpdateTime = now
            };

            await _dbContext.Products.AddAsync(product);
            await _dbContext.SaveChangesAsync();

            // 创建库存记录
            if (request.InitialStock.HasValue)
            {
                var inventory = new ProductInventory
                {
                    ProductId = product.ProductId,
                    QuantityAvailable = request.InitialStock.Value,
                    QuantityReserved = 0,
                    UpdateTime = now
                };
                await _dbContext.ProductInventories.AddAsync(inventory);
            }

            // 添加商品图片
            if (request.Images != null && request.Images.Any())
            {
                var productImages = request.Images.Select((img, index) => new ProductImage
                {
                    ProductId = product.ProductId,
                    ImageUrl = img.Url,
                    ImageType = img.Type ?? "gallery",
                    SortOrder = img.SortOrder ?? index,
                    IsPrimary = img.IsPrimary ?? (index == 0),
                    CreateTime = now
                }).ToList();
                await _dbContext.ProductImages.AddRangeAsync(productImages);
            }

            // 添加商品规格
            if (request.Specifications != null && request.Specifications.Any())
            {
                var specifications = request.Specifications.Select((spec, index) => new ProductSpecification
                {
                    ProductId = product.ProductId,
                    SpecificationName = spec.Name,
                    SpecificationValue = spec.Value,
                    PriceAdjustment = spec.PriceAdjustment ?? 0,
                    StockQuantity = spec.StockQuantity,
                    SortOrder = spec.SortOrder ?? index,
                    IsEnabled = spec.IsEnabled ?? true,
                    CreateTime = now,
                    UpdateTime = now
                }).ToList();
                await _dbContext.ProductSpecifications.AddRangeAsync(specifications);
            }

            await _dbContext.SaveChangesAsync();

            // 返回商品详情
            var result = await GetProductDetailInternal(product.ProductId);
            return WrappedResult.Ok(result);
        }

        /// <summary>
        /// 更新商品
        /// </summary>
        [HttpPut("{productId:long}")]
        public async Task<WrappedResult<StoreProductDetailResult>> UpdateProduct(
            long productId,
            [FromBody] ProductUpdateRequest request)
        {
            if (Manager is null)
            {
                return WrappedResult.Failed("无法获取管理员信息");
            }

            var product = await _dbContext.Products
                .FirstOrDefaultAsync(p => p.ProductId == productId);

            if (product is null)
            {
                return WrappedResult.Failed("商品不存在");
            }

            // 更新基本信息
            if (request.CategoryId.HasValue)
            {
                var category = await _dbContext.ProductCategories
                    .FirstOrDefaultAsync(c => c.CategoryId == request.CategoryId.Value && c.IsActive);
                if (category is null)
                {
                    return WrappedResult.Failed("商品分类不存在或已禁用");
                }
                product.CategoryId = request.CategoryId.Value;
            }

            if (!string.IsNullOrWhiteSpace(request.Name))
                product.Name = request.Name;
            if (request.Subtitle != null)
                product.Subtitle = request.Subtitle;
            if (request.Description != null)
                product.Description = request.Description;
            if (request.ThumbnailUrl != null)
                product.ThumbnailUrl = request.ThumbnailUrl;
            if (request.Price.HasValue)
                product.Price = request.Price.Value;
            if (!string.IsNullOrWhiteSpace(request.Currency))
                product.Currency = request.Currency;
            if (request.ChainId.HasValue)
            {
                var chainExists = _tempCaching.ChainNetworkConfigs.Any(c => c.ChainId == request.ChainId.Value);
                if (chainExists)
                {
                    product.ChainId = request.ChainId.Value;
                }
            }
            if (!string.IsNullOrWhiteSpace(request.Sku))
                product.Sku = request.Sku;
            if (request.IsPublished.HasValue)
                product.IsPublished = request.IsPublished.Value;

            product.UpdateTime = DateTime.UtcNow;

            // 更新库存
            if (request.StockQuantity.HasValue)
            {
                var inventory = await _dbContext.ProductInventories
                    .FirstOrDefaultAsync(i => i.ProductId == productId);
                if (inventory is null)
                {
                    inventory = new ProductInventory
                    {
                        ProductId = productId,
                        QuantityAvailable = request.StockQuantity.Value,
                        QuantityReserved = 0,
                        UpdateTime = DateTime.UtcNow
                    };
                    await _dbContext.ProductInventories.AddAsync(inventory);
                }
                else
                {
                    inventory.QuantityAvailable = request.StockQuantity.Value;
                    inventory.UpdateTime = DateTime.UtcNow;
                }
            }

            _dbContext.Products.Update(product);
            await _dbContext.SaveChangesAsync();

            var result = await GetProductDetailInternal(productId);
            return WrappedResult.Ok(result);
        }

        /// <summary>
        /// 删除商品
        /// </summary>
        [HttpDelete("{productId:long}")]
        public async Task<WrappedResult> DeleteProduct(long productId)
        {
            if (Manager is null)
            {
                return WrappedResult.Failed("无法获取管理员信息");
            }

            var product = await _dbContext.Products
                .Include(p => p.OrderItems)
                .FirstOrDefaultAsync(p => p.ProductId == productId);

            if (product is null)
            {
                return WrappedResult.Failed("商品不存在");
            }

            // 检查是否有订单关联
            if (product.OrderItems.Any())
            {
                // 如果有订单，只标记为未发布，不删除
                product.IsPublished = false;
                product.UpdateTime = DateTime.UtcNow;
                _dbContext.Products.Update(product);
            }
            else
            {
                // 没有订单，可以删除
                _dbContext.Products.Remove(product);
            }

            await _dbContext.SaveChangesAsync();
            return WrappedResult.Ok();
        }

        /// <summary>
        /// 获取商品详情（内部方法）
        /// </summary>
        private async Task<StoreProductDetailResult> GetProductDetailInternal(long productId)
        {
            var product = await _dbContext.Products
                .Include(p => p.Category)
                .Include(p => p.Inventory)
                .Include(p => p.ProductImages.OrderBy(img => img.SortOrder))
                .Include(p => p.ProductSpecifications.OrderBy(spec => spec.SortOrder))
                .Include(p => p.ProductReviews.Where(r => r.IsVisible && r.IsApproved))
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.ProductId == productId);

            if (product is null)
            {
                throw new Exception("商品不存在");
            }

            var averageRating = product.ProductReviews.Any()
                ? product.ProductReviews.Average(r => r.Rating)
                : 0;

            var reviewCount = product.ProductReviews.Count;

            return new StoreProductDetailResult
            {
                ProductId = product.ProductId,
                CategoryId = product.CategoryId,
                CategoryName = product.Category.Name,
                Name = product.Name,
                Subtitle = product.Subtitle,
                Description = product.Description,
                ThumbnailUrl = product.ThumbnailUrl,
                Price = product.Price,
                Currency = product.Currency,
                ChainId = product.ChainId,
                Sku = product.Sku,
                IsPublished = product.IsPublished,
                InventoryAvailable = product.Inventory?.QuantityAvailable ?? 0,
                InventoryReserved = product.Inventory?.QuantityReserved ?? 0,
                AverageRating = Math.Round((double)averageRating, 1),
                ReviewCount = reviewCount,
                Images = product.ProductImages.Select(img => new StoreProductImageResult
                {
                    ImageId = img.ImageId,
                    ImageUrl = img.ImageUrl,
                    ImageType = img.ImageType,
                    SortOrder = img.SortOrder,
                    IsPrimary = img.IsPrimary
                }).ToList(),
                Specifications = product.ProductSpecifications.Select(spec => new StoreProductSpecificationResult
                {
                    SpecificationId = spec.SpecificationId,
                    SpecificationName = spec.SpecificationName,
                    SpecificationValue = spec.SpecificationValue,
                    PriceAdjustment = spec.PriceAdjustment,
                    StockQuantity = spec.StockQuantity,
                    SortOrder = spec.SortOrder,
                    IsEnabled = spec.IsEnabled
                }).ToList(),
                CreateTime = product.CreateTime,
                UpdateTime = product.UpdateTime
            };
        }
    }
}

