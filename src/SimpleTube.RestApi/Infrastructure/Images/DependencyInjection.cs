using Imageflow.Server;
using Imageflow.Server.HybridCache;
using Microsoft.Extensions.Options;
using SimpleTube.RestApi.Extensions;

namespace SimpleTube.RestApi.Infrastructure.Images;

internal static class DependencyInjection
{
    public static IServiceCollection AddImages(this IServiceCollection services)
    {
        var imageFolder = AppData.GetFolder("images");
        services
            .AddOptions<ImageOptions>()
            .Configure(opts =>
            {
                opts.Location = imageFolder;
            })
            .ValidateWithFluentValidation()
            .ValidateOnStart();
        services.AddTransient<IImageService, ImageService>();

        var homeFolder =
            Environment.OSVersion.Platform == PlatformID.Unix
            || Environment.OSVersion.Platform == PlatformID.MacOSX
                ? Environment.GetEnvironmentVariable("HOME")
                : Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%");

        services.AddImageflowHybridCache(
            new HybridCacheOptions(Path.Combine(homeFolder!, "imageflow_hybrid_cache"))
            {
                // How long after a file is created before it can be deleted
                MinAgeToDelete = TimeSpan.FromSeconds(10),
                // How much RAM to use for the write queue before switching to synchronous writes
                QueueSizeLimitInBytes = 100 * 1000 * 1000,
                // The maximum size of the cache (1GB)
                CacheSizeLimitInBytes = 1024 * 1024 * 1024,
            }
        );
        return services;
    }

    public static IApplicationBuilder UseImages(this IApplicationBuilder app)
    {
        var imageOptions = app.ApplicationServices.GetRequiredService<IOptions<ImageOptions>>();
        app.UseImageflow(
            new ImageflowMiddlewareOptions()
                .SetMapWebRoot(false)
                .MapPath("/images", imageOptions.Value.Location)
                .SetAllowCaching(true)
                .SetMyOpenSourceProjectUrl("https://github.com/ShawnTheBeachy/simple-tube")
                .SetDefaultCacheControlString(
                    $"public,max-age={TimeSpan.FromDays(365).TotalSeconds}"
                )
        );
        return app;
    }
}
