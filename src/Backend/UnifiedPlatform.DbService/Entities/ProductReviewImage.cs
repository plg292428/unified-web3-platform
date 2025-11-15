using System;

namespace UnifiedPlatform.DbService.Entities
{
    /// <summary>
    /// 商品评价图片
    /// </summary>
    public partial class ProductReviewImage
    {
        /// <summary>
        /// 图片ID
        /// </summary>
        public long ImageId { get; set; }

        /// <summary>
        /// 评价ID
        /// </summary>
        public long ReviewId { get; set; }

        /// <summary>
        /// 图片URL
        /// </summary>
        public string ImageUrl { get; set; } = string.Empty;

        /// <summary>
        /// 排序顺序
        /// </summary>
        public int SortOrder { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        public virtual ProductReview Review { get; set; } = null!;
    }
}

