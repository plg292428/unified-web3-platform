using System.ComponentModel.DataAnnotations;

namespace UnifiedPlatform.Shared.ActionModels.Request
{
    /// <summary>
    /// 评价投票请求
    /// </summary>
    public class ReviewVoteRequest
    {
        [Range(1, int.MaxValue)]
        public int Uid { get; set; }

        /// <summary>
        /// 是否有用（true=有用, false=无用）
        /// </summary>
        public bool IsHelpful { get; set; }
    }
}

