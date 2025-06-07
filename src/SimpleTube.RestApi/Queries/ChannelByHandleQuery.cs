using FluentValidation;
using SimpleTube.RestApi.Infrastructure.Mediator;

namespace SimpleTube.RestApi.Queries;

public sealed record ChannelByHandleQuery : IQuery<ChannelByHandleQuery.Result>
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

    public sealed class Validator : AbstractValidator<ChannelByHandleQuery>
    {
        public Validator()
        {
            RuleFor(x => x.ChannelHandle).NotEmpty();
        }
    }
}
