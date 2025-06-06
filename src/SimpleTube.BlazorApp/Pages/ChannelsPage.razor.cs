using System.Net.Http.Json;
using System.Text.Json.Serialization;
using SimpleTube.BlazorApp.Infrastructure.Api;

namespace SimpleTube.BlazorApp.Pages;

public sealed partial class ChannelsPage : IAsyncDisposable
{
    private readonly CancellationTokenSource _cancellation = new();
    private readonly IHttpClientFactory _httpClientFactory;

    private IReadOnlyList<Channel> Channels { get; set; } = [];

    public ChannelsPage(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async ValueTask DisposeAsync()
    {
        await _cancellation.CancelAsync();
        _cancellation.Dispose();
    }

    protected override async Task OnInitializedAsync()
    {
        var channels = await _httpClientFactory
            .CreateApiClient()
            .GetFromJsonAsync<IReadOnlyList<Channel>>("channels", _cancellation.Token);
        Channels = channels!;
    }

    private sealed record Channel
    {
        public required string ChannelHandle { get; init; }
        public required string ChannelId { get; init; }
        public required string ChannelName { get; init; }
        public required string ChannelThumbnail { get; init; }

        [JsonPropertyName("$entity#url")]
        public required string Url { get; init; }
    }
}
