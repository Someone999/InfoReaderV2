using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace InfoReader.Tools.Downloader;

public delegate void DownloadCompletedEventHandler(string filePath);

public delegate void DownloadFailedEventHandler(string url, string filePath, Exception? e);

public delegate void DownloadProgressChangedEventHandler(double progress);

public delegate void DownloadRetryEventHandler(string url, string path, int retryCount, Exception? e);

public class HttpWebRequestDownloader : IDownloader
{
    

    public int Timeout { get; set; } = 30000;
    public bool AutoRetry { get; set; }
    public int MaxRetry { get; set; } = 10;
    public int BufferSize { get; set; } = 8192;
    public Dictionary<string, string> Headers { get; } = new Dictionary<string, string>();
    public event DownloadCompletedEventHandler? OnDownloadCompleted;
    public event DownloadFailedEventHandler? OnDownloadFailed;
    public event DownloadProgressChangedEventHandler? OnDownloadProgressChanged;
    public event DownloadRetryEventHandler? OnDownloadRetry;
    bool TryDownload(string url, string path, out Exception? exception)
    {
        exception = null;
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        FileStream? created = null;
        try
        {

            if (Timeout != 0)
            {
                //client.Timeout = TimeSpan.FromMilliseconds(Timeout);
                request.Timeout = Timeout;
            }

            var response = request.GetResponse();
            Stream? s = response.GetResponseStream();
            long total = response.ContentLength;

            byte[] buffer = new byte[BufferSize];
            string? directory = Path.GetDirectoryName(path);
            if (directory != null && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            FileStream fstream = File.Create(path);
            created = fstream;
            int readLen = 0, downloadedLen = 0;
            do
            {
                readLen = s?.Read(buffer, 0, BufferSize) ?? throw new InvalidOperationException();
                fstream.Write(buffer, 0, readLen);
                downloadedLen += readLen;
                OnDownloadProgressChanged?.Invoke((double) downloadedLen / total);
            } while (readLen != 0);

            return true;
        }
        catch (Exception e)
        {
            exception = e;
            request.Abort();
            return false;
        }
        finally
        {
            created?.Close();
        }
    }

    public void Download(string url, string path)
    {
        int retryCount = 0;
        bool b = TryDownload(url, path, out var ex);
        if (b)
        {
            OnDownloadCompleted?.Invoke(path);
            return;
        }
        while (retryCount < MaxRetry && AutoRetry)
        {
            OnDownloadRetry?.Invoke(url, path, retryCount, ex);
            b = TryDownload(url, path,out ex);
            retryCount++;
        }

        if (!b)
        {
            OnDownloadFailed?.Invoke(url, path, ex);
        }
        else
        {
            OnDownloadCompleted?.Invoke(path);
        }
    }

}