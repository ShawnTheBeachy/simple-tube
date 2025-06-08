using SimpleTube.BlazorApp.Providers;

namespace SimpleTube.BlazorApp.Infrastructure.Api;

internal static class ApiClient
{
    private const string ClientName = "ApiClient";

    public static IServiceCollection AddApiClient(this IServiceCollection services)
    {
        services.AddHttpClient(
            ClientName,
            (sp, client) =>
            {
                client.BaseAddress = new Uri(sp.GetRequiredService<ServerUrlProvider>().ServerUrl);
            }
        );
        return services;
    }

    public static HttpClient CreateApiClient(this IHttpClientFactory httpClientFactory) =>
        httpClientFactory.CreateClient(ClientName);
}
