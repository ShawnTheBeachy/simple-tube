using System.Text.Json.Serialization;

namespace SimpleTube.RestApi.Infrastructure.YouTube.Models;

internal sealed record VideoContentDetails
{
    [JsonPropertyName("duration")]
    public required string Duration { get; init; }
}
