using SimpleTube.RestApi.Rest.Channels;

namespace SimpleTube.RestApi.Rest;

internal static class AppEndpoints
{
    public static IEndpointRouteBuilder MapAppEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapChannelEndpoints();
        return builder;
    }
}
