using SimpleTube.RestApi.Infrastructure.Mediator;

namespace SimpleTube.RestApi.Queries;

public sealed record VideoByIdQuery(string VideoId) : IQuery<VideoByIdQuery.Result>
{
    public sealed record Result
    {
        public required string? ChannelHandle { get; init; }
        public required string? ChannelId { get; init; }
        public required string? ChannelName { get; init; }
        public required string Description { get; init; }
        public required string Embed { get; init; }
        public required string Id { get; init; }
        public required IReadOnlyList<VideoStream> Streams { get; init; }
        public required string Thumbnail { get; init; }
        public required string Title { get; init; }

        public sealed record VideoStream
        {
            public required string Id { get; init; }
            public required string Type { get; init; }
        }
    }
}
