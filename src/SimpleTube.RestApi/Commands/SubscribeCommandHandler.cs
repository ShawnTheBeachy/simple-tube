using Microsoft.Data.Sqlite;
using SimpleTube.RestApi.Infrastructure.Database;
using SimpleTube.RestApi.Infrastructure.Database.Entities;
using SimpleTube.RestApi.Infrastructure.YouTube;
using SimpleTube.RestApi.Infrastructure.YouTube.Models;
using SimpleTube.Shared.Commands;
using SimpleTube.Shared.Mediator;

namespace SimpleTube.RestApi.Commands;

internal sealed class SubscribeCommandHandler
    : ICommandHandler<SubscribeCommand, SubscribeCommand.Result>
{
    private readonly ConnectionStringProvider _connectionStringProvider;
    private readonly AppDbContext _dbContext;
    private readonly IHttpClientFactory _httpClientFactory;

    private const string GetExistingSql = """
        SELECT [ChannelHandle],
               [ChannelId],
               [ChannelName],
               [ChannelThumbnail]
        FROM [Subscriptions]
        WHERE [ChannelHandle] = $channelHandle
        """;

    public SubscribeCommandHandler(
        ConnectionStringProvider connectionStringProvider,
        AppDbContext dbContext,
        IHttpClientFactory httpClientFactory
    )
    {
        _connectionStringProvider = connectionStringProvider;
        _dbContext = dbContext;
        _httpClientFactory = httpClientFactory;
    }

    public async ValueTask<SubscribeCommand.Result> Execute(
        SubscribeCommand command,
        CancellationToken cancellationToken
    )
    {
        var subscription = await GetExistingSubscription(command, cancellationToken);

        if (subscription is not null)
            return new SubscribeCommand.Result
            {
                ChannelHandle = subscription.ChannelHandle,
                ChannelId = subscription.ChannelId,
                ChannelName = subscription.ChannelName,
                ChannelThumbnail = subscription.ChannelThumbnail,
            };

        var channelInfo = await GetChannelInfo(command, cancellationToken);
        subscription = new SubscriptionEntity
        {
            ChannelHandle = command.ChannelHandle,
            ChannelId = channelInfo.Id,
            ChannelName = channelInfo.Snippet.Title,
            ChannelThumbnail = channelInfo.Snippet.Thumbnails.High.Url,
        };
        _dbContext.Subscriptions.Add(subscription);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return new SubscribeCommand.Result
        {
            ChannelHandle = subscription.ChannelHandle,
            ChannelId = subscription.ChannelId,
            ChannelName = subscription.ChannelName,
            ChannelThumbnail = subscription.ChannelThumbnail,
        };
    }

    private async ValueTask<Channel> GetChannelInfo(
        SubscribeCommand command,
        CancellationToken cancellationToken
    )
    {
        var response = await _httpClientFactory
            .CreateYouTubeClient()
            .ListChannels(command.ChannelHandle, cancellationToken);
        return response.Items[0];
    }

    private async ValueTask<SubscriptionEntity?> GetExistingSubscription(
        SubscribeCommand command,
        CancellationToken cancellationToken
    )
    {
        await using var connection = new SqliteConnection(
            _connectionStringProvider.ConnectionString
        );
        await connection.OpenAsync(cancellationToken);
        var sqlCommand = connection.CreateCommand();
        sqlCommand.CommandText = GetExistingSql;
        sqlCommand.Parameters.AddWithValue("$channelHandle", command.ChannelHandle);

        var reader = await sqlCommand.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            var subscription = new SubscriptionEntity
            {
                ChannelHandle = reader.GetString(0),
                ChannelId = reader.GetString(1),
                ChannelName = reader.GetString(2),
                ChannelThumbnail = reader.GetString(3),
            };
            return subscription;
        }

        return null;
    }
}
