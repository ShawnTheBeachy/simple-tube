using System.Text.Json;
using System.Text.Json.Serialization;
using SimpleTube.RestApi.Infrastructure.YouTube.Models;

namespace SimpleTube.RestApi.Infrastructure.YouTube;

internal static class HttpClientExtensions
{
    public static async ValueTask<ListResponse<Channel>> ListChannels(
        this HttpClient httpClient,
        string forHandle,
        CancellationToken cancellationToken
    )
    {
        var response = await httpClient.GetAsync(
            $"channels?forHandle={forHandle}&part=snippet",
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken
        );
        response.EnsureSuccessStatusCode();
        var listResponse = await JsonSerializer.DeserializeAsync<ListResponse<Channel>>(
            await response.Content.ReadAsStreamAsync(cancellationToken),
            YouTubeJsonSerializerContext.Default.ListResponseChannel,
            cancellationToken
        );
        return listResponse!;
    }
}

[JsonSerializable(typeof(Channel))]
[JsonSerializable(typeof(ChannelSnippet))]
[JsonSerializable(typeof(ListResponse<Channel>))]
[JsonSerializable(typeof(Thumbnail))]
[JsonSerializable(typeof(Thumbnails))]
internal sealed partial class YouTubeJsonSerializerContext : JsonSerializerContext;
