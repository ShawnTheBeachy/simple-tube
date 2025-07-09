using SimpleTube.RestApi.Infrastructure.Database;
using SimpleTube.RestApi.Infrastructure.Downloads;
using SimpleTube.RestApi.Infrastructure.Images;
using SimpleTube.RestApi.Infrastructure.Tasks;
using SimpleTube.RestApi.Infrastructure.YouTube;

namespace SimpleTube.RestApi.Infrastructure;

internal static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration
    ) => services.AddDatabase().AddDownloads().AddImages().AddTasks().AddYouTube(configuration);
}
