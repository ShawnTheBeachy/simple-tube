using Microsoft.AspNetCore.Mvc;
using SimpleTube.RestApi.Commands;
using SimpleTube.RestApi.Infrastructure.Mediator;
using SimpleTube.RestApi.Queries;

namespace SimpleTube.RestApi.Rest.Channels;

internal static class ChannelEndpoints
{
    public static IEndpointRouteBuilder MapChannelEndpoints(this IEndpointRouteBuilder builder)
    {
        const string groupName = "channels";
        var group = builder.MapGroup($"/{groupName}");
        group
            .MapGet(
                "/",
                async (IMediator mediator, CancellationToken cancellationToken) =>
                {
                    var result = await mediator.Query<ChannelsQuery, ChannelsQuery.Result>(
                        new ChannelsQuery(),
                        cancellationToken
                    );
                    return result
                        .Channels.Select(channel => new Channel
                        {
                            Handle = channel.ChannelHandle,
                            Id = channel.ChannelId,
                            Name = channel.ChannelName,
                            Thumbnail = channel.ChannelThumbnail,
                            UnwatchedVideos = channel.UnwatchedVideos,
                            Url = $"{groupName}/{channel.ChannelHandle}",
                        })
                        .ToArray();
                }
            )
            .WithName("Get all channels")
            .WithTags("channels");
        group
            .MapGet(
                "/{channelHandle}",
                async (
                    string channelHandle,
                    IMediator mediator,
                    CancellationToken cancellationToken
                ) =>
                {
                    var result = await mediator.Query<
                        ChannelByHandleQuery,
                        ChannelByHandleQuery.Result
                    >(
                        new ChannelByHandleQuery { ChannelHandle = channelHandle },
                        cancellationToken
                    );
                    return new Channel
                    {
                        Handle = result.ChannelHandle,
                        Id = result.ChannelId,
                        Name = result.ChannelName,
                        Thumbnail = result.ChannelThumbnail,
                        CreatedAt = result.CreatedAt,
                        LastModifiedAt = result.LastModifiedAt,
                        Url = $"{groupName}/{result.ChannelHandle}",
                    };
                }
            )
            .CacheOutput()
            .WithName("Get channel by handle")
            .WithTags("channels");
        group
            .MapPut(
                "/",
                async (
                    [FromBody] SubscribeCommand command,
                    IMediator mediator,
                    CancellationToken cancellationToken
                ) =>
                {
                    var result = await mediator.Execute<SubscribeCommand, SubscribeCommand.Result>(
                        command,
                        cancellationToken
                    );
                    return new Channel
                    {
                        Handle = result.ChannelHandle,
                        Id = result.ChannelId,
                        Name = result.ChannelName,
                        Thumbnail = result.ChannelThumbnail,
                        Url = $"{groupName}/{result.ChannelHandle}",
                    };
                }
            )
            .WithName("Subscribe to channel")
            .WithTags("channels");
        group
            .MapDelete(
                "/{channelHandle}",
                async (
                    string channelHandle,
                    IMediator mediator,
                    CancellationToken cancellationToken
                ) =>
                {
                    _ = await mediator.Execute<UnsubscribeCommand, UnsubscribeCommand.Result>(
                        new UnsubscribeCommand { ChannelHandle = channelHandle },
                        cancellationToken
                    );
                    return TypedResults.Ok();
                }
            )
            .WithName("Unsubscribe from channel")
            .WithTags("channels");
        return builder;
    }
}
