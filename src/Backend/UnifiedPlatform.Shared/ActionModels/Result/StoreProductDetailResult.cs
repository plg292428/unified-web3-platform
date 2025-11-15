using System;
using System.Collections.Generic;

namespace UnifiedPlatform.Shared.ActionModels.Result;

public class StoreProductDetailResult
{
    public long ProductId { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Subtitle { get; set; }
    public string? Description { get; set; }
    public string? ThumbnailUrl { get; set; }
    public decimal Price { get; set; }
    public string Currency { get; set; } = "USDT";
    public bool IsPublished { get; set; }
    public int InventoryAvailable { get; set; }
    public int InventoryReserved { get; set; }
    public int? ChainId { get; set; }
    public string? Sku { get; set; }
    public double AverageRating { get; set; }
    public int ReviewCount { get; set; }
    public IReadOnlyList<StoreProductImageResult> Images { get; set; } = Array.Empty<StoreProductImageResult>();
    public IReadOnlyList<StoreProductSpecificationResult> Specifications { get; set; } = Array.Empty<StoreProductSpecificationResult>();
    public DateTime CreateTime { get; set; }
    public DateTime UpdateTime { get; set; }
    public IReadOnlyList<StoreProductCategoryBreadcrumbResult> Breadcrumb { get; set; } = Array.Empty<StoreProductCategoryBreadcrumbResult>();
}

public class StoreProductCategoryBreadcrumbResult
{
    public int CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Slug { get; set; }
}

