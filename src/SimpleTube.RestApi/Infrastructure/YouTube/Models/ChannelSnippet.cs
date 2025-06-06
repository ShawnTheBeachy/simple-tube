using System.Text.Json.Serialization;

namespace SimpleTube.RestApi.Infrastructure.YouTube.Models;

internal sealed record ChannelSnippet
{
    [JsonPropertyName("thumbnails")]
    public required Thumbnails Thumbnails { get; init; }

    [JsonPropertyName("title")]
    public required string Title { get; init; }
}
