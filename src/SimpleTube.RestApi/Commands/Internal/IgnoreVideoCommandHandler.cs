using Microsoft.Data.Sqlite;
using SimpleTube.RestApi.Infrastructure.Database;
using SimpleTube.RestApi.Infrastructure.Mediator;

namespace SimpleTube.RestApi.Commands.Internal;

internal sealed class IgnoreVideoCommandHandler
    : ICommandHandler<IgnoreVideoCommand, IgnoreVideoCommand.Result>
{
    private readonly ConnectionStringProvider _connectionStringProvider;

    private const string Sql = """
        UPDATE [Videos]
        SET [Ignored] = 1
        WHERE [Id] = @videoId
        """;

    public IgnoreVideoCommandHandler(ConnectionStringProvider connectionStringProvider)
    {
        _connectionStringProvider = connectionStringProvider;
    }

    public async ValueTask<IgnoreVideoCommand.Result> Execute(
        IgnoreVideoCommand command,
        CancellationToken cancellationToken
    )
    {
        await using var connection = new SqliteConnection(
            _connectionStringProvider.ConnectionString
        );
        await connection.OpenAsync(cancellationToken);
        var sqlCommand = connection.CreateCommand();
        sqlCommand.CommandText = Sql;
        sqlCommand.Parameters.AddWithValue("videoId", command.VideoId);
        await sqlCommand.ExecuteNonQueryAsync(cancellationToken);
        return new IgnoreVideoCommand.Result();
    }
}
