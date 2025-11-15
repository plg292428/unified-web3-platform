using System.Net.WebSockets;
using System.Runtime.Versioning;
using System.Text;

namespace HFastKit.Net;

/// <summary>
/// 简单 Web Socket Client
/// </summary>
public class EasyWebSocketClient : IDisposable
{
    /// <summary>
    /// 是否释放
    /// </summary>
    private bool _disposed = false;

    /// <summary>
    /// Web Socket Client
    /// </summary>
    ClientWebSocket _client;

    public delegate void ReceivedMessageEventHandler(object sender, WebSocketMessage message);

    /// <summary>
    /// 消息编码
    /// </summary>
    public Encoding Encoding { get; }

    /// <summary>
    /// WebSocket 状态
    /// </summary>
    public WebSocketState? State { get => _client?.State; }

    /// <summary>
    ///  WebSocket 关闭状态
    /// </summary>
    public WebSocketCloseStatus? CloseStatus { get => _client?.CloseStatus; }

    // <summary>
    /// 连接打开
    /// </summary>
    public event EventHandler? OnOpen;

    /// <summary>
    /// 收到消息
    /// </summary>
    public event ReceivedMessageEventHandler? OnReceivedMessage;

    /// <summary>
    /// 连接关闭
    /// </summary>
    public event EventHandler? OnClose;

    /// <summary>
    /// 简单 Web Socket Client
    /// </summary>
    /// <param name="encoding">消息编码</param>
    /// <exception cref="ArgumentNullException">参数为空</exception>
    public EasyWebSocketClient(Encoding? encoding = null)
    {
        Encoding = encoding ?? Encoding.UTF8;
        _client = new ClientWebSocket();
        _client.Options.SetBuffer(4096, 4096);
    }

    /// <summary>
    /// 打开连接
    /// </summary>
    /// <param name="uriText">连接地址</param>
    public void Open(string uriText)
    {
        if (string.IsNullOrEmpty(uriText))
        {
            throw new ArgumentNullException(nameof(uriText));
        }
        if (_client.State == WebSocketState.Connecting || _client.State == WebSocketState.Open)
        {
            return;
        }
        Task.Run(async () =>
        {
            // 初始化链接
            Uri uri = new(uriText);
            await _client.ConnectAsync(uri, CancellationToken.None);
            OnOpen?.Invoke(_client, new());
            await ReceiveAsync();
        });
    }

    private async Task ReceiveAsync()
    {
        // 消息容器
        List<byte> bytes = new List<byte>();

        // 缓冲区
        var buffer = new byte[1024 * 4];

        // 是否关闭
        while (!CloseStatus.HasValue)
        {
            // 监听Socket信息
            WebSocketReceiveResult result;
            try
            {
                result = await _client.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }
            catch
            {
                _client.Abort();
                OnClose?.Invoke(this, new());
                break;
            }
            if (result.MessageType == WebSocketMessageType.Close)
            {
                break;
            }

            // 消息处理
            if (result.MessageType == WebSocketMessageType.Text)
            {
                bytes.AddRange(buffer.Take(result.Count));

                // 消息是否已接收完全
                if (result.EndOfMessage)
                {
                    // 发送过来的消息
                    string message = Encoding.GetString(bytes.ToArray(), 0, bytes.Count);

                    WebSocketMessage webSocketMessage = new(message);
                    OnReceivedMessage?.Invoke(_client, webSocketMessage);

                    // 清空消息容器
                    bytes.Clear();
                }
            }
            else if (result.MessageType == WebSocketMessageType.Binary)
            {
                bytes.AddRange(buffer.Take(result.Count));
                if (result.EndOfMessage)
                {
                    WebSocketMessage webSocketMessage = new(bytes.ToArray());
                    OnReceivedMessage?.Invoke(_client, webSocketMessage);

                    // 清空消息容器
                    bytes.Clear();
                }
            }
        }
    }

    /// <summary>
    /// 关闭连接
    /// </summary>
    public void Close()
    {
        Close(WebSocketCloseStatus.NormalClosure, "User Closed");
    }

    /// <summary>
    /// 关闭连接
    /// </summary>
    private void Close(WebSocketCloseStatus closeStatus, string statusDescription)
    {
        if (State.HasValue && State != WebSocketState.Closed && State != WebSocketState.Aborted)
        {
            // 关闭WebSocket（客户端发起）
            try
            {
                _client.CloseAsync(closeStatus, statusDescription, CancellationToken.None);
            }
            catch
            {
            }
            _client.Abort();
        }
        OnClose?.Invoke(this, new());
    }

    /// <summary>
    /// 发送文本消息
    /// </summary>
    /// <param name="message">消息</param>
    public void Send(string message)
    {
        if (_client.State is not WebSocketState.Open)
        {
            return;
        }

        Task.Run(async () =>
        {
            var replyMess = Encoding.GetBytes(message);

            //发送消息
            await _client.SendAsync(new ArraySegment<byte>(replyMess), WebSocketMessageType.Text, true, CancellationToken.None);
        });
    }

    /// <summary>
    /// 发送二进制消息
    /// </summary>
    /// <param name="bytes">消息</param>
    /// <returns></returns>
    public void Send(byte[] bytes)
    {
        if (_client.State is not WebSocketState.Open)
        {
            return;
        }

        Task.Run(async () =>
        {
            // 发送消息
            await _client.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Binary, true, CancellationToken.None);
        });
    }

    /// <summary>
    /// 释放资源
    /// </summary>
    /// <param name="disposing"></param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            _disposed = true;
            if (disposing)
            {
                _client.Dispose();
            }
        }
    }

    /// <summary>
    /// 释放资源
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}

