using FluentValidation;
using SimpleTube.Shared.Mediator;

namespace SimpleTube.Shared.Queries;

public sealed record SubscriptionsQuery : IQuery<SubscriptionsQuery.Result>
{
    public sealed record Result
    {
        public required IReadOnlyList<Subscription> Subscriptions { get; init; }

        public sealed record Subscription
        {
            public required string ChannelHandle { get; init; }
            public required string ChannelId { get; init; }
            public required string ChannelName { get; init; }
            public required string ChannelThumbnail { get; init; }
        }
    }

    public sealed class Validator : AbstractValidator<SubscriptionsQuery>
    {
        public Validator() { }
    }
}
