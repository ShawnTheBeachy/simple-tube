using Microsoft.AspNetCore.Mvc;
using SimpleTube.RestApi.Commands;
using SimpleTube.RestApi.Extensions;
using SimpleTube.RestApi.Infrastructure.Mediator;
using SimpleTube.RestApi.Queries;
using SimpleTube.RestApi.Rest.Entities;

namespace SimpleTube.RestApi.Rest.Subscriptions;

internal static class SubscriptionEndpoints
{
    public static IEndpointRouteBuilder MapSubscriptionEndpoints(this IEndpointRouteBuilder builder)
    {
        const string groupName = "subscriptions";
        var group = builder.MapGroup($"/{groupName}");
        group
            .MapGet(
                "/",
                async (
                    IMediator mediator,
                    IHttpContextAccessor httpContextAccessor,
                    CancellationToken cancellationToken
                ) =>
                {
                    var result = await mediator.Query<
                        SubscriptionsQuery,
                        SubscriptionsQuery.Result
                    >(new SubscriptionsQuery(), cancellationToken);
                    return result
                        .Channels.Select(channel => new Channel
                        {
                            Handle = channel.ChannelHandle,
                            Id = channel.ChannelId,
                            Name = channel.ChannelName,
                            Thumbnail = channel.ChannelThumbnail.ServerImageUrl(
                                httpContextAccessor
                            ),
                            UnsubscribeUrl = $"{groupName}/{channel.ChannelHandle}",
                            UnwatchedVideos = channel.UnwatchedVideos,
                            Url = $"channels/{channel.ChannelHandle}",
                        })
                        .ToArray();
                }
            )
            .WithName("Get subscribed channels")
            .WithTags("channels", "subscriptions");
        group
            .MapPut(
                "/",
                async (
                    [FromBody] SubscribeCommand command,
                    IMediator mediator,
                    IHttpContextAccessor httpContextAccessor,
                    CancellationToken cancellationToken
                ) =>
                {
                    var result = await mediator.Execute<SubscribeCommand, SubscribeCommand.Result>(
                        command,
                        cancellationToken
                    );
                    return new Channel
                    {
                        Banner = result.ChannelBanner.ServerImageUrl(httpContextAccessor),
                        Handle = result.ChannelHandle,
                        Id = result.ChannelId,
                        Name = result.ChannelName,
                        Thumbnail = result.ChannelThumbnail.ServerImageUrl(httpContextAccessor),
                        Url = $"{groupName}/{result.ChannelHandle}",
                    };
                }
            )
            .WithName("Subscribe to channel")
            .WithTags("channels", "subscriptions");
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
            .WithTags("channels", "subscriptions");
        return builder;
    }
}
