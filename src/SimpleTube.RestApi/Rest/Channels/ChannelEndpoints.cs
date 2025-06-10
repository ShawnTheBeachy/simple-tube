using SimpleTube.RestApi.Extensions;
using SimpleTube.RestApi.Infrastructure.Mediator;
using SimpleTube.RestApi.Queries;
using SimpleTube.RestApi.Rest.Entities;

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
                async (
                    IMediator mediator,
                    IHttpContextAccessor httpContextAccessor,
                    CancellationToken cancellationToken
                ) =>
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
                            Subscribed = channel.Subscribed,
                            Thumbnail = channel.ChannelThumbnail.ServerImageUrl(
                                httpContextAccessor
                            ),
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
                    IHttpContextAccessor httpContextAccessor,
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
                        Banner = result.Banner.ServerImageUrl(httpContextAccessor),
                        Handle = result.Handle,
                        Id = result.Id,
                        Name = result.Name,
                        Thumbnail = result.Thumbnail.ServerImageUrl(httpContextAccessor),
                        CreatedAt = result.CreatedAt,
                        LastModifiedAt = result.LastModifiedAt,
                        Url = $"{groupName}/{result.Handle}",
                    };
                }
            )
            .CacheOutput()
            .WithName("Get channel by handle")
            .WithTags("channels");
        group
            .MapGet(
                "/{channelHandle}/videos",
                async (
                    string channelHandle,
                    IMediator mediator,
                    IHttpContextAccessor httpContextAccessor,
                    CancellationToken cancellationToken
                ) =>
                {
                    var result = await mediator.Query<
                        ChannelVideosQuery,
                        ChannelVideosQuery.Result
                    >(new ChannelVideosQuery { ChannelHandle = channelHandle }, cancellationToken);
                    return result
                        .Videos.Select(video => new Video
                        {
                            Id = video.Id,
                            PublishedAt = video.PublishedAt,
                            Thumbnail = video.Thumbnail.ServerImageUrl(httpContextAccessor),
                            Title = video.Title,
                            Url = $"/videos/{video.Id}",
                        })
                        .ToArray();
                }
            )
            .CacheOutput()
            .WithName("Get channel videos")
            .WithTags("channels", "videos");
        return builder;
    }
}
