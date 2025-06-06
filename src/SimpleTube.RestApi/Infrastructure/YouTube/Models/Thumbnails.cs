using System.Text.Json.Serialization;

namespace SimpleTube.RestApi.Infrastructure.YouTube.Models;

internal sealed record Thumbnails
{
    [JsonPropertyName("high")]
    public required Thumbnail High { get; init; }
}
