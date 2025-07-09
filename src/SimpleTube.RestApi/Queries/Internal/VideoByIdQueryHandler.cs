using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Data.Sqlite;
using SimpleTube.RestApi.Exceptions;
using SimpleTube.RestApi.Infrastructure.Database;
using SimpleTube.RestApi.Infrastructure.Downloads;
using SimpleTube.RestApi.Infrastructure.Mediator;

namespace SimpleTube.RestApi.Queries.Internal;

internal sealed class VideoByIdQueryHandler : IQueryHandler<VideoByIdQuery, VideoByIdQuery.Result>
{
    private readonly ConnectionStringProvider _connectionStringProvider;
    private readonly IDownloadsManager _downloadsManager;

    private const string Sql = """
        SELECT [Channels].[Handle],
               [Channels].[Id],
               [Channels].[Name],
               [Videos].[Description],
               [Videos].[Id],
               [Videos].[Title],
               [Videos].[Thumbnail]
        FROM [Videos]

        LEFT JOIN [Channels]
            ON [Channels].[Id] = [Videos].[ChannelId]

        WHERE [Videos].[Id] = @videoId
        """;

    public VideoByIdQueryHandler(
        ConnectionStringProvider connectionStringProvider,
        IDownloadsManager downloadsManager
    )
    {
        _connectionStringProvider = connectionStringProvider;
        _downloadsManager = downloadsManager;
    }

    public async ValueTask<VideoByIdQuery.Result> Execute(
        VideoByIdQuery query,
        CancellationToken cancellationToken
    )
    {
        await using var connection = new SqliteConnection(
            _connectionStringProvider.ConnectionString
        );
        await connection.OpenAsync(cancellationToken);
        var command = connection.CreateCommand();
        command.CommandText = Sql;
        command.Parameters.AddWithValue("videoId", query.VideoId);
        var reader = await command.ExecuteReaderAsync(cancellationToken);

        if (!await reader.ReadAsync(cancellationToken))
            throw new NotFoundException($"Video {query.VideoId} does not exist.");

        var dto = new TableRowDto
        {
            ChannelHandle = await reader.IsDBNullAsync(0, cancellationToken)
                ? null
                : reader.GetString(0),
            ChannelId = await reader.IsDBNullAsync(1, cancellationToken)
                ? null
                : reader.GetString(1),
            ChannelName = await reader.IsDBNullAsync(2, cancellationToken)
                ? null
                : reader.GetString(2),
            Description = reader.GetString(3),
            Id = reader.GetString(4),
            Title = reader.GetString(5),
            Thumbnail = reader.GetString(6),
        };

        return new VideoByIdQuery.Result
        {
            ChannelHandle = dto.ChannelHandle,
            ChannelId = dto.ChannelId,
            ChannelName = dto.ChannelName,
            Description = dto.Description,
            Embed = $"https://www.youtube.com/embed/{dto.Id}?autoplay=1",
            Id = dto.Id,
            Streams = GetStreams(dto.ChannelId, dto.Id).ToArray(),
            Thumbnail = dto.Thumbnail,
            Title = dto.Title,
        };
    }

    private IEnumerable<VideoByIdQuery.Result.VideoStream> GetStreams(
        string? channelId,
        string videoId
    )
    {
        if (string.IsNullOrWhiteSpace(channelId))
            yield break;

        var downloadedFiles = _downloadsManager.GetDownloads(channelId, videoId);

        foreach (var file in downloadedFiles)
        {
            if (
                new FileExtensionContentTypeProvider().TryGetContentType(
                    file.Name,
                    out var contentType
                )
            )
                yield return new VideoByIdQuery.Result.VideoStream
                {
                    Id = file.Extension[1..],
                    Type = contentType,
                };
        }
    }

    private sealed record TableRowDto
    {
        public required string? ChannelHandle { get; init; }
        public required string? ChannelId { get; init; }
        public required string? ChannelName { get; init; }
        public required string Description { get; init; }
        public required string Id { get; init; }
        public required string Title { get; init; }
        public required string Thumbnail { get; init; }
    }
}
