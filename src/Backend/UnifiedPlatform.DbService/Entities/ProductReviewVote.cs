using System;

namespace UnifiedPlatform.DbService.Entities
{
    /// <summary>
    /// 商品评价投票（有用/无用）
    /// </summary>
    public partial class ProductReviewVote
    {
        /// <summary>
        /// 投票ID
        /// </summary>
        public long VoteId { get; set; }

        /// <summary>
        /// 评价ID
        /// </summary>
        public long ReviewId { get; set; }

        /// <summary>
        /// 用户ID（投票人）
        /// </summary>
        public int Uid { get; set; }

        /// <summary>
        /// 是否有用（true=有用, false=无用）
        /// </summary>
        public bool IsHelpful { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        public virtual ProductReview Review { get; set; } = null!;

        public virtual User UidNavigation { get; set; } = null!;
    }
}

