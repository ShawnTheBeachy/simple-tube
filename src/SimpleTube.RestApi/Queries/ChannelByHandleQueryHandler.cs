using Microsoft.Data.Sqlite;
using SimpleTube.RestApi.Exceptions;
using SimpleTube.RestApi.Infrastructure.Database;
using SimpleTube.RestApi.Infrastructure.Mediator;

namespace SimpleTube.RestApi.Queries;

internal sealed class ChannelByHandleQueryHandler
    : IQueryHandler<ChannelByHandleQuery, ChannelByHandleQuery.Result>
{
    private readonly ConnectionStringProvider _connectionStringProvider;

    private const string Sql = """
        SELECT [Handle],
               [Id],
               [Name],
               [Thumbnail],
               [Banner],
               unixepoch([CreatedAt]),
               unixepoch([LastModifiedAt])
        FROM [Channels]
        WHERE [Handle] = @channelHandle
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
                Handle = reader.GetString(0),
                Id = reader.GetString(1),
                Name = reader.GetString(2),
                Thumbnail = reader.GetString(3),
                Banner = reader.IsDBNull(4) ? null : reader.GetString(4),
                CreatedAt = DateTimeOffset.FromUnixTimeSeconds(reader.GetInt64(5)),
                LastModifiedAt = DateTimeOffset.FromUnixTimeSeconds(reader.GetInt64(6)),
            };
            return result;
        }

        throw new NotFoundException();
    }
}
