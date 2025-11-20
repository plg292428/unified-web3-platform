namespace UnifiedPlatform.WebApi.Controllers;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HFastKit.AspNetCore.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UnifiedPlatform.DbService.Entities;
using UnifiedPlatform.Shared;
using UnifiedPlatform.Shared.ActionModels.Request;
using UnifiedPlatform.Shared.ActionModels.Result;

[AllowAnonymous]
[Route("api/store")]
public class StoreController : ApiControllerBase
{
    private readonly StDbContext _dbContext;

    public StoreController(StDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// 获取分类树结构
    /// </summary>
    [HttpGet("categories")]
    public async Task<WrappedResult<IReadOnlyList<StoreProductCategoryResult>>> CategoriesAsync()
    {
        var categories = await _dbContext.ProductCategories
            .AsNoTracking()
            .Where(c => c.IsActive)
            .OrderBy(c => c.SortOrder)
            .ThenBy(c => c.CategoryId)
            .ToListAsync();

        var lookup = categories.ToLookup(c => c.ParentCategoryId);

        List<StoreProductCategoryResult> Build(int? parentId)
        {
            return lookup[parentId]
                .OrderBy(c => c.SortOrder)
                .ThenBy(c => c.CategoryId)
                .Select(c => new StoreProductCategoryResult
                {
                    CategoryId = c.CategoryId,
                    Name = c.Name,
                    Slug = c.Slug,
                    Description = c.Description,
                    ParentCategoryId = c.ParentCategoryId,
                    SortOrder = c.SortOrder,
                    IsActive = c.IsActive,
                    Children = Build(c.CategoryId)
                })
                .ToList();
        }

        var result = (IReadOnlyList<StoreProductCategoryResult>)Build(null);
        return WrappedResult.Ok(result);
    }

    /// <summary>
    /// 分页获取商品列表
    /// </summary>
    [HttpGet("products")]
    public async Task<WrappedResult<StoreProductListResult>> ProductsAsync([FromQuery] StoreProductListRequest request)
    {
        request.Normalize();

        var query = _dbContext.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .Include(p => p.Inventory)
            .Where(p => p.IsPublished);

        if (request.CategoryId.HasValue)
        {
            var categoryId = request.CategoryId.Value;
            var descendantIds = await GetCategoryDescendantIdsAsync(categoryId);
            descendantIds.Add(categoryId);
            query = query.Where(p => descendantIds.Contains(p.CategoryId));
        }

        // 关键词搜索（支持名称、副标题、描述）
        if (!string.IsNullOrWhiteSpace(request.Keyword))
        {
            string keyword = request.Keyword.Trim();
            string pattern = $"%{keyword}%";
            query = query.Where(p =>
                EF.Functions.Like(p.Name, pattern) ||
                EF.Functions.Like(p.Subtitle ?? string.Empty, pattern) ||
                EF.Functions.Like(p.Description ?? string.Empty, pattern));
        }

        // 链ID筛选
        if (request.ChainId.HasValue)
        {
            int chainId = request.ChainId.Value;
            query = query.Where(p => p.ChainId == chainId);
        }

        // 价格范围筛选
        if (request.MinPrice.HasValue)
        {
            query = query.Where(p => p.Price >= request.MinPrice.Value);
        }

        if (request.MaxPrice.HasValue)
        {
            query = query.Where(p => p.Price <= request.MaxPrice.Value);
        }

        int totalCount = await query.CountAsync();

        // 排序处理
        IOrderedQueryable<Product> orderedQuery;
        switch (request.SortBy?.ToLower())
        {
            case "price_asc":
                orderedQuery = query.OrderBy(p => p.Price).ThenByDescending(p => p.ProductId);
                break;
            case "price_desc":
                orderedQuery = query.OrderByDescending(p => p.Price).ThenByDescending(p => p.ProductId);
                break;
            case "time_asc":
                orderedQuery = query.OrderBy(p => p.UpdateTime).ThenByDescending(p => p.ProductId);
                break;
            case "name_asc":
                orderedQuery = query.OrderBy(p => p.Name).ThenByDescending(p => p.ProductId);
                break;
            case "name_desc":
                orderedQuery = query.OrderByDescending(p => p.Name).ThenByDescending(p => p.ProductId);
                break;
            case "time_desc":
            default:
                orderedQuery = query.OrderByDescending(p => p.UpdateTime).ThenByDescending(p => p.ProductId);
                break;
        }

        var items = await orderedQuery
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
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
                IsPublished = p.IsPublished,
                InventoryAvailable = p.Inventory != null
                    ? p.Inventory.QuantityAvailable - p.Inventory.QuantityReserved
                    : 0,
                UpdateTime = p.UpdateTime
            })
            .ToListAsync();

        StoreProductListResult result = new()
        {
            Items = items,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };

        return WrappedResult.Ok(result);
    }

    /// <summary>
    /// 获取商品图片列表
    /// </summary>
    [HttpGet("products/{productId:long}/images")]
    public async Task<WrappedResult<List<StoreProductImageResult>>> ProductImagesAsync(long productId)
        {
            var images = await _dbContext.ProductImages
                .AsNoTracking()
                .Where(img => img.ProductId == productId)
                .OrderBy(img => img.SortOrder)
                .ThenBy(img => img.ImageId)
                .Select(img => new StoreProductImageResult
                {
                    ImageId = img.ImageId,
                    ProductId = img.ProductId,
                    ImageUrl = img.ImageUrl,
                    ImageType = img.ImageType,
                    SortOrder = img.SortOrder,
                    IsPrimary = img.IsPrimary,
                    CreateTime = img.CreateTime
                })
                .ToListAsync();

        return WrappedResult.Ok(images);
    }

    /// <summary>
    /// 获取商品规格列表
    /// </summary>
    [HttpGet("products/{productId:long}/specifications")]
    public async Task<WrappedResult<List<StoreProductSpecificationGroupResult>>> ProductSpecificationsAsync(long productId)
        {
            var specifications = await _dbContext.ProductSpecifications
                .AsNoTracking()
                .Where(spec => spec.ProductId == productId && spec.IsEnabled)
                .OrderBy(spec => spec.SpecificationName)
                .ThenBy(spec => spec.SortOrder)
                .ThenBy(spec => spec.SpecificationId)
                .Select(spec => new StoreProductSpecificationResult
                {
                    SpecificationId = spec.SpecificationId,
                    ProductId = spec.ProductId,
                    SpecificationName = spec.SpecificationName,
                    SpecificationValue = spec.SpecificationValue,
                    PriceAdjustment = spec.PriceAdjustment,
                    StockQuantity = spec.StockQuantity,
                    SortOrder = spec.SortOrder,
                    IsEnabled = spec.IsEnabled,
                    CreateTime = spec.CreateTime,
                    UpdateTime = spec.UpdateTime
                })
                .ToListAsync();

            // 按规格名称分组
            var grouped = specifications
                .GroupBy(spec => spec.SpecificationName)
                .Select(g => new StoreProductSpecificationGroupResult
                {
                    SpecificationName = g.Key,
                    Values = g.ToList()
                })
                .ToList();

        return WrappedResult.Ok(grouped);
    }

    /// <summary>
    /// 获取商品详情
    /// </summary>
    [HttpGet("products/{productId:long}")]
    public async Task<WrappedResult<StoreProductDetailResult>> ProductDetailAsync(long productId)
    {
        try
        {
            // 使用 Select 投影，避免加载 Chain 导航属性，直接查询需要的字段
            var productData = await _dbContext.Products
                .AsNoTracking()
                .Where(p => p.ProductId == productId && p.IsPublished)
                .Select(p => new
                {
                    p.ProductId,
                    p.CategoryId,
                    CategoryName = p.Category != null ? p.Category.Name : "未分类",
                    p.Name,
                    p.Subtitle,
                    p.Description,
                    p.ThumbnailUrl,
                    p.Price,
                    p.Currency,
                    p.IsPublished,
                    InventoryAvailable = p.Inventory != null 
                        ? p.Inventory.QuantityAvailable - p.Inventory.QuantityReserved 
                        : 0,
                    InventoryReserved = p.Inventory != null ? p.Inventory.QuantityReserved : 0,
                    p.ChainId,
                    p.Sku,
                    p.CreateTime,
                    p.UpdateTime
                })
                .FirstOrDefaultAsync();

            if (productData is null)
            {
                return WrappedResult.Failed("商品不存在或未上架");
            }

            // 构建分类面包屑
            var categoryMap = await _dbContext.ProductCategories
                .AsNoTracking()
                .ToDictionaryAsync(c => c.CategoryId);

            List<StoreProductCategoryBreadcrumbResult> breadcrumb = new();
            if (categoryMap.TryGetValue(productData.CategoryId, out var current))
            {
                while (current != null)
                {
                    breadcrumb.Insert(0, new StoreProductCategoryBreadcrumbResult
                    {
                        CategoryId = current.CategoryId,
                        Name = current.Name,
                        Slug = current.Slug
                    });

                    if (current.ParentCategoryId.HasValue && categoryMap.TryGetValue(current.ParentCategoryId.Value, out var parent))
                    {
                        current = parent;
                    }
                    else
                    {
                        current = null;
                    }
                }
            }

            var detail = new StoreProductDetailResult
            {
                ProductId = productData.ProductId,
                CategoryId = productData.CategoryId,
                CategoryName = productData.CategoryName,
                Name = productData.Name,
                Subtitle = productData.Subtitle,
                Description = productData.Description,
                ThumbnailUrl = productData.ThumbnailUrl,
                Price = productData.Price,
                Currency = productData.Currency,
                IsPublished = productData.IsPublished,
                InventoryAvailable = productData.InventoryAvailable,
                InventoryReserved = productData.InventoryReserved,
                ChainId = productData.ChainId,
                Sku = productData.Sku,
                CreateTime = productData.CreateTime,
                UpdateTime = productData.UpdateTime,
                Breadcrumb = breadcrumb
            };

            return WrappedResult.Ok(detail);
        }
        catch (Exception ex)
        {
            // 记录错误日志（可以通过 ApiControllerBase 的日志功能记录）
            return WrappedResult.Failed($"获取商品详情失败: {ex.Message}");
        }
    }

    private async Task<HashSet<int>> GetCategoryDescendantIdsAsync(int categoryId)
    {
        var descendants = new HashSet<int>();

        var allCategories = await _dbContext.ProductCategories
            .AsNoTracking()
            .Where(c => c.IsActive)
            .Select(c => new { c.CategoryId, c.ParentCategoryId })
            .ToListAsync();

        bool added;
        do
        {
            added = false;
            foreach (var category in allCategories)
            {
                if (category.ParentCategoryId.HasValue &&
                    (category.ParentCategoryId.Value == categoryId || descendants.Contains(category.ParentCategoryId.Value)) &&
                    descendants.Add(category.CategoryId))
                {
                    added = true;
                }
            }
        }
        while (added);

        return descendants;
    }
}

