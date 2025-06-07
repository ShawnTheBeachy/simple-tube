using SimpleTube.RestApi.Infrastructure.Mediator;

namespace SimpleTube.RestApi.Queries;

public sealed record ChannelVideosQuery : IQuery<ChannelVideosQuery.Result>
{
    public required string ChannelHandle { get; init; }

    public sealed record Result
    {
        public required IReadOnlyList<Video> Videos { get; init; }

        public sealed record Video
        {
            public required string Id { get; init; }
            public required DateTime PublishedAt { get; init; }
            public required string Thumbnail { get; init; }
            public required string Title { get; init; }
        }
    }
}
