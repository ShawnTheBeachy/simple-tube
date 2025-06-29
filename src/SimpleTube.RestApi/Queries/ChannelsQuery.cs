﻿using FluentValidation;
using SimpleTube.RestApi.Infrastructure.Mediator;

namespace SimpleTube.RestApi.Queries;

public sealed record ChannelsQuery : IQuery<ChannelsQuery.Result>
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
            public required bool Subscribed { get; init; }
            public required int UnwatchedVideos { get; init; }
        }
    }

    public sealed class Validator : AbstractValidator<ChannelsQuery>
    {
        public Validator() { }
    }
}
