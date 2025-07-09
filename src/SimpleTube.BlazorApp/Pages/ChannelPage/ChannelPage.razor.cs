using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using SimpleTube.BlazorApp.Infrastructure.Api;

namespace SimpleTube.BlazorApp.Pages.ChannelPage;

public sealed partial class ChannelPage : IAsyncDisposable
{
    private readonly CancellationTokenSource _cancellation = new();
    private readonly IHttpClientFactory _httpClientFactory;

    [Parameter]
    public string ChannelHandle { get; set; } = "";

    private Channel? ChannelInfo { get; set; }

    public ChannelPage(IHttpClientFactory httpClientFactory)
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
        if (ChannelInfo is not null && ChannelHandle == ChannelInfo.Handle)
            return;

        var client = _httpClientFactory.CreateApiClient();
        var channel = await client.GetFromJsonAsync<Channel>(
            $"channels/{ChannelHandle}",
            _cancellation.Token
        );
        ChannelInfo = channel!;
    }

    private sealed record Channel
    {
        public string? Banner { get; init; }
        public required string Handle { get; init; }
        public required string Name { get; init; }
        public required string Thumbnail { get; init; }
    }
}
