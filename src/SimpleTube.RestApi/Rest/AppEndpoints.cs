using SimpleTube.RestApi.Rest.Channels;

namespace SimpleTube.RestApi.Rest;

internal static class AppEndpoints
{
    public static IEndpointRouteBuilder MapAppEndpoints(this IEndpointRouteBuilder builder)
    {
        builder
            .MapGet(
                "/",
                () =>
                    new Bookmark[]
                    {
                        new() { Name = "Channels", Url = "/channels" },
                    }
            )
            .CacheOutput()
            .WithName("Get bookmarks")
            .WithTags();
        builder.MapChannelEndpoints();
        return builder;
    }

    public sealed record Bookmark
    {
        public required string Name { get; init; }
        public required string Url { get; init; }
    }
}
