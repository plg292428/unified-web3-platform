using System;

namespace UnifiedPlatform.DbService.Entities
{
    /// <summary>
    /// 商品规格
    /// </summary>
    public partial class ProductSpecification
    {
        /// <summary>
        /// 规格ID
        /// </summary>
        public long SpecificationId { get; set; }

        /// <summary>
        /// 商品ID
        /// </summary>
        public long ProductId { get; set; }

        /// <summary>
        /// 规格名称（如：颜色、尺寸）
        /// </summary>
        public string SpecificationName { get; set; } = null!;

        /// <summary>
        /// 规格值（如：红色、XL）
        /// </summary>
        public string SpecificationValue { get; set; } = null!;

        /// <summary>
        /// 价格调整（相对于基础价格）
        /// </summary>
        public decimal PriceAdjustment { get; set; }

        /// <summary>
        /// 库存数量（如果规格有独立库存）
        /// </summary>
        public int? StockQuantity { get; set; }

        /// <summary>
        /// 排序顺序
        /// </summary>
        public int SortOrder { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; }

        public virtual Product Product { get; set; } = null!;
    }
}

