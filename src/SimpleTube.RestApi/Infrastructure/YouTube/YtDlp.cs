using System.Diagnostics;

namespace SimpleTube.RestApi.Infrastructure.YouTube;

internal static class YtDlp
{
    public static void Invoke(params string[] args)
    {
        var ytDlp =
            Environment.OSVersion.Platform == PlatformID.Unix
            || Environment.OSVersion.Platform == PlatformID.MacOSX
                ? "./lib/yt-dlp"
                : "./lib/yt-dlp.exe";
        Process.Start(ytDlp, args);
    }
}
