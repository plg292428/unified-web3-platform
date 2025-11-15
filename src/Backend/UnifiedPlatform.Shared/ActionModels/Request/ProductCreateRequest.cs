namespace UnifiedPlatform.Shared.ActionModels.Request
{
    /// <summary>
    /// 创建商品请求
    /// </summary>
    public class ProductCreateRequest
    {
        /// <summary>
        /// 分类ID
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string Name { get; set; } = string.Empty;

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
        public decimal Price { get; set; }

        /// <summary>
        /// 货币（默认USDT）
        /// </summary>
        public string? Currency { get; set; }

        /// <summary>
        /// 链ID（可选）
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
        /// 初始库存
        /// </summary>
        public int? InitialStock { get; set; }

        /// <summary>
        /// 商品图片列表
        /// </summary>
        public List<ProductImageRequest>? Images { get; set; }

        /// <summary>
        /// 商品规格列表
        /// </summary>
        public List<ProductSpecificationRequest>? Specifications { get; set; }
    }

    /// <summary>
    /// 商品图片请求
    /// </summary>
    public class ProductImageRequest
    {
        /// <summary>
        /// 图片URL
        /// </summary>
        public string Url { get; set; } = string.Empty;

        /// <summary>
        /// 图片类型（thumbnail, gallery, detail）
        /// </summary>
        public string? Type { get; set; }

        /// <summary>
        /// 排序顺序
        /// </summary>
        public int? SortOrder { get; set; }

        /// <summary>
        /// 是否为主图
        /// </summary>
        public bool? IsPrimary { get; set; }
    }

    /// <summary>
    /// 商品规格请求
    /// </summary>
    public class ProductSpecificationRequest
    {
        /// <summary>
        /// 规格名称（如：颜色、尺寸）
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 规格值（如：红色、XL）
        /// </summary>
        public string Value { get; set; } = string.Empty;

        /// <summary>
        /// 价格调整（相对于基础价格）
        /// </summary>
        public decimal? PriceAdjustment { get; set; }

        /// <summary>
        /// 库存数量（如果规格有独立库存）
        /// </summary>
        public int? StockQuantity { get; set; }

        /// <summary>
        /// 排序顺序
        /// </summary>
        public int? SortOrder { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool? IsEnabled { get; set; }
    }
}

