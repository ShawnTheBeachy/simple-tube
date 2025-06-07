using System.Text.Json.Serialization;

namespace SimpleTube.RestApi.Infrastructure.YouTube.Models;

internal sealed record SearchResultId
{
    [JsonPropertyName("videoId")]
    public string? VideoId { get; init; }
}
