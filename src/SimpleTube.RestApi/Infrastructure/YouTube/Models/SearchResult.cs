using System.Text.Json.Serialization;

namespace SimpleTube.RestApi.Infrastructure.YouTube.Models;

internal sealed record SearchResult
{
    [JsonPropertyName("id")]
    public required SearchResultId Id { get; init; }

    [JsonPropertyName("snippet")]
    public required SearchResultSnippet Snippet { get; init; }
}
