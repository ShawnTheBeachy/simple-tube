using System.Text.Json.Serialization;
using SimpleTube.RestApi.Commands;
using SimpleTube.RestApi.Queries;

namespace SimpleTube.RestApi.Infrastructure.Mediator.Internal;

[JsonSerializable(typeof(ChannelByHandleQuery))]
[JsonSerializable(
    typeof(ChannelByHandleQuery.Result),
    TypeInfoPropertyName = $"{nameof(Queries.ChannelByHandleQuery)}{nameof(Queries.ChannelByHandleQuery.Result)}"
)]
[JsonSerializable(typeof(ChannelsQuery))]
[JsonSerializable(
    typeof(ChannelsQuery.Result),
    TypeInfoPropertyName = $"{nameof(Queries.ChannelsQuery)}{nameof(Queries.ChannelsQuery.Result)}"
)]
[JsonSerializable(typeof(ChannelVideosQuery))]
[JsonSerializable(
    typeof(ChannelVideosQuery.Result),
    TypeInfoPropertyName = $"{nameof(Queries.ChannelVideosQuery)}{nameof(Queries.ChannelVideosQuery.Result)}"
)]
[JsonSerializable(typeof(DownloadChannelImagesCommand))]
[JsonSerializable(
    typeof(DownloadChannelImagesCommand.Result),
    TypeInfoPropertyName = $"{nameof(Commands.DownloadChannelImagesCommand)}{nameof(Commands.DownloadChannelImagesCommand.Result)}"
)]
[JsonSerializable(typeof(DownloadVideoCommand))]
[JsonSerializable(
    typeof(DownloadVideoCommand.Result),
    TypeInfoPropertyName = $"{nameof(Commands.DownloadVideoCommand)}{nameof(Commands.DownloadVideoCommand.Result)}"
)]
[JsonSerializable(typeof(IgnoreVideoCommand))]
[JsonSerializable(
    typeof(IgnoreVideoCommand.Result),
    TypeInfoPropertyName = $"{nameof(Commands.IgnoreVideoCommand)}{nameof(Commands.IgnoreVideoCommand.Result)}"
)]
[JsonSerializable(typeof(ScanChannelCommand))]
[JsonSerializable(
    typeof(ScanChannelCommand.Result),
    TypeInfoPropertyName = $"{nameof(Commands.ScanChannelCommand)}{nameof(Commands.ScanChannelCommand.Result)}"
)]
[JsonSerializable(typeof(SubscribeCommand))]
[JsonSerializable(
    typeof(SubscribeCommand.Result),
    TypeInfoPropertyName = $"{nameof(Commands.SubscribeCommand)}{nameof(Commands.SubscribeCommand.Result)}"
)]
[JsonSerializable(typeof(UnsubscribeCommand))]
[JsonSerializable(
    typeof(UnsubscribeCommand.Result),
    TypeInfoPropertyName = $"{nameof(Commands.UnsubscribeCommand)}{nameof(Commands.UnsubscribeCommand.Result)}"
)]
internal sealed partial class MediatorJsonSerializerContext : JsonSerializerContext;
