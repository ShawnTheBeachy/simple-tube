using System.Text.Json.Serialization;

namespace SimpleTube.RestApi.Infrastructure.YouTube.Models;

internal sealed record Video
{
    [JsonPropertyName("contentDetails")]
    public VideoContentDetails? ContentDetails { get; init; }

    [JsonPropertyName("etag")]
    public required string ETag { get; init; }

    [JsonPropertyName("id")]
    public required string Id { get; init; }

    [JsonPropertyName("snippet")]
    public VideoSnippet? Snippet { get; init; }
}
