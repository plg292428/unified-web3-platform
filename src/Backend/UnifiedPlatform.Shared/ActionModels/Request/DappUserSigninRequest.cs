using System.ComponentModel.DataAnnotations;

namespace UnifiedPlatform.Shared.ActionModels
{
    /// <summary>
    /// Dapp 用户登入请求
    /// </summary>
    public class DappUserSigninRequest : DappUserWalletRequest
    {
        /// <summary>
        /// 邀请者链接令牌
        /// </summary>
        public string? InvitationLinkToken { get; set; }

        /// <summary>
        /// 签名文本
        /// </summary>
        [Required(ErrorMessage = "Invalid request parameter")]
        public string? SignedText { get; set; }
    }
}

