using System.Net.Http.Json;
using SimpleTube.BlazorApp.Infrastructure.Api;

namespace SimpleTube.BlazorApp.Layout.NavMenu;

public sealed partial class NavMenu : IAsyncDisposable
{
    private readonly CancellationTokenSource _cancellation = new();
    private readonly IHttpClientFactory _httpClientFactory;

    private IReadOnlyList<Bookmark> Bookmarks { get; set; } = [];

    public NavMenu(IHttpClientFactory httpClientFactory)
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
        var bookmarks = await _httpClientFactory
            .CreateApiClient()
            .GetFromJsonAsync<IReadOnlyList<Bookmark>>("/");
        Bookmarks = bookmarks!;
    }

    private sealed record Bookmark
    {
        public required string Name { get; init; }
        public required string Url { get; init; }
    }
}
