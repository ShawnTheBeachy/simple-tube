using Microsoft.Data.Sqlite;
using SimpleTube.RestApi.Infrastructure.Database;
using SimpleTube.Shared.Mediator;
using SimpleTube.Shared.Queries;

namespace SimpleTube.RestApi.Queries;

internal sealed class SubscriptionsQueryHandler
    : IQueryHandler<SubscriptionsQuery, SubscriptionsQuery.Result>
{
    private readonly ConnectionStringProvider _connectionStringProvider;

    private const string Sql = """
        SELECT [ChannelHandle],
               [ChannelId],
               [ChannelName],
               [ChannelThumbnail]
        FROM [Subscriptions]
        """;

    public SubscriptionsQueryHandler(ConnectionStringProvider connectionStringProvider)
    {
        _connectionStringProvider = connectionStringProvider;
    }

    public async ValueTask<SubscriptionsQuery.Result> Execute(
        SubscriptionsQuery query,
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

        var subscriptions = new List<SubscriptionsQuery.Result.Subscription>();

        while (await reader.ReadAsync(cancellationToken))
        {
            subscriptions.Add(
                new SubscriptionsQuery.Result.Subscription
                {
                    ChannelHandle = reader.GetString(0),
                    ChannelId = reader.GetString(1),
                    ChannelName = reader.GetString(2),
                    ChannelThumbnail = reader.GetString(3),
                }
            );
        }

        return new SubscriptionsQuery.Result { Subscriptions = subscriptions.ToArray() };
    }
}
