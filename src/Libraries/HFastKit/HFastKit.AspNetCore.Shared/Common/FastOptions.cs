using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HFastKit.AspNetCore.Shared.Common
{
    /// <summary>
    /// 快速选项
    /// </summary>
    public static class FastOptions
    {
        /// <summary>
        /// Json 序列化选项 (不序列化空值、跳过注释、非严格编码)
        /// </summary>
        public static readonly JsonSerializerOptions JsonSerializerOptions = new() { 
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull, 
            ReadCommentHandling = JsonCommentHandling.Skip, 
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping 
        };

        /// <summary>
        /// Json 序列化选项 (不序列化空值、跳过注释、非严格编码、CamelCase命名)
        /// </summary>
        public static readonly JsonSerializerOptions JsonSerializerOptionsByCamelCase = new() { 
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull, 
            ReadCommentHandling = JsonCommentHandling.Skip, 
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, 
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
        };
    }
}
