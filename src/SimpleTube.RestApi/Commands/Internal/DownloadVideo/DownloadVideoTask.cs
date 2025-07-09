using System.Text.Json;
using System.Text.Json.Serialization;
using SimpleTube.RestApi.Infrastructure.Downloads;
using SimpleTube.RestApi.Infrastructure.Tasks;
using SimpleTube.RestApi.Infrastructure.YouTube;

namespace SimpleTube.RestApi.Commands.Internal.DownloadVideo;

internal sealed partial class DownloadVideoTask : ITask
{
    private readonly IDownloadsManager _downloadsManager;
    private readonly string _videoId;
    public IProgress<ProgressReport> Progress { get; } = new Progress<ProgressReport>();

    public DownloadVideoTask(string videoId, IDownloadsManager downloadsManager)
    {
        _downloadsManager = downloadsManager;
        _videoId = videoId;
    }

    public async ValueTask Execute(CancellationToken cancellationToken)
    {
        var tempName = Path.GetTempFileName();
        File.Delete(tempName);
        var extensionIndex = tempName.LastIndexOf('.');

        if (extensionIndex > -1)
            tempName = tempName[..extensionIndex];

        Directory.CreateDirectory(tempName);
        var process = YtDlp.Invoke(
            $"--windows-filenames --write-info-json --progress-template \"%(progress)j\" --no-simulate -P \"{tempName}\" -P temp:temp --output %(id)s.%(ext)s https://www.youtube.com/watch?v={_videoId}"
        );

        while (!process.StandardOutput.EndOfStream)
        {
            var line = await process.StandardOutput.ReadLineAsync(cancellationToken);

            if (line is null || line.Length < 1 || line[0] != '{')
                continue;

            try
            {
                var progressReport = JsonSerializer.Deserialize<ProgressReport>(
                    line,
                    YtDlpJsonContext.Default.ProgressReport
                );

                if (progressReport is not null)
                    Progress.Report(progressReport);
            }
            catch (JsonException)
            {
                // Suppress.
            }
        }

        await process.WaitForExitAsync(cancellationToken);
        var downloadInfo = await GetDownloadInfo(tempName, cancellationToken);
        var videoFile = Path.Combine(tempName, $"{_videoId}.{downloadInfo.Extension}");
        _downloadsManager.AddDownload(downloadInfo.ChannelId, downloadInfo.Id, videoFile);
        Directory.Delete(tempName, true);
    }

    private async ValueTask<DownloadInfo> GetDownloadInfo(
        string directory,
        CancellationToken cancellationToken
    )
    {
        await using var stream = File.OpenRead(Path.Combine(directory, $"{_videoId}.info.json"));
        var info = await JsonSerializer.DeserializeAsync<DownloadInfo>(
            stream,
            YtDlpJsonContext.Default.DownloadInfo,
            cancellationToken
        );
        return info ?? throw new Exception("Failed to deserialize download info.");
    }

    public sealed record ProgressReport
    {
        [JsonPropertyName("downloaded_bytes")]
        public required long DownloadedBytes { get; init; }

        [JsonPropertyName("elapsed")]
        public decimal Elapsed { get; init; }

        [JsonPropertyName("eta")]
        public decimal? Eta { get; init; }

        [JsonPropertyName("fragment_count")]
        public int FragmentCount { get; init; }

        [JsonPropertyName("fragment_index")]
        public int FragmentIndex { get; init; }

        [JsonPropertyName("_percent")]
        public decimal Percent { get; init; }

        [JsonPropertyName("speed")]
        public double? Speed { get; init; }

        [JsonPropertyName("status")]
        public string? Status { get; init; }

        [JsonPropertyName("total_bytes")]
        public double? TotalBytes { get; init; }

        [JsonPropertyName("total_bytes_estimate")]
        public double? TotalBytesEstimate { get; init; }
    }

    public sealed record DownloadInfo
    {
        [JsonPropertyName("channel_id")]
        public required string ChannelId { get; init; }

        [JsonPropertyName("ext")]
        public required string Extension { get; init; }

        [JsonPropertyName("id")]
        public required string Id { get; init; }
    }

    [JsonSerializable(typeof(DownloadInfo))]
    [JsonSerializable(typeof(ProgressReport))]
    private sealed partial class YtDlpJsonContext : JsonSerializerContext;
}
