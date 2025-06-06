namespace SimpleTube.BlazorApp.Infrastructure.Api;

internal static class ApiClient
{
    private const string ClientName = "ApiClient";

    public static IServiceCollection AddApiClient(this IServiceCollection services)
    {
        services.AddHttpClient(
            ClientName,
            client =>
            {
                client.BaseAddress = new Uri("http://localhost:5190/");
            }
        );
        return services;
    }

    public static HttpClient CreateApiClient(this IHttpClientFactory httpClientFactory) =>
        httpClientFactory.CreateClient(ClientName);
}
