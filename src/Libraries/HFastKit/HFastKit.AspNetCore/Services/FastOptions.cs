using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HFastKit.AspNetCore.Services
{
    /// <summary>
    /// 快速选项
    /// </summary>
    public static class FastOptions
    {
        /// <summary>
        /// MVC JSON 配置
        /// </summary>
        public static readonly Action<JsonOptions> MvcJsonConfigure = (options) =>
        {
            options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;     // 不序列化空值
            options.JsonSerializerOptions.ReadCommentHandling = JsonCommentHandling.Skip;                   // 忽略注释
            options.JsonSerializerOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;            // 防止Json中文编码、转义
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;                // Camel命名
        };
    }
}

