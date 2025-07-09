using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Components;
using SimpleTube.BlazorApp.Infrastructure.Api;
using SimpleTube.BlazorApp.Providers;

namespace SimpleTube.BlazorApp.Pages.VideoPage;

public sealed partial class VideoPage : IAsyncDisposable
{
    private readonly CancellationTokenSource _cancellation = new();
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ServerUrlProvider _serverUrlProvider;

    private VideoInfo? Video { get; set; }

    [Parameter]
    public string VideoId { get; set; } = "";

    public VideoPage(IHttpClientFactory httpClientFactory, ServerUrlProvider serverUrlProvider)
    {
        _httpClientFactory = httpClientFactory;
        _serverUrlProvider = serverUrlProvider;
    }

    public async ValueTask DisposeAsync()
    {
        await _cancellation.CancelAsync();
        _cancellation.Dispose();
    }

    private async Task DownloadVideo()
    {
        var client = _httpClientFactory.CreateApiClient();
        _ = await client.PutAsJsonAsync(
            "videos/downloaded",
            new Request { VideoId = VideoId },
            _cancellation.Token
        );
    }

    protected override async Task OnInitializedAsync()
    {
        var client = _httpClientFactory.CreateApiClient();
        Video = await client.GetFromJsonAsync<VideoInfo>($"videos/{VideoId}", _cancellation.Token);
    }

    private sealed record Request
    {
        public required string VideoId { get; init; }
    }

    private sealed record Stream
    {
        [JsonPropertyName("type")]
        public required string Type { get; init; }

        [JsonPropertyName("$entity#url")]
        public required string Url { get; init; }
    }

    private sealed record VideoInfo
    {
        [JsonPropertyName("channel")]
        public ChannelInfo? Channel { get; init; }

        [JsonPropertyName("description")]
        public required string Description { get; init; }

        [JsonPropertyName("embedUrl")]
        public string? EmbedUrl { get; init; }

        [JsonPropertyName("streams")]
        public IReadOnlyList<Stream> Streams { get; init; } = [];

        [JsonPropertyName("thumbnail")]
        public string? Thumbnail { get; init; }

        [JsonPropertyName("title")]
        public required string Title { get; init; }
    }

    private sealed record ChannelInfo
    {
        [JsonPropertyName("handle")]
        public string? Handle { get; init; }

        [JsonPropertyName("id")]
        public required string Id { get; init; }

        [JsonPropertyName("name")]
        public string? Name { get; init; }

        [JsonPropertyName("$entity#url")]
        public required string Url { get; init; }
    }
}
