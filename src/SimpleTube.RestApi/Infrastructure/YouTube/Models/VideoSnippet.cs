using System.Text.Json.Serialization;

namespace SimpleTube.RestApi.Infrastructure.YouTube.Models;

internal sealed record VideoSnippet
{
    [JsonPropertyName("description")]
    public required string Description { get; init; }

    [JsonPropertyName("publishedAt")]
    public required DateTime PublishedAt { get; init; }

    [JsonPropertyName("tags")]
    public string[] Tags { get; init; } = [];

    [JsonPropertyName("thumbnails")]
    public required Thumbnails Thumbnails { get; init; }

    [JsonPropertyName("title")]
    public required string Title { get; init; }
}
