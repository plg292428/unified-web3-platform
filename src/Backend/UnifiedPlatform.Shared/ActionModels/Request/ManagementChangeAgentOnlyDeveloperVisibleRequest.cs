namespace UnifiedPlatform.Shared.ActionModels
{
    public class ManagementChangeAgentOnlyDeveloperVisibleRequest
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public int Uid { get; set; }

        /// <summary>
        /// 是否仅开发者可见
        /// </summary>
        public bool OnlyDeveloperVisible { get; set; }
    }
}

