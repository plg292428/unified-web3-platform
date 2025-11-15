using HFastKit.AspNetCore.Shared;
using HFastKit.AspNetCore.Shared.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text.Json;

namespace HFastKit.AspNetCore.Middlewares
{
    /// <summary>
    /// 简易异常处理器拓展
    /// </summary>
    public static class EasyExceptionHandlerExtensions
    {
        /// <summary>
        /// 使用简易异常处理器
        /// </summary>
        /// <param name="app">IApplicationBuilder</param>
        /// <returns></returns>
        public static IApplicationBuilder UseEasyExceptionHandler(this IApplicationBuilder app)=> app.UseExceptionHandler(errorApp =>
        {
            errorApp.Run(async context =>
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError; ;
                var wrappedResult = WrappedResult.Failed("Internal Server Error");
                string result = JsonSerializer.Serialize(wrappedResult, FastOptions.JsonSerializerOptionsByCamelCase);
                await context.Response.WriteAsync(result);
            });
        });
    }
}

