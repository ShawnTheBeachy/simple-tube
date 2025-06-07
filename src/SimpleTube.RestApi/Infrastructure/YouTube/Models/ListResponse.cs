using System.Text.Json.Serialization;

namespace SimpleTube.RestApi.Infrastructure.YouTube.Models;

internal sealed record ListResponse<T>
{
    [JsonPropertyName("items")]
    public IReadOnlyList<T> Items { get; init; } = [];

    [JsonPropertyName("nextPageToken")]
    public string? NextPageToken { get; init; }

    [JsonPropertyName("pageInfo")]
    public PageInfo? PageInfo { get; init; }
}
