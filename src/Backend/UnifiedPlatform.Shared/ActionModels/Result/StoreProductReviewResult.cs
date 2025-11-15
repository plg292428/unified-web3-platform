using System.Collections.Generic;

namespace UnifiedPlatform.Shared.ActionModels.Result
{
    /// <summary>
    /// 商品评价结果
    /// </summary>
    public class StoreProductReviewResult
    {
        public long ReviewId { get; set; }

        public long ProductId { get; set; }

        public int Uid { get; set; }

        public string? UserWalletAddress { get; set; }

        public long? OrderId { get; set; }

        public int Rating { get; set; }

        public string? Content { get; set; }

        public bool IsApproved { get; set; }

        public bool IsVisible { get; set; }

        /// <summary>
        /// 评价图片列表
        /// </summary>
        public List<ReviewImageResult> Images { get; set; } = new();

        /// <summary>
        /// 商家回复列表
        /// </summary>
        public List<ReviewReplyResult> Replies { get; set; } = new();

        /// <summary>
        /// 有用投票数
        /// </summary>
        public int HelpfulCount { get; set; }

        /// <summary>
        /// 无用投票数
        /// </summary>
        public int NotHelpfulCount { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime UpdateTime { get; set; }
    }

    /// <summary>
    /// 评价图片结果
    /// </summary>
    public class ReviewImageResult
    {
        public long ImageId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public int SortOrder { get; set; }
    }

    /// <summary>
    /// 评价回复结果
    /// </summary>
    public class ReviewReplyResult
    {
        public long ReplyId { get; set; }
        public int? ManagerUid { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreateTime { get; set; }
    }

    /// <summary>
    /// 商品评价列表结果
    /// </summary>
    public class StoreProductReviewListResult
    {
        public List<StoreProductReviewResult> Items { get; set; } = new();

        public int TotalCount { get; set; }

        public double AverageRating { get; set; }

        public Dictionary<int, int> RatingDistribution { get; set; } = new();
    }
}

