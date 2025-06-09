using FluentValidation;
using SimpleTube.RestApi.Infrastructure.Mediator;

namespace SimpleTube.RestApi.Commands;

public sealed record IgnoreVideoCommand : ICommand<IgnoreVideoCommand.Result>
{
    public required string VideoId { get; init; }

    public sealed record Result;

    internal sealed class Validator : AbstractValidator<IgnoreVideoCommand>
    {
        public Validator()
        {
            RuleFor(x => x.VideoId).NotEmpty();
        }
    }
}
