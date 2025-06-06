using SimpleTube.BlazorApp.Infrastructure.Api;

namespace SimpleTube.BlazorApp.Infrastructure;

internal static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services) =>
        services.AddApiClient();
}
