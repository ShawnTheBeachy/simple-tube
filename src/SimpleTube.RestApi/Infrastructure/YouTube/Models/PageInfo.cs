using System.Text.Json.Serialization;

namespace SimpleTube.RestApi.Infrastructure.YouTube.Models;

internal sealed record PageInfo
{
    [JsonPropertyName("resultsPerPage")]
    public required int ResultsPerPage { get; init; }

    [JsonPropertyName("totalResults")]
    public required int TotalResults { get; init; }
}
