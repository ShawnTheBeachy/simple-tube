using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using SimpleTube.RestApi.Infrastructure.YouTube.Models;

namespace SimpleTube.RestApi.Infrastructure.YouTube;

internal static class HttpClientExtensions
{
    public static async ValueTask<Channel> GetChannelById(
        this HttpClient httpClient,
        string id,
        CancellationToken cancellationToken
    )
    {
        var response = await httpClient.Send(
            $"channels?id={id}&part=snippet,brandingSettings",
            YouTubeJsonSerializerContext.Default.ListResponseChannel,
            cancellationToken
        );
        return response.Items[0];
    }

    public static async ValueTask<ListResponse<Channel>> ListChannels(
        this HttpClient httpClient,
        string forHandle,
        CancellationToken cancellationToken
    ) =>
        await httpClient.Send(
            $"channels?forHandle={forHandle}&part=snippet,brandingSettings",
            YouTubeJsonSerializerContext.Default.ListResponseChannel,
            cancellationToken
        );

    public static async IAsyncEnumerable<SearchResult> ListChannelVideos(
        this HttpClient httpClient,
        string channelId,
        [EnumeratorCancellation] CancellationToken cancellationToken
    )
    {
        string? pageToken = null;

        do
        {
            var response = await httpClient.Send(
                $"search?channelId={channelId}&type=video&part=snippet&order=date&maxResults=50&pageToken={pageToken}",
                YouTubeJsonSerializerContext.Default.ListResponseSearchResult,
                cancellationToken
            );
            pageToken = response.NextPageToken;

            foreach (var result in response.Items)
                yield return result;
        } while (pageToken is not null);
    }

    private static async ValueTask<TR> Send<TR>(
        this HttpClient httpClient,
        string url,
        JsonTypeInfo<TR> jsonTypeInfo,
        CancellationToken cancellationToken
    )
    {
        var response = await httpClient.GetAsync(
            url,
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken
        );
        response.EnsureSuccessStatusCode();
        var tr = await JsonSerializer.DeserializeAsync(
            await response.Content.ReadAsStreamAsync(cancellationToken),
            jsonTypeInfo,
            cancellationToken
        );
        return tr!;
    }
}

[JsonSerializable(typeof(BrandingSettings))]
[JsonSerializable(typeof(Channel))]
[JsonSerializable(typeof(ChannelSnippet))]
[JsonSerializable(typeof(ListResponse<Channel>))]
[JsonSerializable(typeof(ListResponse<SearchResult>))]
[JsonSerializable(typeof(SearchResult))]
[JsonSerializable(typeof(SearchResultId))]
[JsonSerializable(typeof(SearchResultSnippet))]
[JsonSerializable(typeof(Thumbnail))]
[JsonSerializable(typeof(Thumbnails))]
internal sealed partial class YouTubeJsonSerializerContext : JsonSerializerContext;
