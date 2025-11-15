using System;

namespace UnifiedPlatform.Shared.ActionModels.Result;

public class StoreProductSummaryResult
{
    public long ProductId { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Subtitle { get; set; }
    public string? ThumbnailUrl { get; set; }
    public decimal Price { get; set; }
    public string Currency { get; set; } = "USDT";
    public int? ChainId { get; set; }
    public bool IsPublished { get; set; }
    public int InventoryAvailable { get; set; }
    public DateTime UpdateTime { get; set; }
}

