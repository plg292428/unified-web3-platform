namespace HFastKit.AspNetCore.Services.Jwt
{
    /// <summary>
    /// Jwt配置
    /// </summary>
    public class JwtOptions
    {
        /// <summary>
        /// 接受方
        /// </summary>
        public string Audience { get; set; } = string.Empty;

        /// <summary>
        /// 签发方
        /// </summary>
        public string Issuer { get; set; } = string.Empty;

        /// <summary>
        /// 私钥
        /// </summary>
        public string SecurityKey { get; set; } = string.Empty;

        public JwtOptions()
        {
        }
    }
}
