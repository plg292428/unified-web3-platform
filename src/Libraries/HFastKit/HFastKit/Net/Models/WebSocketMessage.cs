using System.Net.WebSockets;
using System.Runtime.Versioning;

namespace HFastKit.Net;

/// <summary>
/// Web Socket 消息接口
/// </summary>
public interface IWebSocketMessage
{
    /// <summary>
    /// 消息类型
    /// </summary>
    public WebSocketMessageType MessageType { get; }

    /// <summary>
    /// 文本内容
    /// </summary>
    public string? Text { get; }

    /// <summary>
    /// 二进制内容
    /// </summary>
    public byte[]? Binary { get; }
}

/// <summary>
/// Web Socket 消息
/// </summary>
public class WebSocketMessage : IWebSocketMessage
{
    /// <summary>
    /// 消息类型
    /// </summary>
    public WebSocketMessageType MessageType { get; }

    /// <summary>
    /// 文本内容
    /// </summary>
    public string? Text { get; }

    /// <summary>
    /// 二进制内容
    /// </summary>
    public byte[]? Binary { get; }

    /// <summary>
    /// Web Socket 消息
    /// </summary>
    /// <param name="data">二进制数据</param>
    public WebSocketMessage(byte[] data)
    {
        MessageType = WebSocketMessageType.Binary;
        Binary = data;
    }

    /// <summary>
    /// Web Socket 消息
    /// </summary>
    /// <param name="data">文本数据</param>
    public WebSocketMessage(string data)
    {
        MessageType = WebSocketMessageType.Text;
        Text = data;
    }
}
