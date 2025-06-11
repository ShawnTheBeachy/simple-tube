using System.Net.Http.Json;
using System.Text.Json.Serialization;
using BlazorComponentBus;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using SimpleTube.BlazorApp.Extensions;
using SimpleTube.BlazorApp.Infrastructure.Api;
using SimpleTube.BlazorApp.Messages;

namespace SimpleTube.BlazorApp.Pages.ChannelsPage;

public sealed partial class ChannelsPage : IAsyncDisposable
{
    private readonly IComponentBus _bus;
    private CancellationTokenSource _cancellation = new();
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly NavigationManager _navigation;

    private IReadOnlyList<Channel> Channels { get; set; } = [];
    private View CurrentView { get; set; } = View.AllChannels;

    public ChannelsPage(
        IComponentBus bus,
        IHttpClientFactory httpClientFactory,
        NavigationManager navigation
    )
    {
        _bus = bus;
        _httpClientFactory = httpClientFactory;
        _navigation = navigation;
    }

    public async ValueTask DisposeAsync()
    {
        await _cancellation.CancelAsync();
        _cancellation.Dispose();
        _bus.UnSubscribeFrom<SubscribedToChannelMessage>(OnSubscribedToChannel);
        _navigation.LocationChanged -= OnLocationChanged;
    }

    protected override void OnInitialized()
    {
        UpdateCurrentView();
        _navigation.LocationChanged += OnLocationChanged;
        _bus.SubscribeTo<SubscribedToChannelMessage>(OnSubscribedToChannel);
    }

    private void OnLocationChanged(object? sender, LocationChangedEventArgs args)
    {
        UpdateCurrentView();
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
        await Refresh();
    }

    private async Task Refresh()
    {
        await _cancellation.CancelAsync();
        _cancellation.Dispose();
        _cancellation = new CancellationTokenSource();
        var channels = await _httpClientFactory
            .CreateApiClient()
            .GetFromJsonAsync<IReadOnlyList<Channel>>(
                CurrentView == View.Subscriptions ? "subscriptions" : "channels",
                _cancellation.Token
            );
        Channels = channels!;
        await InvokeAsync(StateHasChanged);
    }

    private void UpdateCurrentView()
    {
        CurrentView =
            _navigation.Uri.Split('/', StringSplitOptions.RemoveEmptyEntries)[^1] == "subscriptions"
                ? View.Subscriptions
                : View.AllChannels;
        Refresh().FireAndForget();
    }

    private sealed record Channel
    {
        public required string Handle { get; init; }
        public required string Id { get; init; }
        public required string Name { get; init; }
        public bool? Subscribed { get; init; }
        public required string Thumbnail { get; init; }
        public required int UnwatchedVideos { get; init; }

        [JsonPropertyName("$entity#url")]
        public required string Url { get; init; }
    }

    private enum View
    {
        AllChannels,
        Subscriptions,
    }
}
