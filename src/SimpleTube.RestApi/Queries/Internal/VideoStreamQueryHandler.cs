using Microsoft.Data.Sqlite;
using SimpleTube.RestApi.Infrastructure.Database;
using SimpleTube.RestApi.Infrastructure.Downloads;
using SimpleTube.RestApi.Infrastructure.Mediator;

namespace SimpleTube.RestApi.Queries.Internal;

internal sealed class VideoStreamQueryHandler : IQueryHandler<VideoStreamQuery, Stream?>
{
    private readonly ConnectionStringProvider _connectionStringProvider;
    private readonly IDownloadsManager _downloadsManager;

    public VideoStreamQueryHandler(
        ConnectionStringProvider connectionStringProvider,
        IDownloadsManager downloadsManager
    )
    {
        _connectionStringProvider = connectionStringProvider;
        _downloadsManager = downloadsManager;
    }

    public async ValueTask<Stream?> Execute(
        VideoStreamQuery query,
        CancellationToken cancellationToken
    )
    {
        await using var connection = new SqliteConnection(
            _connectionStringProvider.ConnectionString
        );
        await connection.OpenAsync(cancellationToken);
        var command = connection.CreateCommand();
        command.CommandText = """
            SELECT [ChannelId]
            FROM [Videos]
            WHERE [Id] = @id
            """;
        command.Parameters.AddWithValue("id", query.VideoId);
        var channelIdObj = await command.ExecuteScalarAsync(cancellationToken);
        return channelIdObj is not string channelId
            ? null
            : _downloadsManager.GetDownload(channelId, query.VideoId, query.Type);
    }
}
