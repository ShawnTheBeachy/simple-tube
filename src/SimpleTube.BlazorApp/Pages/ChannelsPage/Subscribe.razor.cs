using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using BlazorComponentBus;
using SimpleTube.BlazorApp.Infrastructure.Api;

namespace SimpleTube.BlazorApp.Pages.ChannelsPage;

public sealed partial class Subscribe : IAsyncDisposable
{
    private readonly IComponentBus _bus;
    private readonly CancellationTokenSource _cancellation = new();
    private readonly IHttpClientFactory _httpClientFactory;
    private string? _value;

    public Subscribe(IComponentBus bus, IHttpClientFactory httpClientFactory)
    {
        _bus = bus;
        _httpClientFactory = httpClientFactory;
    }

    public async ValueTask DisposeAsync()
    {
        await _cancellation.CancelAsync();
        _cancellation.Dispose();
    }

    private async Task SubscribeToChannel()
    {
        var content = new StringContent(
            JsonSerializer.Serialize(new Request { ChannelHandle = _value! }),
            Encoding.UTF8,
            MediaTypeNames.Application.Json
        );
        var response = await _httpClientFactory
            .CreateApiClient()
            .PutAsync("channels", content, _cancellation.Token);
        response.EnsureSuccessStatusCode();
        _value = null;
        await InvokeAsync(StateHasChanged);
        var deserialized = await JsonSerializer.DeserializeAsync<Response>(
            await response.Content.ReadAsStreamAsync(_cancellation.Token),
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower }
        );
        await _bus.Publish(
            new Messages.SubscribedToChannelMessage
            {
                Handle = deserialized!.Handle,
                Id = deserialized.Id,
                Name = deserialized.Name,
                Thumbnail = deserialized.Thumbnail,
                Url = deserialized.Url,
            }
        );
    }

    private sealed record Request
    {
        public required string ChannelHandle { get; init; }
    }

    private sealed record Response
    {
        public required string Handle { get; init; }
        public required string Id { get; init; }
        public required string Name { get; init; }
        public required string Thumbnail { get; init; }

        [JsonPropertyName("$entity#url")]
        public required string Url { get; init; }
    }
}
