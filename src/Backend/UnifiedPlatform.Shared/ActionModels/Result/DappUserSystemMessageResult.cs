namespace UnifiedPlatform.Shared.ActionModels
{
    /// <summary>
    /// Dapp 用户系统短消息响应数据
    /// </summary>
    public class DappUserSystemMessageResult
    {
        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public required string Title { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public required string Content { get; set; }

        /// <summary>
        /// 是否为激活码消息
        /// </summary>
        public bool IsActivationCodeMessage { get; set; }

        /// <summary>
        /// 是否已读
        /// </summary>
        public bool IsRead { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}

