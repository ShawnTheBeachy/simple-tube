namespace SimpleTube.RestApi.Infrastructure.YouTube;

internal static class YouTubeClient
{
    private const string ClientName = "YouTubeClient";

    public static IServiceCollection AddYouTubeHttpClient(this IServiceCollection services)
    {
        services
            .AddTransient<ApiKeyMessageHandler>()
            .AddHttpClient(
                ClientName,
                client =>
                {
                    client.BaseAddress = new Uri("https://www.googleapis.com/youtube/v3/");
                }
            )
            .AddHttpMessageHandler<ApiKeyMessageHandler>();
        return services;
    }

    public static HttpClient CreateYouTubeClient(this IHttpClientFactory httpClientFactory) =>
        httpClientFactory.CreateClient(ClientName);
}
