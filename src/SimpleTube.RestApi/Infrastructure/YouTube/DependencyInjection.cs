using SimpleTube.RestApi.Extensions;

namespace SimpleTube.RestApi.Infrastructure.YouTube;

internal static class DependencyInjection
{
    public static IServiceCollection AddYouTube(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services
            .AddOptions<YouTubeOptions>()
            .Bind(configuration.GetRequiredSection(YouTubeOptions.SectionName))
            .ValidateWithFluentValidation()
            .ValidateOnStart();
        services.AddYouTubeHttpClient();
        return services;
    }
}
