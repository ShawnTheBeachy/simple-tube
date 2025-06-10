using Microsoft.Data.Sqlite;

namespace SimpleTube.RestApi.Infrastructure.Database;

internal sealed class SqliteConnectionFactory : IDbConnectionFactory
{
    private readonly ConnectionStringProvider _connectionStringProvider;

    public SqliteConnectionFactory(ConnectionStringProvider connectionStringProvider)
    {
        _connectionStringProvider = connectionStringProvider;
    }

    public async ValueTask<SqliteConnection> CreateConnection(CancellationToken cancellationToken)
    {
        var connection = new SqliteConnection(_connectionStringProvider.ConnectionString);
        await connection.OpenAsync(cancellationToken);
        return connection;
    }
}
