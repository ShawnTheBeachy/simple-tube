using System.Text.Json.Serialization;

namespace SimpleTube.RestApi.Infrastructure.YouTube.Models;

internal sealed record BrandingImage
{
    [JsonPropertyName("bannerExternalUrl")]
    public required string? BannerUrl { get; init; }
}
