using System.ComponentModel.DataAnnotations;

namespace UnifiedPlatform.Shared.ActionModels.Request
{
    /// <summary>
    /// 回复评价请求
    /// </summary>
    public class ReviewReplyRequest
    {
        [Required]
        [StringLength(2000)]
        public string Content { get; set; } = string.Empty;
    }
}

