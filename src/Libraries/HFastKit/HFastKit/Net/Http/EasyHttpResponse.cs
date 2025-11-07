using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Json;
using System.Text.Json;

namespace HFastKit.Net;

/// <summary>
/// EasyHttp 响应
/// </summary>
public class EasyHttpResponse : IDisposable
{
    private bool _disposed = false;

    /// <summary>
    /// 是否请求成功
    /// </summary>
    public bool IsRequestSuccess { get; } = false;

    /// <summary>
    /// HTTP 响应消息
    /// </summary>
    public HttpResponseMessage? HttpResponseMessage { get; }

    public EasyHttpResponse(bool isRequestSuccess = false, HttpResponseMessage? httpResponseMessage = null)
    {
        IsRequestSuccess = isRequestSuccess;
        HttpResponseMessage = httpResponseMessage;
    }

    public EasyHttpResponse(HttpResponseMessage httpResponseMessage) : this(true, httpResponseMessage)
    {

    }

    /// <summary>
    /// 读字节数组
    /// </summary>
    /// <returns></returns>
    public byte[] ReadAsByteArray()
    {
        CheckDisposed();
        CheckResponseMessageIsNull();
        return HttpResponseMessage.Content.ReadAsByteArrayAsync().Result;
    }

    /// <summary>
    /// 读字符串
    /// </summary>
    /// <returns></returns>
    public string ReadAsString()
    {
        CheckDisposed();
        CheckResponseMessageIsNull();
        return HttpResponseMessage.Content.ReadAsStringAsync().Result;
    }

    /// <summary>
    /// 读 JSON
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="options">JSON 序列化选项</param>
    /// <returns></returns>
    public T? ReadFromJson<T>(JsonSerializerOptions? options = null)
    {
        CheckDisposed();
        CheckResponseMessageIsNull();
        options ??= new();
        return HttpResponseMessage.Content.ReadFromJsonAsync<T>(options).Result;
    }

    /// <summary>
    /// 读 JSON
    /// </summary>
    /// <param name="options">JSON 序列化选项</param>
    /// <returns></returns>
    public object? ReadFromJson(JsonSerializerOptions? options = null)
    {
        return ReadFromJson<object?>(options);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing && !_disposed)
        {
            _disposed = true;
            if (disposing)
            {
                HttpResponseMessage?.Dispose();
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

    [MemberNotNull("HttpResponseMessage")]
    private void CheckResponseMessageIsNull()
    {
        if (HttpResponseMessage is null)
        {
            throw new InvalidOperationException("HttpResponseMessage instance is null");
        }
    }
}
