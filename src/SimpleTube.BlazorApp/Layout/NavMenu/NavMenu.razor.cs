using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using SimpleTube.BlazorApp.Infrastructure.Api;
using SimpleTube.BlazorApp.Providers;

namespace SimpleTube.BlazorApp.Layout.NavMenu;

public sealed partial class NavMenu : IAsyncDisposable
{
    private readonly CancellationTokenSource _cancellation = new();
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly NavigationManager _navigation;
    private readonly ServerUrlProvider _serverUrlProvider;

    private IReadOnlyList<Bookmark> Bookmarks { get; set; } = [];

    public NavMenu(
        IHttpClientFactory httpClientFactory,
        NavigationManager navigation,
        ServerUrlProvider serverUrlProvider
    )
    {
        _httpClientFactory = httpClientFactory;
        _navigation = navigation;
        _serverUrlProvider = serverUrlProvider;
    }

    public async ValueTask DisposeAsync()
    {
        await _cancellation.CancelAsync();
        _cancellation.Dispose();
    }

    protected override async Task OnInitializedAsync()
    {
        var bookmarks = await _httpClientFactory
            .CreateApiClient()
            .GetFromJsonAsync<IReadOnlyList<Bookmark>>("/");
        Bookmarks = bookmarks!;
        _navigation.LocationChanged += (_, _) =>
        {
            StateHasChanged();
        };
    }

    private sealed record Bookmark
    {
        public string? IconUrl { get; init; }
        public required string Name { get; init; }
        public required string Url { get; init; }
    }
}
