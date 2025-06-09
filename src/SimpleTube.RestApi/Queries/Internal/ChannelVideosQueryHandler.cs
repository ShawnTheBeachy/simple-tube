using Microsoft.Data.Sqlite;
using SimpleTube.RestApi.Infrastructure.Database;
using SimpleTube.RestApi.Infrastructure.Mediator;

namespace SimpleTube.RestApi.Queries.Internal;

internal sealed class ChannelVideosQueryHandler
    : IQueryHandler<ChannelVideosQuery, ChannelVideosQuery.Result>
{
    private readonly ConnectionStringProvider _connectionStringProvider;

    private const string Sql = """
        SELECT [Videos].[Id],
               [Videos].[Title],
               [Videos].[Thumbnail],
               [Videos].[PublishedAt]
        FROM [Videos]
        INNER JOIN [Channels]
            ON [Channels].[Id] = [Videos].[ChannelId]
            AND [Channels].[Handle] = @channelHandle
        WHERE [Videos].[Ignored] = 0
        ORDER BY [PublishedAt] DESC
        """;

    public ChannelVideosQueryHandler(ConnectionStringProvider connectionStringProvider)
    {
        _connectionStringProvider = connectionStringProvider;
    }

    public async ValueTask<ChannelVideosQuery.Result> Execute(
        ChannelVideosQuery query,
        CancellationToken cancellationToken
    )
    {
        await using var connection = new SqliteConnection(
            _connectionStringProvider.ConnectionString
        );
        await connection.OpenAsync(cancellationToken);
        var command = connection.CreateCommand();
        command.CommandText = Sql;
        command.Parameters.AddWithValue("channelHandle", query.ChannelHandle);

        var reader = await command.ExecuteReaderAsync(cancellationToken);
        var videos = new List<ChannelVideosQuery.Result.Video>();

        while (await reader.ReadAsync(cancellationToken))
        {
            videos.Add(
                new ChannelVideosQuery.Result.Video
                {
                    Id = reader.GetString(0),
                    Thumbnail = reader.GetString(2),
                    Title = reader.GetString(1),
                    PublishedAt = reader.GetDateTime(3),
                }
            );
        }

        return new ChannelVideosQuery.Result { Videos = videos.ToArray() };
    }
}
