using System.Text.Json.Serialization;

namespace SimpleTube.RestApi.Infrastructure.YouTube.Models;

internal sealed record Channel
{
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    [JsonPropertyName("snippet")]
    public required ChannelSnippet Snippet { get; init; }
}
