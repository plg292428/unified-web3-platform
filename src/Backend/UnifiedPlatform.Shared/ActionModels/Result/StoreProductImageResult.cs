namespace UnifiedPlatform.Shared.ActionModels.Result
{
    /// <summary>
    /// 商品图片结果
    /// </summary>
    public class StoreProductImageResult
    {
        public long ImageId { get; set; }

        public long ProductId { get; set; }

        public string ImageUrl { get; set; } = null!;

        public string ImageType { get; set; } = "gallery";

        public int SortOrder { get; set; }

        public bool IsPrimary { get; set; }

        public DateTime CreateTime { get; set; }
    }
}

