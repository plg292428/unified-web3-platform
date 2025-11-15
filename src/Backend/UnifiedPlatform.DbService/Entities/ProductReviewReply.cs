using System;

namespace UnifiedPlatform.DbService.Entities
{
    /// <summary>
    /// 商品评价回复（商家回复）
    /// </summary>
    public partial class ProductReviewReply
    {
        /// <summary>
        /// 回复ID
        /// </summary>
        public long ReplyId { get; set; }

        /// <summary>
        /// 评价ID
        /// </summary>
        public long ReviewId { get; set; }

        /// <summary>
        /// 管理员ID（回复人）
        /// </summary>
        public int? ManagerUid { get; set; }

        /// <summary>
        /// 回复内容
        /// </summary>
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateTime { get; set; }

        public virtual ProductReview Review { get; set; } = null!;

        public virtual Manager? Manager { get; set; }
    }
}

