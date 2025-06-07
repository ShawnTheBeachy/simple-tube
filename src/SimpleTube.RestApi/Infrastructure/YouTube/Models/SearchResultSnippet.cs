using System.Text.Json.Serialization;

namespace SimpleTube.RestApi.Infrastructure.YouTube.Models;

internal sealed record SearchResultSnippet
{
    [JsonPropertyName("channelId")]
    public required string ChannelId { get; init; }

    [JsonPropertyName("description")]
    public required string Description { get; init; }

    [JsonPropertyName("publishedAt")]
    public required DateTime PublishedAt { get; init; }

    [JsonPropertyName("thumbnails")]
    public required Thumbnails Thumbnails { get; init; }

    [JsonPropertyName("title")]
    public required string Title { get; init; }
}
