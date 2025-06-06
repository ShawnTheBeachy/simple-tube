using SimpleTube.RestApi.Infrastructure.Database;

namespace SimpleTube.RestApi.Infrastructure;

internal static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration
    ) => services.AddDatabase(configuration);
}
