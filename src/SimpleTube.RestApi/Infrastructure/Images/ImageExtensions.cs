using System.Diagnostics.CodeAnalysis;

namespace SimpleTube.RestApi.Infrastructure.Images;

internal static class ImageExtensions
{
    [return: NotNullIfNotNull(nameof(url))]
    public static string? ImageUrl(this string? url) =>
        string.IsNullOrWhiteSpace(url) ? url : $"images/{(url[0] == '/' ? url[1..] : url)}";
}
