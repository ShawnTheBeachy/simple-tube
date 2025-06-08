using System.Text.Json.Serialization;

namespace SimpleTube.RestApi.Infrastructure.YouTube.Models;

internal sealed record BrandingSettings
{
    [JsonPropertyName("image")]
    public BrandingImage? Image { get; init; }
}
