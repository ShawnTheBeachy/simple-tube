using Microsoft.Data.Sqlite;
using SimpleTube.RestApi.Infrastructure.Database;
using SimpleTube.RestApi.Infrastructure.Images;
using SimpleTube.RestApi.Infrastructure.Tasks;
using SimpleTube.RestApi.Infrastructure.YouTube;
using SimpleTube.RestApi.Infrastructure.YouTube.Models;

namespace SimpleTube.RestApi.Commands.Internal.DownloadChannelImages;

internal sealed class DownloadChannelImagesTask : ITask
{
    private readonly string _channelId;
    private readonly ConnectionStringProvider _connectionStringProvider;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IImageService _imageService;

    private const string Sql = """
        UPDATE [Channels]
        SET [Banner]    = COALESCE(@banner, [Banner]),
            [Thumbnail] = @thumbnail
        WHERE [Id] = @channelId
        """;

    public DownloadChannelImagesTask(
        string channelId,
        ConnectionStringProvider connectionStringProvider,
        IHttpClientFactory httpClientFactory,
        IImageService imageService
    )
    {
        _channelId = channelId;
        _connectionStringProvider = connectionStringProvider;
        _httpClientFactory = httpClientFactory;
        _imageService = imageService;
    }

    private async ValueTask<string?> DownloadBanner(
        Channel channel,
        CancellationToken cancellationToken
    )
    {
        if (string.IsNullOrWhiteSpace(channel.BrandingSettings?.Image?.BannerUrl))
            return null;

        var banner = await _imageService.DownloadImage(
            $"{channel.BrandingSettings.Image.BannerUrl}=w2120-fcrop64=1,00000000ffffffff-k-c0xffffffff-no-nd-rj",
            "banner",
            [channel.Id],
            cancellationToken
        );
        return banner;
    }

    private async ValueTask<string> DownloadThumbnail(
        Channel channel,
        CancellationToken cancellationToken
    )
    {
        var thumbnail = await _imageService.DownloadImage(
            channel.Snippet.Thumbnails.Max?.Url ?? channel.Snippet.Thumbnails.High.Url,
            "thumb",
            [channel.Id],
            cancellationToken
        );
        return thumbnail;
    }

    public async ValueTask Execute(CancellationToken cancellationToken)
    {
        var channelInfo = await _httpClientFactory
            .CreateYouTubeClient()
            .GetChannelById(_channelId, cancellationToken);
        var banner = await DownloadBanner(channelInfo, cancellationToken);
        var thumbnail = await DownloadThumbnail(channelInfo, cancellationToken);

        await using var connection = new SqliteConnection(
            _connectionStringProvider.ConnectionString
        );
        await connection.OpenAsync(cancellationToken);
        var command = connection.CreateCommand();
        command.CommandText = Sql;
        command.Parameters.AddWithValue("channelId", _channelId);
        command.Parameters.AddWithValue("banner", banner);
        command.Parameters.AddWithValue("thumbnail", thumbnail);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }
}
