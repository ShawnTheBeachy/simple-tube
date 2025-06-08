using System.Net.Http.Json;
using System.Text.Json.Serialization;
using BlazorComponentBus;
using SimpleTube.BlazorApp.Infrastructure.Api;
using SimpleTube.BlazorApp.Messages;

namespace SimpleTube.BlazorApp.Pages.ChannelsPage;

public sealed partial class ChannelsPage : IAsyncDisposable
{
    private readonly IComponentBus _bus;
    private readonly CancellationTokenSource _cancellation = new();
    private readonly IHttpClientFactory _httpClientFactory;

    private IReadOnlyList<Channel> Channels { get; set; } = [];

    public ChannelsPage(IComponentBus bus, IHttpClientFactory httpClientFactory)
    {
        _bus = bus;
        _httpClientFactory = httpClientFactory;
    }

    public async ValueTask DisposeAsync()
    {
        await _cancellation.CancelAsync();
        _cancellation.Dispose();
        _bus.UnSubscribeFrom<SubscribedToChannelMessage>(OnSubscribedToChannel);
    }

    protected override async Task OnInitializedAsync()
    {
        await Refresh(_cancellation.Token);
        _bus.SubscribeTo<SubscribedToChannelMessage>(OnSubscribedToChannel);
    }

    private async Task OnSubscribedToChannel(
        SubscribedToChannelMessage message,
        CancellationToken cancellationToken
    )
    {
        Channels = Channels
            .Append(
                new Channel
                {
                    Handle = message.Handle,
                    Id = message.Id,
                    Name = message.Name,
                    Thumbnail = message.Thumbnail,
                    UnwatchedVideos = 0,
                    Url = message.Url,
                }
            )
            .OrderBy(x => x.Name)
            .ToArray();
        await InvokeAsync(StateHasChanged);
        await Refresh(cancellationToken);
    }

    private async Task Refresh(CancellationToken cancellationToken)
    {
        var channels = await _httpClientFactory
            .CreateApiClient()
            .GetFromJsonAsync<IReadOnlyList<Channel>>("channels", cancellationToken);
        Channels = channels!;
    }

    private sealed record Channel
    {
        public required string Handle { get; init; }
        public required string Id { get; init; }
        public required string Name { get; init; }
        public required string Thumbnail { get; init; }
        public required int UnwatchedVideos { get; init; }

        [JsonPropertyName("$entity#url")]
        public required string Url { get; init; }
    }
}
