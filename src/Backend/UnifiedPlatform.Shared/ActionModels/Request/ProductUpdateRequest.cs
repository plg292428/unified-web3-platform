namespace UnifiedPlatform.Shared.ActionModels.Request
{
    /// <summary>
    /// 更新商品请求
    /// </summary>
    public class ProductUpdateRequest
    {
        /// <summary>
        /// 分类ID
        /// </summary>
        public int? CategoryId { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 副标题
        /// </summary>
        public string? Subtitle { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// 缩略图URL
        /// </summary>
        public string? ThumbnailUrl { get; set; }

        /// <summary>
        /// 价格
        /// </summary>
        public decimal? Price { get; set; }

        /// <summary>
        /// 货币
        /// </summary>
        public string? Currency { get; set; }

        /// <summary>
        /// 链ID
        /// </summary>
        public int? ChainId { get; set; }

        /// <summary>
        /// SKU
        /// </summary>
        public string? Sku { get; set; }

        /// <summary>
        /// 是否发布
        /// </summary>
        public bool? IsPublished { get; set; }

        /// <summary>
        /// 库存数量
        /// </summary>
        public int? StockQuantity { get; set; }
    }
}

