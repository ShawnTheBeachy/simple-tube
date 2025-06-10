using SimpleTube.RestApi.Infrastructure.Mediator;
using SimpleTube.RestApi.Infrastructure.YouTube;

namespace SimpleTube.RestApi.Commands.Internal.DownloadVideo;

internal sealed class DownloadVideoCommandHandler
    : ICommandHandler<DownloadVideoCommand, DownloadVideoCommand.Result>
{
    public ValueTask<DownloadVideoCommand.Result> Execute(
        DownloadVideoCommand command,
        CancellationToken cancellationToken
    )
    {
        var tempName = Path.GetTempFileName();
        File.Delete(tempName);
        var extensionIndex = tempName.LastIndexOf('.');

        if (extensionIndex > -1)
            tempName = tempName[..extensionIndex];

        Directory.CreateDirectory(tempName);
        YtDlp.Invoke(
            "--windows-filenames",
            "--write-info-json",
            "--dump-json",
            "--no-simulate",
            //"--progress-template",
            "-P",
            $"\"{tempName}\"",
            "-P",
            "temp:temp",
            "--output",
            "%(id)s.%(ext)s",
            $"https://www.youtube.com/watch?v={command.VideoId}"
        );
        return ValueTask.FromResult(new DownloadVideoCommand.Result());
    }
}
