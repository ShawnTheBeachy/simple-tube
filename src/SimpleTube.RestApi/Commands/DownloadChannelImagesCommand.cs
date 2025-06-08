using FluentValidation;
using SimpleTube.RestApi.Infrastructure.Mediator;

namespace SimpleTube.RestApi.Commands;

public sealed record DownloadChannelImagesCommand : ICommand<DownloadChannelImagesCommand.Result>
{
    public required string ChannelId { get; init; }

    public sealed record Result;

    internal sealed class Validator : AbstractValidator<DownloadChannelImagesCommand>
    {
        public Validator()
        {
            RuleFor(x => x.ChannelId).NotEmpty();
        }
    }
}
