using FluentValidation;
using SimpleTube.Shared.Mediator;

namespace SimpleTube.Shared.Queries;

public sealed record SubscriptionByChannelHandleQuery
    : IQuery<SubscriptionByChannelHandleQuery.Result>
{
    public required string ChannelHandle { get; init; }

    public sealed record Result
    {
        public required string ChannelHandle { get; init; }
        public required string ChannelId { get; init; }
        public required string ChannelName { get; init; }
        public required string ChannelThumbnail { get; init; }
        public required DateTimeOffset CreatedAt { get; init; }
        public required DateTimeOffset LastModifiedAt { get; init; }
    }

    public sealed class Validator : AbstractValidator<SubscriptionByChannelHandleQuery>
    {
        public Validator()
        {
            RuleFor(x => x.ChannelHandle).NotEmpty();
        }
    }
}
