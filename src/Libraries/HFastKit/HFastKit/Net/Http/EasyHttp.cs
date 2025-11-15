using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace HFastKit.Net;

/// <summary>
/// 简易 HTTP 客户端
/// </summary>
public class EasyHttp : IDisposable
{
    private bool _disposed = false;
    private TimeSpan _timeout;

    /// <summary>
    /// HTTP Client
    /// </summary>
    public HttpClient HttpClient { get; }

    /// <summary>
    /// 超时时间
    /// </summary>
    public TimeSpan TimeOut
    {
        get => _timeout;
        set
        {
            _timeout = value;
            if (HttpClient is not null)
            {
                HttpClient.Timeout = value;
            }
        }
    }

    /// <summary>
    /// 默认请求头
    /// </summary>
    public HttpRequestHeaders DefaultRequestHeaders => HttpClient.DefaultRequestHeaders;

    /// <summary>
    /// 授权信息
    /// </summary>
    public AuthenticationHeaderValue? AuthenticationHeaderValue => DefaultRequestHeaders?.Authorization;

    /// <summary>
    /// 简易 HTTP 客户端
    /// </summary>
    /// <param name="timeout">请求超时时间</param>
    public EasyHttp(TimeSpan? timeout = null)
    {
        if (timeout is null)
        {
            timeout = new TimeSpan(0, 0, 20);
        }
        HttpClient = new HttpClient();
        TimeOut = timeout.Value;
        EmulatePc();
    }

    /// <summary>
    /// 设置授权令牌
    /// </summary>
    /// <param name="scheme">方案</param>
    /// <param name="token">令牌</param>
    public void SetAuthorizationToken(string scheme, string token)
    {
        HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme, token);
    }

    /// <summary>
    /// 设置授权令牌
    /// </summary>
    /// <param name="token">令牌</param>
    public void SetAuthorizationToken(string token)
    {
        SetAuthorizationToken("Bearer", token);
    }

    /// <summary>
    /// 删除授权令牌
    /// </summary>
    public void RemoveAuthorizationToken()
    {
        HttpClient.DefaultRequestHeaders.Authorization = null;
    }

    /// <summary>
    /// 模拟手机
    /// </summary>
    public void EmulateMoble()
    {
        HttpClient.DefaultRequestHeaders.Remove("user-agent");
        HttpClient.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (iPhone; CPU iPhone OS 13_2_3 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/13.0.3 Mobile/15E148 Safari/604.1");
    }

    /// <summary>
    /// 模拟PC
    /// </summary>
    public void EmulatePc()
    {
        HttpClient.DefaultRequestHeaders.Remove("user-agent");
        HttpClient.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/80.0.3987.149 Safari/537.36 Edg/80.0.361.69");
    }

    /// <summary>
    /// HTTP Get
    /// </summary>
    /// <param name="requestUri">请求地址</param>
    /// <param name="httpResponseMessage">HTTP 响应消息</param>
    /// <returns>请求是否发送成功</returns>
    public bool TryGet(Uri requestUri, [NotNullWhen(true)] out HttpResponseMessage? httpResponseMessage)
    {
        CheckDisposed();
        httpResponseMessage = null;
        try
        {
            httpResponseMessage = HttpClient.GetAsync(requestUri).Result;
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// HTTP Get
    /// </summary>
    /// <param name="requestUri">请求地址</param>
    /// <param name="httpResponseMessage">HTTP 响应消息</param>
    /// <returns>请求是否发送成功</returns>
    public bool TryGet(string requestUri, [NotNullWhen(true)] out HttpResponseMessage? httpResponseMessage)
    {
        Uri uri = new(requestUri);
        return TryGet(uri, out httpResponseMessage);
    }

    /// <summary>
    /// HTTP Post
    /// </summary>
    /// <param name="requestUri">请求地址</param>
    /// <param name="httpContent">请求正文</param>
    /// <param name="httpResponseMessage">HTTP 响应消息</param>
    /// <returns>请求是否发送成功</returns>
    public bool TryPost(Uri requestUri, HttpContent httpContent, [NotNullWhen(true)] out HttpResponseMessage? httpResponseMessage)
    {
        CheckDisposed();
        httpResponseMessage = null;
        try
        {
            httpResponseMessage = HttpClient.PostAsync(requestUri, httpContent).Result;
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// HTTP Post
    /// </summary>
    /// <param name="requestUri">请求地址</param>
    /// <param name="httpContent">请求正文</param>
    /// <param name="httpResponseMessage">HTTP 响应消息</param>
    /// <returns>请求是否发送成功</returns>
    public bool TryPost(string requestUri, HttpContent httpContent, [NotNullWhen(true)] out HttpResponseMessage? httpResponseMessage)
    {
        Uri uri = new(requestUri);
        return TryPost(uri, httpContent, out httpResponseMessage);
    }

    /// <summary>
    /// HTTP Get
    /// </summary>
    /// <param name="requestUri">请求地址</param>
    /// <returns>响应</returns>
    public EasyHttpResponse Get(Uri requestUri)
    {
        if (!TryGet(requestUri, out HttpResponseMessage? httpResponseMessage))
        {
            return new EasyHttpResponse();
        }
        return new EasyHttpResponse(httpResponseMessage);
    }

    /// <summary>
    /// HTTP Get
    /// </summary>
    /// <param name="requestUri">请求地址</param>
    /// <returns>响应</returns>
    public EasyHttpResponse Get(string requestUri)
    {
        Uri uri = new(requestUri);
        return Get(uri);
    }

    /// <summary>
    /// HTTP Post 表单
    /// </summary>
    /// <param name="requestUri">请求地址</param>
    /// <param name="content">请求正文</param>
    /// <returns>响应</returns>
    /// <exception cref="ArgumentNullException">表单数据为空</exception>
    public EasyHttpResponse PostByForm(Uri requestUri, IDictionary<string, string> content)
    {
        if (content.Count < 1)
        {
            throw new ArgumentNullException(nameof(content));
        }
        using MultipartFormDataContent formData = new();
        foreach (var item in content)
        {
            StringContent stringContent = new StringContent(item.Value);
            formData.Add(stringContent, item.Key);
        }
        if (!TryPost(requestUri, formData, out HttpResponseMessage? httpResponseMessage))
        {
            return new EasyHttpResponse();
        }
        return new EasyHttpResponse(httpResponseMessage);
    }

    /// <summary>
    /// HTTP Post 表单
    /// </summary>
    /// <param name="requestUri">请求地址</param>
    /// <param name="content">请求正文</param>
    /// <returns>响应</returns>
    /// <exception cref="ArgumentNullException">表单数据为空</exception>
    public EasyHttpResponse PostByForm(string requestUri, IDictionary<string, string> content)
    {
        Uri uri = new(requestUri);
        return PostByForm(uri, content);
    }

    /// <summary>
    /// HTTP Post JSON
    /// </summary>
    /// <param name="requestUri">请求地址</param>
    /// <param name="content">请求正文</param>
    /// <param name="mediaType">媒体类型</param>
    /// <param name="options">JSON 序列化选项</param>
    /// <returns>响应</returns>
    public EasyHttpResponse PostByJson<T>(Uri requestUri, T content, MediaTypeHeaderValue? mediaType = null, JsonSerializerOptions? options = null)
    {
        mediaType ??= new("application/json");
        options ??= new();
        using JsonContent jsonContent = JsonContent.Create(content, mediaType, options);
        if (!TryPost(requestUri, jsonContent, out HttpResponseMessage? httpResponseMessage))
        {
            return new EasyHttpResponse();
        }
        return new EasyHttpResponse(httpResponseMessage);
    }

    /// <summary>
    /// HTTP Post JSON
    /// </summary>
    /// <param name="requestUri">请求地址</param>
    /// <param name="content">请求正文</param>
    /// <param name="mediaType">媒体类型</param>
    /// <param name="options">JSON 序列化选项</param>
    /// <returns>响应</returns>
    public EasyHttpResponse PostByJson(Uri requestUri, object content, MediaTypeHeaderValue? mediaType = null, JsonSerializerOptions? options = null)
    {
        return PostByJson(requestUri, content, mediaType, options);
    }

    /// <summary>
    /// HTTP Post JSON
    /// </summary>
    /// <param name="requestUri">请求地址</param>
    /// <param name="content">请求正文</param>
    /// <param name="mediaType">媒体类型</param>
    /// <param name="options">JSON 序列化选项</param>
    /// <returns>响应</returns>
    public EasyHttpResponse PostByJson<T>(string requestUri, T content, MediaTypeHeaderValue? mediaType = null, JsonSerializerOptions? options = null)
    {
        Uri uri = new(requestUri);
        return PostByJson(uri, content, mediaType, options);
    }

    /// <summary>
    /// HTTP Post JSON
    /// </summary>
    /// <param name="requestUri">请求地址</param>
    /// <param name="content">请求正文</param>
    /// <param name="mediaType">媒体类型</param>
    /// <param name="options">JSON 序列化选项</param>
    /// <returns>响应</returns>
    public EasyHttpResponse PostByJson(string requestUri, object content, MediaTypeHeaderValue? mediaType = null, JsonSerializerOptions? options = null)
    {
        Uri uri = new(requestUri);
        return PostByJson(uri, content, mediaType, options);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            _disposed = true;
            if (disposing)
            {
                HttpClient.Dispose();
            }
        }
    }

    private void CheckDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(GetType().ToString());
        }
    }
}

