using SimpleTube.RestApi.Infrastructure.Downloads.Internal;

namespace SimpleTube.RestApi.Infrastructure.Downloads;

internal static class DependencyInjection
{
    public static IServiceCollection AddDownloads(this IServiceCollection services) =>
        services.AddTransient<IDownloadsManager, DownloadsManager>();
}
