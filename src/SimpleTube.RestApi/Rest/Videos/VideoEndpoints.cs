using Microsoft.AspNetCore.Mvc;
using SimpleTube.RestApi.Commands;
using SimpleTube.RestApi.Infrastructure.Mediator;

namespace SimpleTube.RestApi.Rest.Videos;

internal static class VideoEndpoints
{
    public static IEndpointRouteBuilder MapVideoEndpoints(this IEndpointRouteBuilder builder)
    {
        var group = builder.MapGroup("/videos");
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
        return builder;
    }
}
