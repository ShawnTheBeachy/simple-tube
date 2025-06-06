using SimpleTube.Shared.Mediator;
using SimpleTube.Shared.Queries;

namespace SimpleTube.RestApi.Queries;

internal static class DependencyInjection
{
    public static IServiceCollection AddQueries(this IServiceCollection services) =>
        services
            .AddTransient<
                IQueryHandler<
                    SubscriptionByChannelHandleQuery,
                    SubscriptionByChannelHandleQuery.Result
                >,
                SubscriptionByChannelHandleQueryHandler
            >()
            .AddTransient<
                IQueryHandler<SubscriptionsQuery, SubscriptionsQuery.Result>,
                SubscriptionsQueryHandler
            >();
}
