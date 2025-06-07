using SimpleTube.RestApi.Infrastructure.Mediator;

namespace SimpleTube.RestApi.Queries;

internal static class DependencyInjection
{
    public static IServiceCollection AddQueries(this IServiceCollection services) =>
        services
            .AddTransient<
                IQueryHandler<ChannelByHandleQuery, ChannelByHandleQuery.Result>,
                ChannelByHandleQueryHandler
            >()
            .AddTransient<
                IQueryHandler<ChannelsQuery, ChannelsQuery.Result>,
                ChannelsQueryHandler
            >()
            .AddTransient<
                IQueryHandler<ChannelVideosQuery, ChannelVideosQuery.Result>,
                ChannelVideosQueryHandler
            >();
}
