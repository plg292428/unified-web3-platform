namespace HFastKit.Net;

/// <summary>
/// Easy Downloader 重命名事件参数
/// </summary>
public class DownloaderFileRenamedEventArgs : EventArgs
{
    /// <summary>
    /// 旧文件名
    /// </summary>
    public string OldFileName { get; set; }

    /// <summary>
    /// 新文件名
    /// </summary>
    public string NewFileName { get; set; }

    /// <summary>
    /// Easy Downloader 重命名事件参数
    /// </summary>
    /// <param name="oldFileName">旧文件名</param>
    /// <param name="newFileName">新文件名</param>
    public DownloaderFileRenamedEventArgs(string oldFileName, string newFileName)
    {
        OldFileName = oldFileName;
        NewFileName = newFileName;
    }
}

