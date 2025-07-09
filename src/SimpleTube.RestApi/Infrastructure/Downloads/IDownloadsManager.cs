namespace SimpleTube.RestApi.Infrastructure.Downloads;

public interface IDownloadsManager
{
    void AddDownload(string channelId, string videoId, string fileName);
    Stream? GetDownload(string channelId, string videoId, string type);
    IReadOnlyList<FileInfo> GetDownloads(string channelId, string videoId);
}
