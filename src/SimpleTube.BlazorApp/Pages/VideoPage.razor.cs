using Microsoft.AspNetCore.Components;

namespace SimpleTube.BlazorApp.Pages;

public sealed partial class VideoPage : IAsyncDisposable
{
    private readonly CancellationTokenSource _cancellation = new();

    [Parameter]
    public string VideoId { get; set; } = "";

    public async ValueTask DisposeAsync()
    {
        await _cancellation.CancelAsync();
        _cancellation.Dispose();
    }
}
