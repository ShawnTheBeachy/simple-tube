using Microsoft.Data.Sqlite;
using SimpleTube.RestApi.Exceptions;
using SimpleTube.RestApi.Infrastructure.Database;
using SimpleTube.RestApi.Infrastructure.Mediator;
using SimpleTube.RestApi.Infrastructure.YouTube;
using SimpleTube.RestApi.Infrastructure.YouTube.Models;
using SimpleTube.RestApi.Messages;
using SlimMessageBus;

namespace SimpleTube.RestApi.Commands.Internal;

internal sealed class SubscribeCommandHandler
    : ICommandHandler<SubscribeCommand, SubscribeCommand.Result>
{
    private readonly ConnectionStringProvider _connectionStringProvider;
    private readonly IDbConnectionFactory _dbConnectionFactory;
    private readonly IHttpClientFactory _httpClientFactory;

    public SubscribeCommandHandler(
        ConnectionStringProvider connectionStringProvider,
        IDbConnectionFactory dbConnectionFactory,
        IHttpClientFactory httpClientFactory
    )
    {
        _connectionStringProvider = connectionStringProvider;
        _dbConnectionFactory = dbConnectionFactory;
        _httpClientFactory = httpClientFactory;
    }

    public async ValueTask<SubscribeCommand.Result> Execute(
        SubscribeCommand command,
        CancellationToken cancellationToken
    )
    {
        var result = await GetExistingChannel(command, cancellationToken);

        if (result is not null)
        {
            await SubscribeToExisting(command, cancellationToken);
            return result;
        }

        var channelInfo = await GetChannelInfo(command, cancellationToken);
        result = await SubscribeToNew(command, channelInfo, cancellationToken);
        await MessageBus.Current.Publish(
            new SubscribedToChannelMessage { ChannelId = channelInfo.Id },
            cancellationToken: cancellationToken
        );
        return result;
    }

    private async ValueTask<Channel> GetChannelInfo(
        SubscribeCommand command,
        CancellationToken cancellationToken
    )
    {
        var response = await _httpClientFactory
            .CreateYouTubeClient()
            .ListChannels(command.ChannelHandle, cancellationToken);

        if (response.Items.Count < 1)
            throw new NotFoundException();

        return response.Items[0];
    }

    private async ValueTask<SubscribeCommand.Result?> GetExistingChannel(
        SubscribeCommand command,
        CancellationToken cancellationToken
    )
    {
        await using var connection = new SqliteConnection(
            _connectionStringProvider.ConnectionString
        );
        await connection.OpenAsync(cancellationToken);
        var sqlCommand = connection.CreateCommand();
        sqlCommand.CommandText = SqlQueries.GetExisting;
        sqlCommand.Parameters.AddWithValue("@channelHandle", command.ChannelHandle);

        var reader = await sqlCommand.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            var result = new SubscribeCommand.Result
            {
                ChannelHandle = reader.GetString(0),
                ChannelId = reader.GetString(1),
                ChannelName = reader.GetString(2),
                ChannelThumbnail = reader.GetString(3),
                ChannelBanner = reader.GetString(4),
            };
            return result;
        }

        return null;
    }

    private async ValueTask SubscribeToExisting(
        SubscribeCommand command,
        CancellationToken cancellationToken
    )
    {
        await using var connection = await _dbConnectionFactory.CreateConnection(cancellationToken);
        var sqlCommand = connection.CreateCommand();
        sqlCommand.CommandText = SqlQueries.SubscribeToExisting;
        sqlCommand.Parameters.AddWithValue("channelHandle", command.ChannelHandle);
        await sqlCommand.ExecuteNonQueryAsync(cancellationToken);
    }

    private async ValueTask<SubscribeCommand.Result> SubscribeToNew(
        SubscribeCommand command,
        Channel channel,
        CancellationToken cancellationToken
    )
    {
        var result = new SubscribeCommand.Result
        {
            ChannelBanner = channel.BrandingSettings?.Image?.BannerUrl,
            ChannelHandle = command.ChannelHandle,
            ChannelId = channel.Id,
            ChannelName = channel.Snippet.Title,
            ChannelThumbnail =
                channel.Snippet.Thumbnails.Max?.Url ?? channel.Snippet.Thumbnails.High.Url,
        };

        await using var connection = await _dbConnectionFactory.CreateConnection(cancellationToken);
        var sqlCommand = connection.CreateCommand();
        sqlCommand.CommandText = SqlQueries.Create;
        sqlCommand.Parameters.AddWithValue("banner", result.ChannelBanner);
        sqlCommand.Parameters.AddWithValue("handle", result.ChannelHandle);
        sqlCommand.Parameters.AddWithValue("id", result.ChannelId);
        sqlCommand.Parameters.AddWithValue("name", result.ChannelName);
        sqlCommand.Parameters.AddWithValue("now", DateTimeOffset.Now);
        sqlCommand.Parameters.AddWithValue("thumbnail", result.ChannelThumbnail);
        await sqlCommand.ExecuteNonQueryAsync(cancellationToken);
        return result;
    }

    private static class SqlQueries
    {
        public const string Create = """
            INSERT INTO [Channels]
                ([Banner], [CreatedAt], [Handle], [Id], [Name], [Thumbnail], [LastModifiedAt], [Subscribed])
            VALUES (@banner, @now, @handle, @id, @name, @thumbnail, @now, 1)
            """;

        public const string GetExisting = """
            SELECT [Handle],
                   [Id],
                   [Name],
                   [Thumbnail],
                   [Banner]
            FROM [Channels]
            WHERE [Handle] = @channelHandle
            """;

        public const string SubscribeToExisting = """
            UPDATE [Channels]
            SET [Subscribed]     = 1,
                [LastModifiedAt] = GETDATE()
            WHERE [Handle] = @channelHandle                                    
            """;
    }
}
