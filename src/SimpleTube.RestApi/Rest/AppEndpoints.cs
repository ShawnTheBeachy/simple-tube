using Microsoft.Data.Sqlite;
using SimpleTube.RestApi.Infrastructure.Database;
using SimpleTube.RestApi.Infrastructure.Images;
using SimpleTube.RestApi.Rest.Channels;
using SimpleTube.RestApi.Rest.Subscriptions;
using SimpleTube.RestApi.Rest.Videos;

namespace SimpleTube.RestApi.Rest;

internal static class AppEndpoints
{
    private const string FavoriteChannelsSql = """
        SELECT [Channels].[Handle],
               [Channels].[Name],
               [Channels].[Thumbnail],
               COUNT([Videos].[Id]) AS [UnwatchedVideos]
        FROM [Channels]
        LEFT JOIN [Videos]
            ON [Videos].[ChannelId] = [Channels].[Id]
            AND [Videos].[Watched] = 0
        WHERE [Favorite] = 1
        GROUP BY [Channels].[Handle],
                 [Channels].[Name],
                 [Channels].[Thumbnail]
        ORDER BY UnwatchedVideos DESC,
            [Name] COLLATE NOCASE
        LIMIT 5
        """;

    public static IEndpointRouteBuilder MapAppEndpoints(this IEndpointRouteBuilder builder)
    {
        builder
            .MapGet(
                "/",
                async (
                    ConnectionStringProvider connectionStringProvider,
                    CancellationToken cancellationToken
                ) =>
                {
                    var bookmarks = new List<Bookmark>
                    {
                        new() { Name = "Channels", Url = "/channels" },
                    };

                    await using var connection = new SqliteConnection(
                        connectionStringProvider.ConnectionString
                    );
                    await connection.OpenAsync(cancellationToken);
                    var command = connection.CreateCommand();
                    command.CommandText = FavoriteChannelsSql;
                    var reader = await command.ExecuteReaderAsync(cancellationToken);

                    while (await reader.ReadAsync(cancellationToken))
                    {
                        var bookmark = new Bookmark
                        {
                            IconUrl = reader.GetString(2).ImageUrl(),
                            Name = reader.GetString(1),
                            Url = $"/channels/{reader.GetString(0)}",
                        };
                        bookmarks.Add(bookmark);
                    }

                    return bookmarks.ToArray();
                }
            )
            .CacheOutput()
            .WithName("Get bookmarks")
            .WithTags();
        builder.MapChannelEndpoints().MapSubscriptionEndpoints().MapVideoEndpoints();
        return builder;
    }

    public sealed record Bookmark
    {
        public string? IconUrl { get; init; }
        public required string Name { get; init; }
        public required string Url { get; init; }
    }
}
