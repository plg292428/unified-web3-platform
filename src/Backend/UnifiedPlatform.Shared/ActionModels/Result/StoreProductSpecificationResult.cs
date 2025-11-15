namespace UnifiedPlatform.Shared.ActionModels.Result
{
    /// <summary>
    /// 商品规格结果
    /// </summary>
    public class StoreProductSpecificationResult
    {
        public long SpecificationId { get; set; }

        public long ProductId { get; set; }

        public string SpecificationName { get; set; } = null!;

        public string SpecificationValue { get; set; } = null!;

        public decimal PriceAdjustment { get; set; }

        public int? StockQuantity { get; set; }

        public int SortOrder { get; set; }

        public bool IsEnabled { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime UpdateTime { get; set; }
    }

    /// <summary>
    /// 商品规格分组结果（按规格名称分组）
    /// </summary>
    public class StoreProductSpecificationGroupResult
    {
        public string SpecificationName { get; set; } = null!;

        public List<StoreProductSpecificationResult> Values { get; set; } = new();
    }
}

