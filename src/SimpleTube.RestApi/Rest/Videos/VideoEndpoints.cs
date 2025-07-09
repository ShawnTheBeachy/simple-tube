using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SimpleTube.RestApi.Commands;
using SimpleTube.RestApi.Infrastructure.Mediator;
using SimpleTube.RestApi.Queries;
using SimpleTube.RestApi.Rest.Entities;

namespace SimpleTube.RestApi.Rest.Videos;

internal static class VideoEndpoints
{
    public static IEndpointRouteBuilder MapVideoEndpoints(this IEndpointRouteBuilder builder)
    {
        const string groupName = "videos";
        var group = builder.MapGroup(groupName);
        group.MapPut(
            "/downloaded",
            async (
                [FromBody] DownloadVideoCommand request,
                IMediator mediator,
                CancellationToken cancellationToken
            ) =>
            {
                await mediator.Execute<DownloadVideoCommand, DownloadVideoCommand.Result>(
                    request,
                    cancellationToken
                );
                return TypedResults.Ok();
            }
        );
        group.MapPut(
            "/ignored",
            async (
                [FromBody] IgnoreVideoCommand request,
                IMediator mediator,
                CancellationToken cancellationToken
            ) =>
            {
                await mediator.Execute<IgnoreVideoCommand, IgnoreVideoCommand.Result>(
                    request,
                    cancellationToken
                );
                return TypedResults.Ok();
            }
        );
        group.MapGet(
            "/{videoId}/streams/{streamId}",
            async ValueTask<Results<NotFound, FileStreamHttpResult>> (
                string videoId,
                string streamId,
                IMediator mediator,
                CancellationToken cancellationToken
            ) =>
            {
                var stream = await mediator.Query<VideoStreamQuery, Stream?>(
                    new VideoStreamQuery { Type = streamId, VideoId = videoId },
                    cancellationToken
                );
                return stream is null ? TypedResults.NotFound() : TypedResults.Stream(stream);
            }
        );
        group.MapGet(
            "/{videoId}",
            async (string videoId, IMediator mediator, CancellationToken cancellationToken) =>
            {
                var result = await mediator.Query<VideoByIdQuery, VideoByIdQuery.Result>(
                    new VideoByIdQuery(videoId),
                    cancellationToken
                );
                return new Video
                {
                    Channel = new Channel
                    {
                        Handle = result.ChannelHandle,
                        Id = result.ChannelId,
                        Name = result.ChannelName,
                        Url = $"channels/{result.ChannelHandle}",
                    },
                    Description = result.Description,
                    EmbedUrl = result.Embed,
                    Id = result.Id,
                    Streams =
                        result.Streams.Count < 1
                            ? null
                            : result
                                .Streams.DistinctBy(x => x.Type)
                                .Select(stream => new VideoStream
                                {
                                    Type = stream.Type,
                                    Url = $"{groupName}/{result.Id}/streams/{stream.Id}",
                                })
                                .ToArray(),
                    Thumbnail = result.Thumbnail,
                    Title = result.Title,
                    Url = $"{groupName}/{result.Id}",
                };
            }
        );
        return builder;
    }
}
