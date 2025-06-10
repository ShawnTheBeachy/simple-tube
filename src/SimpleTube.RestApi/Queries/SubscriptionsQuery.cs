using FluentValidation;
using SimpleTube.RestApi.Infrastructure.Mediator;

namespace SimpleTube.RestApi.Queries;

public sealed record SubscriptionsQuery : IQuery<SubscriptionsQuery.Result>
{
    public sealed record Result
    {
        public required IReadOnlyList<Channel> Channels { get; init; }

        public sealed record Channel
        {
            public required string ChannelHandle { get; init; }
            public required string ChannelId { get; init; }
            public required string ChannelName { get; init; }
            public required string ChannelThumbnail { get; init; }
            public required int UnwatchedVideos { get; init; }
        }
    }

    public sealed class Validator : AbstractValidator<SubscriptionsQuery>
    {
        public Validator() { }
    }
}
