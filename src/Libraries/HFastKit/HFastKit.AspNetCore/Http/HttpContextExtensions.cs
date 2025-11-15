using Microsoft.AspNetCore.Http;
using System.Net;

namespace HFastKit.AspNetCore.Http
{
    /// <summary>
    /// HttpContext 拓展方法
    /// </summary>
    public static class HttpContextExtensions
    {
        /// <summary>
        /// 获取请求客户IP
        /// </summary>
        /// <param name="httpContext">HttpContext对象</param>
        /// <returns></returns>
        public static string GetRemoteIp(this HttpContext httpContext)
        {
            string result = string.Empty;
            try
            {
                IPAddress? ipAddress = httpContext.Connection.RemoteIpAddress;
                if (ipAddress is null)
                {
                    return result;
                }
                result = ipAddress.MapToIPv4().ToString();
            }
            catch { }
            return result;
        }
    }
}
