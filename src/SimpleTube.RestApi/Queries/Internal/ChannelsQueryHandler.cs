using Microsoft.Data.Sqlite;
using SimpleTube.RestApi.Infrastructure.Database;
using SimpleTube.RestApi.Infrastructure.Images;
using SimpleTube.RestApi.Infrastructure.Mediator;

namespace SimpleTube.RestApi.Queries.Internal;

internal sealed class ChannelsQueryHandler : IQueryHandler<ChannelsQuery, ChannelsQuery.Result>
{
    private readonly ConnectionStringProvider _connectionStringProvider;

    private const string Sql = """
        SELECT [Channels].[Handle],
               [Channels].[Id],
               [Channels].[Name],
               [Channels].[Thumbnail],
               COUNT([Videos].[Id])
        FROM [Channels]
        LEFT JOIN [Videos]
            ON [Videos].[ChannelId] = [Channels].[Id]
            AND [Videos].[Watched] = 0
        GROUP BY [Channels].[Handle],
                 [Channels].[Id],
                 [Channels].[Name],
                 [Channels].[Thumbnail],
                 [Channels].[CreatedAt],
                 [Channels].[LastModifiedAt]
        ORDER BY [Name] COLLATE NOCASE
        """;

    public ChannelsQueryHandler(ConnectionStringProvider connectionStringProvider)
    {
        _connectionStringProvider = connectionStringProvider;
    }

    public async ValueTask<ChannelsQuery.Result> Execute(
        ChannelsQuery query,
        CancellationToken cancellationToken
    )
    {
        await using var connection = new SqliteConnection(
            _connectionStringProvider.ConnectionString
        );
        await connection.OpenAsync(cancellationToken);
        var command = connection.CreateCommand();
        command.CommandText = Sql;
        var reader = await command.ExecuteReaderAsync(cancellationToken);

        var channels = new List<ChannelsQuery.Result.Channel>();

        while (await reader.ReadAsync(cancellationToken))
        {
            channels.Add(
                new ChannelsQuery.Result.Channel
                {
                    ChannelHandle = reader.GetString(0),
                    ChannelId = reader.GetString(1),
                    ChannelName = reader.GetString(2),
                    ChannelThumbnail = reader.GetString(3).ImageUrl(),
                    UnwatchedVideos = reader.GetInt16(4),
                }
            );
        }

        return new ChannelsQuery.Result { Channels = channels.ToArray() };
    }
}
