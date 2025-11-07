using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.Versioning;
using System.Security.Claims;
using System.Text;

namespace HFastKit.AspNetCore.Shared
{
    /// <summary>
    /// JWT 帮助
    /// </summary>
    public static class JwtHelper
    {
        /// <summary>
        /// 生成令牌
        /// </summary>
        /// <returns>令牌</returns>
        public static string CreateToken(string audience, string issuer, string secretKey, Claim[] claims, int expirationDays = 30)
        {
            if (expirationDays < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(expirationDays));
            }

            // 私钥和加密算法
            SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(secretKey));
            SigningCredentials credentials = new(key, SecurityAlgorithms.HmacSha256);

            // 实例化JwtSecurityToken
            JwtSecurityToken jwt = new(
                issuer: issuer,
                audience: audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddDays(expirationDays),
                signingCredentials: credentials
            );

            // 生成 Token
            string token = new JwtSecurityTokenHandler().WriteToken(jwt);
            return token;
        }

        /// <summary>
        /// 从 Jwt Token 文本转换
        /// </summary>
        /// <param name="jwtTokenText"></param>
        /// <returns></returns>
        public static ClaimsPrincipal? TokenTextToClaimsPrincipal(string jwtTokenText)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            if (tokenHandler.CanReadToken(jwtTokenText))
            {
                var jwtSecurityToken = tokenHandler.ReadJwtToken(jwtTokenText);
                var identity = new ClaimsPrincipal(new ClaimsIdentity(jwtSecurityToken.Claims));
                return identity;
            }
            return null;
        }
    }
}
