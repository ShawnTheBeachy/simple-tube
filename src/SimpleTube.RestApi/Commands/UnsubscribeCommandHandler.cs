using Microsoft.Data.Sqlite;
using SimpleTube.RestApi.Infrastructure.Database;
using SimpleTube.RestApi.Infrastructure.Mediator;

namespace SimpleTube.RestApi.Commands;

internal sealed class UnsubscribeCommandHandler
    : ICommandHandler<UnsubscribeCommand, UnsubscribeCommand.Result>
{
    private readonly ConnectionStringProvider _connectionStringProvider;

    private const string Sql = """
        DELETE
        FROM [Channels]
        WHERE [Handle] = @channelHandle
        """;

    public UnsubscribeCommandHandler(ConnectionStringProvider connectionStringProvider)
    {
        _connectionStringProvider = connectionStringProvider;
    }

    public async ValueTask<UnsubscribeCommand.Result> Execute(
        UnsubscribeCommand command,
        CancellationToken cancellationToken
    )
    {
        await using var connection = new SqliteConnection(
            _connectionStringProvider.ConnectionString
        );
        await connection.OpenAsync(cancellationToken);
        var sqlCommand = connection.CreateCommand();
        sqlCommand.CommandText = Sql;
        sqlCommand.Parameters.AddWithValue("channelHandle", command.ChannelHandle);
        await sqlCommand.ExecuteNonQueryAsync(cancellationToken);
        return new UnsubscribeCommand.Result { ChannelHandle = command.ChannelHandle };
    }
}
