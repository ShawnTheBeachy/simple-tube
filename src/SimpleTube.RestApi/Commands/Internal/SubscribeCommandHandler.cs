using Microsoft.Data.Sqlite;
using SimpleTube.RestApi.Exceptions;
using SimpleTube.RestApi.Infrastructure.Database;
using SimpleTube.RestApi.Infrastructure.Database.Entities;
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
    private readonly AppDbContext _dbContext;
    private readonly IHttpClientFactory _httpClientFactory;

    private const string GetExistingSql = """
        SELECT [Handle],
               [Id],
               [Name],
               [Thumbnail],
               [Banner]
        FROM [Channels]
        WHERE [Handle] = @channelHandle
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
        var channel = await GetExistingChannel(command, cancellationToken);

        if (channel is not null)
            return new SubscribeCommand.Result
            {
                ChannelBanner = channel.Banner,
                ChannelHandle = channel.Handle,
                ChannelId = channel.Id,
                ChannelName = channel.Name,
                ChannelThumbnail = channel.Thumbnail,
            };

        var channelInfo = await GetChannelInfo(command, cancellationToken);
        channel = new ChannelEntity
        {
            Banner = channelInfo.BrandingSettings?.Image?.BannerUrl,
            Handle = command.ChannelHandle,
            Id = channelInfo.Id,
            Name = channelInfo.Snippet.Title,
            Thumbnail =
                channelInfo.Snippet.Thumbnails.Max?.Url ?? channelInfo.Snippet.Thumbnails.High.Url,
        };
        _dbContext.Channels.Add(channel);
        await _dbContext.SaveChangesAsync(cancellationToken);
        await MessageBus.Current.Publish(
            new SubscribedToChannelMessage { ChannelId = channel.Id },
            cancellationToken: cancellationToken
        );
        return new SubscribeCommand.Result
        {
            ChannelBanner = channel.Banner,
            ChannelHandle = channel.Handle,
            ChannelId = channel.Id,
            ChannelName = channel.Name,
            ChannelThumbnail = channel.Thumbnail,
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

        if (response.Items.Count < 1)
            throw new NotFoundException();

        return response.Items[0];
    }

    private async ValueTask<ChannelEntity?> GetExistingChannel(
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
        sqlCommand.Parameters.AddWithValue("@channelHandle", command.ChannelHandle);

        var reader = await sqlCommand.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            var subscription = new ChannelEntity
            {
                Handle = reader.GetString(0),
                Id = reader.GetString(1),
                Name = reader.GetString(2),
                Thumbnail = reader.GetString(3),
                Banner = reader.GetString(4),
            };
            return subscription;
        }

        return null;
    }
}
