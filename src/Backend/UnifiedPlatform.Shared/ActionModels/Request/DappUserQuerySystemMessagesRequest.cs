using HFastKit.AspNetCore.Shared;

namespace UnifiedPlatform.Shared.ActionModels
{
    /// <summary>
    /// Dapp 查询用户系统短消息请求
    /// </summary>
    public class DappUserQuerySystemMessagesRequest : QueryByPagingRequest
    {
        /// <summary>
        /// 仅未读的
        /// </summary>
        public bool? OnlyUnread { get; set; }
    }
}

