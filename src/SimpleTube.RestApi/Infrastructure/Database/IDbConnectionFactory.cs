using Microsoft.Data.Sqlite;

namespace SimpleTube.RestApi.Infrastructure.Database;

public interface IDbConnectionFactory
{
    ValueTask<SqliteConnection> CreateConnection(CancellationToken cancellationToken);
}
