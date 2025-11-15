using System.Collections.Generic;namespace UnifiedPlatform.Shared.ActionModels.Result;public class StoreProductListResult{    public IReadOnlyList<StoreProductSummaryResult> Items { get; set; } = new List<StoreProductSummaryResult>();    public int TotalCount { get; set; }    public int Page { get; set; }    public int PageSize { get; set; }}

