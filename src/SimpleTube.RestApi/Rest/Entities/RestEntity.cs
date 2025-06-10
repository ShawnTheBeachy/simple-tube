using System.Text.Json.Serialization;

namespace SimpleTube.RestApi.Rest.Entities;

internal abstract record RestEntity
{
    [JsonPropertyName("$entity#createdAt")]
    public DateTimeOffset? CreatedAt { get; init; }

    [JsonPropertyName("$entity#modifiedAt")]
    public DateTimeOffset? LastModifiedAt { get; init; }

    [JsonPropertyName("$entity#url")]
    public required string Url { get; init; }
}
