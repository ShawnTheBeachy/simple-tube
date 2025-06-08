using System.Diagnostics.CodeAnalysis;

namespace SimpleTube.RestApi.Extensions;

internal static class ServerImageExtensions
{
    [return: NotNullIfNotNull(nameof(url))]
    public static string? ServerImageUrl(
        this string? url,
        IHttpContextAccessor httpContextAccessor
    ) =>
        string.IsNullOrWhiteSpace(url) || url.StartsWith("http")
            ? url
            : $"{httpContextAccessor.HttpContext!.Request.Scheme}://{httpContextAccessor.HttpContext.Request.Host}/{url}";
}
