namespace SmallTarget.WebApi.Services
{
    /// <summary>
    /// 登录令牌
    /// </summary>
    public class SignToken
    {
        /// <summary>
        /// 唯一标识
        /// </summary>
        public Guid Guid { get; private set; }

        /// <summary>
        /// 十六进制文本
        /// </summary>
        public string GuidText => Guid.ToString();

        public string SignatureContent => $"Dear user,\r\n\r\nTo ensure the security of your account, please perform signature verification.\r\n\r\nSignature content: {GuidText}";

        /// <summary>
        /// 过期时间
        /// </summary>
        public DateTime ExpirationTime { get; private set; }

        /// <summary>
        /// 登录令牌
        /// </summary>
        /// <param name="expirationTime">过期时间</param>
        public SignToken(TimeSpan expirationTime)
        {
            Guid = Guid.NewGuid();
            ExpirationTime = DateTime.UtcNow + expirationTime;
        }
    }
}
