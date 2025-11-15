using System;

namespace UnifiedPlatform.DbService.Entities
{
    /// <summary>
    /// 商品图片
    /// </summary>
    public partial class ProductImage
    {
        /// <summary>
        /// 图片ID
        /// </summary>
        public long ImageId { get; set; }

        /// <summary>
        /// 商品ID
        /// </summary>
        public long ProductId { get; set; }

        /// <summary>
        /// 图片URL
        /// </summary>
        public string ImageUrl { get; set; } = null!;

        /// <summary>
        /// 图片类型（thumbnail, gallery, detail）
        /// </summary>
        public string ImageType { get; set; } = "gallery";

        /// <summary>
        /// 排序顺序
        /// </summary>
        public int SortOrder { get; set; }

        /// <summary>
        /// 是否为主图
        /// </summary>
        public bool IsPrimary { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        public virtual Product Product { get; set; } = null!;
    }
}

