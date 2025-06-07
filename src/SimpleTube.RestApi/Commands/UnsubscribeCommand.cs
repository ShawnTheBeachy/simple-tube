using FluentValidation;
using SimpleTube.RestApi.Infrastructure.Mediator;

namespace SimpleTube.RestApi.Commands;

public sealed record UnsubscribeCommand : ICommand<UnsubscribeCommand.Result>
{
    public required string ChannelHandle { get; init; }

    public sealed record Result
    {
        public required string ChannelHandle { get; init; }
    }

    public sealed class Validator : AbstractValidator<UnsubscribeCommand>
    {
        public Validator()
        {
            RuleFor(x => x.ChannelHandle).NotEmpty();
        }
    }
}
