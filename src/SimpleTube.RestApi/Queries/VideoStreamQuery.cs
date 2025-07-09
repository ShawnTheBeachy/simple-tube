using FluentValidation;
using SimpleTube.RestApi.Infrastructure.Mediator;

namespace SimpleTube.RestApi.Queries;

public sealed record VideoStreamQuery : IQuery<Stream?>
{
    public required string Type { get; init; }
    public required string VideoId { get; init; }

    public sealed class Validator : AbstractValidator<VideoStreamQuery>
    {
        public Validator()
        {
            RuleFor(x => x.Type).NotEmpty();
            RuleFor(x => x.VideoId).NotEmpty();
        }
    }
}
