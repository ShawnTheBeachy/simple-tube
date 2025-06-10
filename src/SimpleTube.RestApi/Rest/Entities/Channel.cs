using System.Text.Json.Serialization;

namespace SimpleTube.RestApi.Rest.Entities;

internal sealed record Channel : RestEntity
{
    public string? Banner { get; init; }
    public string? Handle { get; init; }
    public string? Id { get; init; }
    public string? Name { get; init; }
    public bool? Subscribed { get; init; }
    public string? Thumbnail { get; init; }
    public int? UnwatchedVideos { get; init; }

    [JsonPropertyName("$entity#unsubscribe")]
    public string? UnsubscribeUrl { get; init; }
}
