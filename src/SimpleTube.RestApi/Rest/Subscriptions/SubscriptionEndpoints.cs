using Microsoft.AspNetCore.Mvc;
using SimpleTube.Shared.Commands;
using SimpleTube.Shared.Mediator;
using SimpleTube.Shared.Queries;

namespace SimpleTube.RestApi.Rest.Subscriptions;

internal static class SubscriptionEndpoints
{
    public static IEndpointRouteBuilder MapSubscriptionEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("/subscriptions");
        group.MapGet(
            "/",
            async (IMediator mediator, CancellationToken cancellationToken) =>
            {
                var result = await mediator.Query<SubscriptionsQuery, SubscriptionsQuery.Result>(
                    new SubscriptionsQuery(),
                    cancellationToken
                );
                return result
                    .Subscriptions.Select(subscription => new RestEntity<SubscriptionRestEntity>
                    {
                        Entity = new SubscriptionRestEntity
                        {
                            ChannelHandle = subscription.ChannelHandle,
                            ChannelId = subscription.ChannelId,
                            ChannelName = subscription.ChannelName,
                            ChannelThumbnail = subscription.ChannelThumbnail,
                        },
                        Url = $"subscriptions/{subscription.ChannelHandle}",
                    })
                    .ToArray();
            }
        );
        group.MapGet(
            "/{channelHandle}",
            async (string channelHandle, IMediator mediator, CancellationToken cancellationToken) =>
            {
                var result = await mediator.Query<
                    SubscriptionByChannelHandleQuery,
                    SubscriptionByChannelHandleQuery.Result
                >(
                    new SubscriptionByChannelHandleQuery { ChannelHandle = channelHandle },
                    cancellationToken
                );
                return new RestEntity<SubscriptionRestEntity>
                {
                    Entity = new SubscriptionRestEntity
                    {
                        ChannelHandle = result.ChannelHandle,
                        ChannelId = result.ChannelId,
                        ChannelName = result.ChannelName,
                        ChannelThumbnail = result.ChannelThumbnail,
                        CreatedAt = result.CreatedAt,
                        LastModifiedAt = result.LastModifiedAt,
                    },
                    Url = $"subscriptions/{result.ChannelHandle}",
                };
            }
        );
        group.MapPut(
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
                return new RestEntity<SubscriptionRestEntity>
                {
                    Entity = new SubscriptionRestEntity
                    {
                        ChannelHandle = result.ChannelHandle,
                        ChannelId = result.ChannelId,
                        ChannelName = result.ChannelName,
                        ChannelThumbnail = result.ChannelThumbnail,
                    },
                    Url = $"subscriptions/{result.ChannelHandle}",
                };
            }
        );
        return builder;
    }
}
