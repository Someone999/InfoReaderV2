using InfoReader.Tools.Downloader;

namespace InfoReader.Tools.Downloader
{
}

namespace InfoReader.Tools
{
    public interface IDownloader
    {
        int Timeout { get; set; }
        int BufferSize { get; set; }
        public bool AutoRetry { get; set; }
        public int MaxRetry { get; set; }

        event DownloadCompletedEventHandler? OnDownloadCompleted;
        event DownloadFailedEventHandler? OnDownloadFailed;
        event DownloadProgressChangedEventHandler? OnDownloadProgressChanged;
        event DownloadRetryEventHandler? OnDownloadRetry;
        void Download(string url, string path);
    }
}