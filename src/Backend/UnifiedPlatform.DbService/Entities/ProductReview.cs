using System;

namespace UnifiedPlatform.DbService.Entities
{
    /// <summary>
    /// 商品评价
    /// </summary>
    public partial class ProductReview
    {
        /// <summary>
        /// 评价ID
        /// </summary>
        public long ReviewId { get; set; }

        /// <summary>
        /// 商品ID
        /// </summary>
        public long ProductId { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public int Uid { get; set; }

        /// <summary>
        /// 订单ID（可选，用于验证购买）
        /// </summary>
        public long? OrderId { get; set; }

        /// <summary>
        /// 评分（1-5星）
        /// </summary>
        public int Rating { get; set; }

        /// <summary>
        /// 评价内容
        /// </summary>
        public string? Content { get; set; }

        /// <summary>
        /// 是否已审核
        /// </summary>
        public bool IsApproved { get; set; }

        /// <summary>
        /// 是否显示
        /// </summary>
        public bool IsVisible { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; }

        public virtual Product Product { get; set; } = null!;

        public virtual User UidNavigation { get; set; } = null!;

        public virtual Order? Order { get; set; }

        public virtual ICollection<ProductReviewImage> ReviewImages { get; set; } = new List<ProductReviewImage>();

        public virtual ICollection<ProductReviewReply> Replies { get; set; } = new List<ProductReviewReply>();

        public virtual ICollection<ProductReviewVote> Votes { get; set; } = new List<ProductReviewVote>();
    }
}

