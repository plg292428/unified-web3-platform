namespace HFastKit.Net;

/// <summary>
/// Easy Downloader 下载开始事件参数
/// </summary>
public class DownloaderDownloadStartedEventArgs : EventArgs
{
    /// <summary>
    /// 文件长度
    /// </summary>
    public long FileLength { get; set; }

    /// <summary>
    /// Easy Downloader 下载开始事件参数
    /// </summary>
    /// <param name="fileLength">文件长度</param>
    public DownloaderDownloadStartedEventArgs(long fileLength)
    {
        FileLength = fileLength;
    }
}
