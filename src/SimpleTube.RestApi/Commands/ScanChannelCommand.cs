using FluentValidation;
using SimpleTube.RestApi.Infrastructure.Mediator;

namespace SimpleTube.RestApi.Commands;

public sealed record ScanChannelCommand : ICommand<ScanChannelCommand.Result>
{
    public required string ChannelId { get; init; }

    public sealed record Result;

    public sealed class Validator : AbstractValidator<ScanChannelCommand>
    {
        public Validator()
        {
            RuleFor(x => x.ChannelId).NotEmpty();
        }
    }
}
