using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Components;
using SimpleTube.BlazorApp.Infrastructure.Api;

namespace SimpleTube.BlazorApp.Pages.ChannelPage;

public sealed partial class VideosList : IAsyncDisposable
{
    private readonly CancellationTokenSource _cancellation = new();
    private readonly IHttpClientFactory _httpClientFactory;
    private string? _loadedChannelHandle;

    [Parameter, EditorRequired]
    public required string ChannelHandle { get; set; }

    private IReadOnlyList<Video> Videos { get; set; } = [];

    public VideosList(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async ValueTask DisposeAsync()
    {
        await _cancellation.CancelAsync();
        _cancellation.Dispose();
    }

    protected override async Task OnParametersSetAsync()
    {
        if (ChannelHandle == _loadedChannelHandle)
            return;

        _loadedChannelHandle = ChannelHandle;
        var videos = await _httpClientFactory
            .CreateApiClient()
            .GetFromJsonAsync<IReadOnlyList<Video>>(
                $"channels/{ChannelHandle}/videos",
                _cancellation.Token
            );
        Videos = videos!;
    }

    private sealed record Video
    {
        public required string Id { get; init; }
        public required DateTime PublishedAt { get; init; }
        public required string Thumbnail { get; init; }
        public required string Title { get; init; }

        [JsonPropertyName("$entity#url")]
        public required string Url { get; init; }
    }
}
