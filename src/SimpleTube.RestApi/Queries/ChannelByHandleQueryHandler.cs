using Microsoft.Data.Sqlite;
using SimpleTube.RestApi.Exceptions;
using SimpleTube.RestApi.Infrastructure.Database;
using SimpleTube.Shared.Mediator;
using SimpleTube.Shared.Queries;

namespace SimpleTube.RestApi.Queries;

internal sealed class ChannelByHandleQueryHandler
    : IQueryHandler<ChannelByHandleQuery, ChannelByHandleQuery.Result>
{
    private readonly ConnectionStringProvider _connectionStringProvider;

    private const string Sql = """
        SELECT [ChannelHandle],
               [ChannelId],
               [ChannelName],
               [ChannelThumbnail],
               unixepoch([CreatedAt]),
               unixepoch([LastModifiedAt])
        FROM [Subscriptions]
        WHERE [ChannelHandle] = @channelHandle
        """;

    public ChannelByHandleQueryHandler(ConnectionStringProvider connectionStringProvider)
    {
        _connectionStringProvider = connectionStringProvider;
    }

    public async ValueTask<ChannelByHandleQuery.Result> Execute(
        ChannelByHandleQuery query,
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

        while (await reader.ReadAsync(cancellationToken))
        {
            var result = new ChannelByHandleQuery.Result
            {
                ChannelHandle = reader.GetString(0),
                ChannelId = reader.GetString(1),
                ChannelName = reader.GetString(2),
                ChannelThumbnail = reader.GetString(3),
                CreatedAt = DateTimeOffset.FromUnixTimeSeconds(reader.GetInt64(4)),
                LastModifiedAt = DateTimeOffset.FromUnixTimeSeconds(reader.GetInt64(5)),
            };
            return result;
        }

        throw new NotFoundException();
    }
}
