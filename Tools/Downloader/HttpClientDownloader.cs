using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace InfoReader.Tools.Downloader
{
    public class HttpClientDownloader : IDownloader
    {
        public int Timeout { get; set; } = 30000;
        public int BufferSize { get; set; } = 8192;
        public bool AutoRetry { get; set; }
        public int MaxRetry { get; set; } = 10;
        public event DownloadCompletedEventHandler? OnDownloadCompleted;
        public event DownloadFailedEventHandler? OnDownloadFailed;
        public event DownloadProgressChangedEventHandler? OnDownloadProgressChanged;
        public event DownloadRetryEventHandler? OnDownloadRetry;

        bool TryDownload(string url, string path, out Exception? exception)
        {
            exception = null;
            HttpClient client = new HttpClient();
            FileStream? created = null;
            try
            {

                if (Timeout != 0)
                {
                    client.Timeout = TimeSpan.FromMilliseconds(Timeout);
                }
                HttpResponseMessage msg = client.GetAsync(url, HttpCompletionOption.ResponseContentRead).Result;
                long total = msg.Content.Headers.ContentLength ?? throw new InvalidOperationException();
                Stream s = msg.Content.ReadAsStreamAsync().Result;


                byte[] buffer = new byte[BufferSize];
                FileStream fstream = File.Create(path);
                created = fstream;
                int readLen = 0, downloadedLen = 0;
                do
                {
                    readLen = s.Read(buffer, 0, BufferSize);
                    fstream.Write(buffer, 0, readLen);
                    downloadedLen += readLen;
                    OnDownloadProgressChanged?.Invoke((double)downloadedLen / total);
                } while (readLen != 0);
                return true;
            }
            catch (Exception e)
            {
                client.CancelPendingRequests();
                exception = e;
                created?.Close();
                return false;
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
                b = TryDownload(url, path, out ex);
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
}
