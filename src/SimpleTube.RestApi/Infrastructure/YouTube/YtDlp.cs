using System.Diagnostics;

namespace SimpleTube.RestApi.Infrastructure.YouTube;

internal static class YtDlp
{
    public static Process Invoke(string args)
    {
        var ytDlp =
            Environment.OSVersion.Platform == PlatformID.Unix
            || Environment.OSVersion.Platform == PlatformID.MacOSX
                ? "./lib/yt-dlp"
                : "./lib/yt-dlp.exe";
        var process = new ProcessStartInfo
        {
            Arguments = args,
            CreateNoWindow = true,
            FileName = ytDlp,
            RedirectStandardOutput = true,
            UseShellExecute = false,
        };
        return Process.Start(process)!;
    }
}
