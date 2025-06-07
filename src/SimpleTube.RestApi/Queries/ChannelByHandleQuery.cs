using FluentValidation;
using SimpleTube.RestApi.Infrastructure.Mediator;

namespace SimpleTube.RestApi.Queries;

public sealed record ChannelByHandleQuery : IQuery<ChannelByHandleQuery.Result>
{
    public required string ChannelHandle { get; init; }

    public sealed record Result
    {
        public string? Banner { get; init; }
        public required DateTimeOffset CreatedAt { get; init; }
        public required string Handle { get; init; }
        public required string Id { get; init; }
        public required DateTimeOffset LastModifiedAt { get; init; }
        public required string Name { get; init; }
        public required string Thumbnail { get; init; }
    }

    public sealed class Validator : AbstractValidator<ChannelByHandleQuery>
    {
        public Validator()
        {
            RuleFor(x => x.ChannelHandle).NotEmpty();
        }
    }
}
