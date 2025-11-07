namespace SmallTarget.WebApi.Services
{
    /// <summary>
    /// 签名令牌验证结果
    /// </summary>
    public class SignTokenVerifyResult
    {
        /// <summary>
        /// 是否验证成功
        /// </summary>
        public bool IsVerified { get; set; } = false;

        /// <summary>
        /// 登录Guid
        /// </summary>
        public Guid? Guid { get; set; }

        /// <summary>
        /// 错误消息
        /// </summary>
        public string? ErrorMessage { get; set; }
    }
}
