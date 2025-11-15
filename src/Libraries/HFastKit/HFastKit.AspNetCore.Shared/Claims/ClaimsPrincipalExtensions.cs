using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;

namespace HFastKit.AspNetCore.Shared
{
    /// <summary>
    /// ClaimsPrincipal 拓展方法
    /// </summary>
    public static class ClaimsPrincipalExtensions
    {
        /// <summary>
        /// 获取Int值
        /// </summary>
        /// <param name="claimsPrincipal">ClaimsPrincipal对象</param>
        /// <param name="keyName">键名称</param>
        /// <returns>获取成功返回值，失败返回 null</returns>
        public static int? GetInt(this ClaimsPrincipal claimsPrincipal, string keyName)
        {
            Claim? claim = claimsPrincipal.FindFirst(keyName);
            return claim is null ? null : int.TryParse(claim.Value, out int value) ? value : null;
        }

        /// <summary>
        /// 获取Int值
        /// </summary>
        /// <param name="claimsPrincipal">ClaimsPrincipal对象</param>
        /// <param name="keyName">键名称</param>
        /// <param name="value">是否获取到值</param>
        /// <returns></returns>
        public static bool TryGetInt(this ClaimsPrincipal claimsPrincipal, string keyName, [NotNullWhen(true)] out int? value)
        {
            value = claimsPrincipal.GetInt(keyName);
            return value is not null;
        }

        /// <summary>
        /// 获取字符串值
        /// </summary>
        /// <param name="claimsPrincipal">ClaimsPrincipal对象</param>
        /// <param name="keyName">键名称</param>
        /// <returns>>获取成功返回值，失败返回 null</returns>
        public static string? GetString(this ClaimsPrincipal claimsPrincipal, string keyName)
        {
            Claim? claim = claimsPrincipal.FindFirst(keyName);
            return claim is null ? null : claim.Value;
        }

        /// <summary>
        /// 获取字符串值
        /// </summary>
        /// <param name="claimsPrincipal">ClaimsPrincipal对象</param>
        /// <param name="keyName">键名称</param>
        /// <param name="value">是否获取到值</param>
        /// <returns></returns>
        public static bool TryGetString(this ClaimsPrincipal claimsPrincipal, string keyName, [NotNullWhen(true)] out string? value)
        {
            value = claimsPrincipal.GetString(keyName);
            return value is not null;
        }
    }
}
