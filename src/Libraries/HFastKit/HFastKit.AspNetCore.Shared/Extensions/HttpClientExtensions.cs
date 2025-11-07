using HFastKit.AspNetCore.Shared.Common;
using System.Net.Http.Json;
using System.Text.Json;
using System.Web;

namespace HFastKit.AspNetCore.Shared.Extensions
{
    /// <summary>
    /// HttpClient 扩展方法
    /// </summary>
    public static class HttpClientExtensions
    {
        /// <summary>
        /// Post
        /// </summary>
        /// <typeparam name="TResult">响应内容附加数据</typeparam>
        /// <typeparam name="TValue">请求内容</typeparam>
        /// <param name="requestUri">请求地址</param>
        /// <param name="requestContent">请求内容</param>
        /// <returns></returns>
        public static async Task<WrappedResult<TResult?>> PostAsJsonFromWrappedAsync<TResult, TValue>(this HttpClient httpClient, string requestUri, TValue? requestContent) where TValue : class?
        {
            try
            {
                using var response = await httpClient.PostAsJsonAsync(requestUri, requestContent, FastOptions.JsonSerializerOptionsByCamelCase);
                var result = await response.Content.ReadFromJsonAsync<WrappedResult<TResult?>>(FastOptions.JsonSerializerOptionsByCamelCase);
                return result ?? WrappedResult.Failed("请求过程中遇到异常");
            }
            catch
            {
                return WrappedResult.Failed("请求过程中遇到异常");
            }
        }

        /// <summary>
        /// Post
        /// </summary>
        /// <typeparam name="TValue">请求内容</typeparam>
        /// <param name="requestUri">请求地址</param>
        /// <param name="requestContent">请求内容</param>
        /// <returns></returns>
        public static async Task<WrappedResult> PostAsJsonFromWrappedAsync<TValue>(this HttpClient httpClient, string requestUri, TValue? requestContent) where TValue : class? =>
            (await PostAsJsonFromWrappedAsync<object?, TValue>(httpClient, requestUri, requestContent)).Convert();

        /// <summary>
        /// Get
        /// </summary>
        /// <typeparam name="TResult">响应内容附加数据</typeparam>
        /// <param name="requestUri">请求地址</param>
        /// <returns></returns>
        public static async Task<WrappedResult<TResult?>> GetFromWrappedAsync<TResult>(this HttpClient httpClient, string requestUri)
        {
            try
            {
                //var result = await httpClient.GetFromJsonAsync<WrappedResult<TResult?>>($"{requestUri}", FastOptions.JsonSerializerOptionsByCamelCase);
                using var response = await httpClient.GetAsync($"{requestUri}");
                var result = await response.Content.ReadFromJsonAsync<WrappedResult<TResult?>>(FastOptions.JsonSerializerOptionsByCamelCase);
                return result ?? WrappedResult.Failed("请求过程中遇到异常");
            }
            catch
            {
                return WrappedResult.Failed("请求过程中遇到异常");
            }
        }

        /// <summary>
        /// Get
        /// </summary>
        /// <typeparam name="TResult">响应内容附加数据</typeparam>
        /// <typeparam name="TValue">请求内容</typeparam>
        /// <param name="requestUri">请求地址</param>
        /// <param name="requestContent">请求内容</param>
        /// <returns></returns>
        public static async Task<WrappedResult<TResult?>> GetAsJsonFromWrappedAsync<TResult, TValue>(this HttpClient httpClient, string requestUri, TValue? requestContent = null) where TValue : class?
        {
            var queryString = string.Empty;
            if (requestContent is not null)
            {
                queryString = ToQueryString(requestContent);
            }
            return await GetFromWrappedAsync<TResult?>(httpClient, $"{requestUri}{queryString}");
        }

        /// <summary>
        /// Get
        /// </summary>
        /// <typeparam name="TValue">请求内容</typeparam>
        /// <param name="requestUri">请求地址</param>
        /// <param name="requestContent">请求内容</param>
        /// <returns></returns>
        public static async Task<WrappedResult> GetAsJsonFromWrappedAsync<TValue>(this HttpClient httpClient, string requestUri, TValue? requestContent = null) where TValue : class? =>
            (await GetAsJsonFromWrappedAsync<object?, TValue?>(httpClient, requestUri, requestContent)).Convert();


        /// <summary>
        /// Get
        /// </summary>
        /// <typeparam name="TResult">响应内容附加数据</typeparam>
        /// <param name="requestUri">请求地址</param>
        /// <returns></returns>
        public static async Task<WrappedResult<TResult?>> GetAsJsonFromWrappedAsync<TResult>(this HttpClient httpClient, string requestUri) where TResult : class? => 
            await GetAsJsonFromWrappedAsync<TResult?, object?>(httpClient, requestUri, null);


        /// <summary>
        /// 对象到查询字符串
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static string ToQueryString<TEntity>(this TEntity? entity) where TEntity : class?
        {
            if (entity is null) return string.Empty;
            string jsonText = JsonSerializer.Serialize(entity, FastOptions.JsonSerializerOptionsByCamelCase);
            var dictionary = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonText);
            if (dictionary is null) return string.Empty;
            var enumerable = dictionary.Select(x => HttpUtility.UrlEncode(x.Key) + "=" + HttpUtility.UrlEncode(x.Value.ToString()));
            return $"?{string.Join("&", enumerable)}";
        }
    }
}
