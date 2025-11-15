using System.Diagnostics;
using System.Runtime.Versioning;

namespace HFastKit.Net;

/// <summary>
/// 简单Http下载
/// </summary>
public class EasyDownloader
{
    /// <summary>
    /// 是否释放
    /// </summary>
    private bool _disposed = false;

    /// <summary>
    /// 私有HttpClient
    /// </summary>
    private HttpClient _client;

    /// <summary>
    /// 接收缓冲区大小
    /// </summary>
    private int _buffSize;

    /// <summary>
    /// 下载中
    /// </summary>
    public bool Downloading { get; private set; } = false;

    /// <summary>
    /// 下载地址
    /// </summary>
    public string? DownLoadUrl { get; private set; } = null;

    /// <summary>
    /// 绝对地址
    /// </summary>
    public Uri? Uri { get; private set; } = null;

    /// <summary>
    /// 保存文件名
    /// </summary>
    public string? SaveFileName { get; private set; } = null;

    /// <summary>
    /// 保存路径
    /// </summary>
    public string? SavePath { get; private set; } = null;

    /// <summary>
    /// 文件长度
    /// </summary>
    public long FileLength { get; private set; } = 0;

    /// <summary>
    /// 已接收长度
    /// </summary>
    public long ReceivedFileLength { get; private set; } = 0;

    /// <summary>
    /// 错误事件
    /// </summary>
    public event EventHandler<ErrorEventArgs>? OnError;

    /// <summary>
    /// 下载开始
    /// </summary>
    public event EventHandler<DownloaderDownloadStartedEventArgs>? OnDownloadStarted;

    /// <summary>
    /// 下载完成
    /// </summary>
    public event EventHandler? OnDownloadCompleted;

    /// <summary>
    /// 文件重命名
    /// </summary>
    public event EventHandler<DownloaderFileRenamedEventArgs>? OnFileRenamed;

    /// <summary>
    /// 文件接收
    /// </summary>
    public event EventHandler<DownloaderFileReceivedEventArgs>? OnFileReceived;

    /// <summary>
    /// 简单Http下载类
    /// </summary>
    /// <param name="timeOutSeconds">超时时间</param>
    public EasyDownloader(int buffSize = 4096, int timeOutSeconds = 20)
    {
        if (buffSize < 1024 || buffSize > 10240)
        {
            throw new ArgumentOutOfRangeException(nameof(buffSize));
        }
        _buffSize = buffSize;
        _client = new HttpClient();

        SetTimeOut(timeOutSeconds);
        EmulatePc();
    }

    /// <summary>
    /// 设置请求超时时间（秒）
    /// </summary>
    /// <param name="timeOutSeconds">超时时间</param>
    public void SetTimeOut(int timeOutSeconds)
    {
        if (timeOutSeconds < 1)
        {
            timeOutSeconds = 1;
        }
        _client.Timeout = TimeSpan.FromSeconds(timeOutSeconds);
    }

    /// <summary>
    /// 重置
    /// </summary>
    public void Reset()
    {
        if (Downloading)
        {
            var ex = new Exception("File is downloading");
            OnError?.Invoke(this, new(ex));
            return;
        }
        Downloading = false;
        DownLoadUrl = null;
        SaveFileName = null;
        SavePath = null;
        FileLength = 0;
        ReceivedFileLength = 0;
        Uri = null;
    }

    /// <summary>
    /// 下载文件
    /// </summary>
    /// <param name="downLoadUrl">下载地址</param>
    /// <param name="saveFileName">保存文件名</param>
    /// <param name="savePath">保存目录</param>
    public void DownLoadFile(string downLoadUrl, string? saveFileName = null, string? savePath = null)
    {
        if (Downloading)
        {
            var ex = new Exception("File is downloading");
            OnError?.Invoke(this, new(ex));
            return;
        }

        DownLoadUrl = downLoadUrl;
        try
        {
            Uri = new Uri(downLoadUrl);
        }
        catch (Exception ex)
        {
            OnError?.Invoke(this, new(ex));
            return;
        }

        if (string.IsNullOrEmpty(saveFileName))
        {
            SaveFileName = "DownloadTemp";
        }
        else
        {
            SaveFileName = saveFileName;
        }

        if (string.IsNullOrEmpty(savePath))
        {
            SavePath = Directory.GetCurrentDirectory();
        }
        else
        {
            SavePath = savePath;
        }

        // 检查目录
        if (!Directory.Exists(SavePath))
        {
            Directory.CreateDirectory(SavePath);
        }

        // 检查文件是否存在，重复则自动重命名
        int i = 1;
        string oldSaveFileName = SaveFileName;
        while (File.Exists(Path.Combine(SavePath, SaveFileName)))
        {
            SaveFileName = $"{oldSaveFileName}{i}";
            if (i > 500)
            {
                break;
            }
            i++;
        }
        if (i > 500)
        {
            var ex = new Exception("Unable to rename file automatically");
            OnError?.Invoke(this, new(ex));
            return;
        }
        if (SaveFileName != oldSaveFileName)
        {
            OnFileRenamed?.Invoke(this, new(oldSaveFileName, SaveFileName));
        }
        FileInfo fileInfo = new(Path.Combine(SavePath, SaveFileName));

        // 下载文件
        Stopwatch stopwatch = new();
        Task.Run(async () =>
        {
            try
            {
                // 响应
                HttpResponseMessage response = await _client.GetAsync(Uri, HttpCompletionOption.ResponseHeadersRead);
                if (!response.IsSuccessStatusCode)
                {
                    var ex = new Exception("Http status code error");
                    OnError?.Invoke(this, new(ex));
                    return;
                }

                // 文件长度
                long? fileLength = response.Content.Headers.ContentLength;
                if (!fileLength.HasValue)
                {
                    var ex = new Exception("Unable to read file length");
                    OnError?.Invoke(this, new(ex));
                    return;
                }
                FileLength = fileLength.Value;
                OnDownloadStarted?.Invoke(this, new(FileLength));

                // 接收文件
                stopwatch.Start();
                using Stream stream = await response.Content.ReadAsStreamAsync();
                using FileStream fileStream = fileInfo.Create();
                byte[] buffer = new byte[_buffSize];
                int length = 0;
                while ((length = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    ReceivedFileLength += length;

                    // 写入到文件
                    fileStream.Write(buffer, 0, length);
                    fileStream.Flush();

                    // 计算平均下载速度
                    stopwatch.Stop();
                    long speed = (long)Math.Floor(ReceivedFileLength / stopwatch.Elapsed.TotalSeconds);

                    OnFileReceived?.Invoke(this, new(FileLength, ReceivedFileLength, speed));
                    stopwatch.Start();
                }

                // 下载完成
                Downloading = false;
            }
            catch (Exception ex)
            {
                Downloading = false;
                OnError?.Invoke(this, new(ex));
                return;
            }
            finally
            {
                stopwatch.Stop();
            }
            OnDownloadCompleted?.Invoke(this, new());
        });
    }

    /// <summary>
    /// 模拟PC
    /// </summary>
    private void EmulatePc()
    {
        _client.DefaultRequestHeaders.Remove("user-agent");
        _client.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/80.0.3987.149 Safari/537.36 Edg/80.0.361.69");
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

