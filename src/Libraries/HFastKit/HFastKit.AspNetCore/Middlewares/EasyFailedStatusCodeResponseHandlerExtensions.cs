using HFastKit.AspNetCore.Shared;
using HFastKit.AspNetCore.Shared.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text.Json;

namespace HFastKit.AspNetCore.Middlewares
{
    /// <summary>
    /// 简易失败状态码响应处理拓展
    /// </summary>
    public static class EasyFailedStatusCodeResponseHandlerExtensions
    {
        /// <summary>
        /// 使用简易失败状态码响应处理拓展
        /// </summary>
        /// <param name="app">IApplicationBuilder</param>
        /// <returns></returns>
        public static IApplicationBuilder UseFailedStatusCodeResponseHandler(this IApplicationBuilder app) => app.UseStatusCodePages(async context =>
        {
            context.HttpContext.Response.ContentType = "application/json";
            context.HttpContext.Response.StatusCode = context.HttpContext.Response.StatusCode;

            var wrappedResult = WrappedResult.Failed();
            wrappedResult.ErrorMessage = (HttpStatusCode)context.HttpContext.Response.StatusCode switch
            {
                HttpStatusCode.BadRequest => "Bad Request",
                HttpStatusCode.Unauthorized => "Unauthorized",
                HttpStatusCode.Forbidden => "Forbidden",
                HttpStatusCode.NotFound => "Not Found",
                HttpStatusCode.MethodNotAllowed => "Method Not Allowed",
                HttpStatusCode.UnsupportedMediaType => "Unsupported Nedia Type",
                HttpStatusCode.BadGateway => "Bad Gateway",
                _ => "Unknown error",
            };
            string result = JsonSerializer.Serialize(wrappedResult, FastOptions.JsonSerializerOptionsByCamelCase);
            await context.HttpContext.Response.WriteAsync(result);
        });
    }
}

