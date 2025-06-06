using Microsoft.AspNetCore.Mvc;
using SimpleTube.Shared.Commands;
using SimpleTube.Shared.Mediator;
using SimpleTube.Shared.Queries;

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
                        .Channels.Select(subscription => new Channel
                        {
                            ChannelHandle = subscription.ChannelHandle,
                            ChannelId = subscription.ChannelId,
                            ChannelName = subscription.ChannelName,
                            ChannelThumbnail = subscription.ChannelThumbnail,
                            Url = $"{groupName}/{subscription.ChannelHandle}",
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
                        ChannelHandle = result.ChannelHandle,
                        ChannelId = result.ChannelId,
                        ChannelName = result.ChannelName,
                        ChannelThumbnail = result.ChannelThumbnail,
                        CreatedAt = result.CreatedAt,
                        LastModifiedAt = result.LastModifiedAt,
                        Url = $"{groupName}/{result.ChannelHandle}",
                    };
                }
            )
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
                        ChannelHandle = result.ChannelHandle,
                        ChannelId = result.ChannelId,
                        ChannelName = result.ChannelName,
                        ChannelThumbnail = result.ChannelThumbnail,
                        Url = $"{groupName}/{result.ChannelHandle}",
                    };
                }
            )
            .WithName("Subscribe to channel")
            .WithTags("channels");
        return builder;
    }
}
