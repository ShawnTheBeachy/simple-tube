namespace SimpleTube.RestApi.Infrastructure.Downloads.Internal;

internal sealed class DownloadsManager : IDownloadsManager
{
    private static string DownloadsFolder => AppData.GetFolder("downloads");

    public void AddDownload(string channelId, string videoId, string fileName)
    {
        var sourceFile = new FileInfo(fileName);
        var targetFile = new FileInfo(
            Path.Combine(DownloadsFolder, channelId, $"{videoId}{sourceFile.Extension}")
        );
        Directory.CreateDirectory(targetFile.DirectoryName!);
        sourceFile.CopyTo(targetFile.FullName, true);
    }

    public Stream? GetDownload(string channelId, string videoId, string type)
    {
        var fileInfo = new FileInfo(Path.Combine(DownloadsFolder, channelId, $"{videoId}.{type}"));
        return !fileInfo.Exists ? null : fileInfo.OpenRead();
    }

    public IReadOnlyList<FileInfo> GetDownloads(string channelId, string videoId)
    {
        var channelDir = new DirectoryInfo(Path.Combine(DownloadsFolder, channelId));
        return !channelDir.Exists
            ? []
            : channelDir.EnumerateFiles($"{videoId}.*", SearchOption.TopDirectoryOnly).ToArray();
    }
}
