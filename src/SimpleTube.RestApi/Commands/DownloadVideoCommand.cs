using FluentValidation;
using SimpleTube.RestApi.Infrastructure.Mediator;

namespace SimpleTube.RestApi.Commands;

public sealed record DownloadVideoCommand : ICommand<DownloadVideoCommand.Result>
{
    public required string VideoId { get; init; }

    public sealed record Result;

    public sealed class Validator : AbstractValidator<DownloadVideoCommand>
    {
        public Validator()
        {
            RuleFor(x => x.VideoId).NotEmpty();
        }
    }
}
