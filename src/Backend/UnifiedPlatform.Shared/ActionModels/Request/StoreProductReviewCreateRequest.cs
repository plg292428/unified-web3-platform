using System.ComponentModel.DataAnnotations;

namespace UnifiedPlatform.Shared.ActionModels.Request
{
    /// <summary>
    /// 创建商品评价请求
    /// </summary>
    public class StoreProductReviewCreateRequest
    {
        [Range(1, int.MaxValue)]
        public int Uid { get; set; }

        [Range(1, long.MaxValue)]
        public long ProductId { get; set; }

        [Range(1, long.MaxValue)]
        public long? OrderId { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        [StringLength(2000)]
        public string? Content { get; set; }

        /// <summary>
        /// 评价图片URL列表
        /// </summary>
        public List<string>? ImageUrls { get; set; }
    }
}

