using Microsoft.Extensions.Options;

namespace SimpleTube.RestApi.Infrastructure.YouTube;

internal sealed class ApiKeyMessageHandler : DelegatingHandler
{
    private readonly IOptions<YouTubeOptions> _options;

    public ApiKeyMessageHandler(IOptions<YouTubeOptions> options)
    {
        _options = options;
    }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken
    )
    {
        var apiKey = _options.Value.ApiKey;
        var url = request.RequestUri?.ToString();

        if (url is null)
            return base.SendAsync(request, cancellationToken);

        url += $"{(url.Contains('?') ? '&' : '?')}key={apiKey}";

        request.RequestUri = new Uri(url);
        return base.SendAsync(request, cancellationToken);
    }
}
