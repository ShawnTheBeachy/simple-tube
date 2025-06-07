using FluentValidation;
using SimpleTube.RestApi.Infrastructure.Mediator;

namespace SimpleTube.RestApi.Commands;

public sealed record SubscribeCommand : ICommand<SubscribeCommand.Result>
{
    public required string ChannelHandle { get; init; }

    public sealed record Result
    {
        public required string ChannelHandle { get; init; }
        public required string ChannelId { get; init; }
        public required string ChannelName { get; init; }
        public required string ChannelThumbnail { get; init; }
    }

    public sealed class Validator : AbstractValidator<SubscribeCommand>
    {
        public Validator()
        {
            RuleFor(x => x.ChannelHandle).NotEmpty();
        }
    }
}
