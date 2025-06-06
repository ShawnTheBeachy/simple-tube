using System.Text.Json.Serialization;

namespace SimpleTube.RestApi.Infrastructure.YouTube.Models;

internal sealed record Thumbnail
{
    [JsonPropertyName("url")]
    public required string Url { get; init; }
}
