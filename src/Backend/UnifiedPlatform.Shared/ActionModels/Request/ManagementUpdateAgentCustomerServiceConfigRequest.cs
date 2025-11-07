using System.ComponentModel.DataAnnotations;

namespace SmallTarget.Shared.ActionModels
{
    /// <summary>
    /// 管理端更新代理客服配置请求
    /// </summary>
    public class ManagementUpdateAgentCustomerServiceConfigRequest
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public int Uid { get; set; }

        /// <summary>
        /// 是否开启客服系统
        /// </summary>
        public bool OnlineCustomerServiceEnabled { get; set; }

        /// <summary>
        /// 客服链接
        /// </summary>
        public string? OnlineCustomerServiceChatWootKey { get; set; }
    }
}
