using System.Text.Json.Serialization;

namespace SimpleTube.RestApi.Infrastructure.YouTube.Models;

internal sealed record BrandingSettings
{
    [JsonPropertyName("image")]
    public required BrandingImage Image { get; init; }
}
