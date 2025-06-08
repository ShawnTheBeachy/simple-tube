using Microsoft.Data.Sqlite;
using SimpleTube.RestApi.Infrastructure.Database;
using SimpleTube.RestApi.Infrastructure.Tasks;
using SimpleTube.RestApi.Infrastructure.YouTube;

namespace SimpleTube.RestApi.Commands.Internal.ScanChannel;

internal sealed class ScanChannelTask : ITask
{
    private readonly string _channelId;
    private readonly ConnectionStringProvider _connectionStringProvider;
    private readonly IHttpClientFactory _httpClientFactory;

    private const string Sql = """
        INSERT INTO [Videos]
            ([ChannelId], [Description], [Id], [PublishedAt], [Thumbnail], [Title])
        VALUES
            (@channelId, @description, @id, @publishedAt, @thumbnail, @title)
        ON CONFLICT ([Id]) DO UPDATE
            SET [Description] = @description,
                [Thumbnail]   = @thumbnail,
                [Title]       = @title
        """;

    public ScanChannelTask(
        string channelId,
        ConnectionStringProvider connectionStringProvider,
        IHttpClientFactory httpClientFactory
    )
    {
        _channelId = channelId;
        _connectionStringProvider = connectionStringProvider;
        _httpClientFactory = httpClientFactory;
    }

    public async ValueTask Execute(CancellationToken cancellationToken)
    {
        await using var connection = new SqliteConnection(
            _connectionStringProvider.ConnectionString
        );
        await connection.OpenAsync(cancellationToken);
        var transaction = await connection.BeginTransactionAsync(cancellationToken);

        await foreach (
            var result in _httpClientFactory
                .CreateYouTubeClient()
                .ListChannelVideos(_channelId, cancellationToken)
        )
        {
            var command = connection.CreateCommand();
            command.CommandText = Sql;
            command.Parameters.AddWithValue("channelId", result.Snippet.ChannelId);
            command.Parameters.AddWithValue("description", result.Snippet.Description);
            command.Parameters.AddWithValue("id", result.Id.VideoId);
            command.Parameters.AddWithValue("publishedAt", result.Snippet.PublishedAt);
            command.Parameters.AddWithValue(
                "thumbnail",
                result.Snippet.Thumbnails.Max?.Url ?? result.Snippet.Thumbnails.High.Url
            );
            command.Parameters.AddWithValue("title", result.Snippet.Title);
            await command.ExecuteNonQueryAsync(cancellationToken);
        }

        await transaction.CommitAsync(cancellationToken);
    }
}
