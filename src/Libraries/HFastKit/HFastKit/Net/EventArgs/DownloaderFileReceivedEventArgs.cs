namespace HFastKit.Net;

/// <summary>
/// Easy Downloader 接收文件事件参数
/// </summary>
public class DownloaderFileReceivedEventArgs : EventArgs
{
    /// <summary>
    /// 文件长度
    /// </summary>
    public long FileLength { get; set; }

    /// <summary>
    /// 已接收长度
    /// </summary>
    public long ReceivedLength { get; set; }

    /// <summary>
    /// 下载速度
    /// </summary>
    public long DownloadSpeed { get; set; }

    /// <summary>
    /// Easy Downloader 接收文件事件参数
    /// </summary>
    /// <param name="fileLength">文件长度</param>
    /// <param name="receivedLength">已接收长度</param>
    /// <param name="downloadSpeed">下载速度</param>
    public DownloaderFileReceivedEventArgs(long fileLength, long receivedLength, long downloadSpeed)
    {
        FileLength = fileLength;
        ReceivedLength = receivedLength;
        DownloadSpeed = downloadSpeed;
    }
}

