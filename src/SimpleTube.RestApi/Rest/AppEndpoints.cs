using SimpleTube.RestApi.Rest.Subscriptions;

namespace SimpleTube.RestApi.Rest;

internal static class AppEndpoints
{
    public static IEndpointRouteBuilder MapAppEndpoints(this IEndpointRouteBuilder builder)
    {
        builder.MapSubscriptionEndpoints();
        return builder;
    }
}
