using System.Text.Json.Serialization;
using SimpleTube.Shared.Commands;
using SimpleTube.Shared.Queries;

namespace SimpleTube.Shared.Mediator.Internal;

[JsonSerializable(typeof(SubscribeCommand))]
[JsonSerializable(
    typeof(SubscribeCommand.Result),
    TypeInfoPropertyName = $"{nameof(Commands.SubscribeCommand)}{nameof(Commands.SubscribeCommand.Result)}"
)]
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
[JsonSerializable(typeof(UnsubscribeCommand))]
[JsonSerializable(
    typeof(UnsubscribeCommand.Result),
    TypeInfoPropertyName = $"{nameof(Commands.UnsubscribeCommand)}{nameof(Commands.UnsubscribeCommand.Result)}"
)]
internal sealed partial class MediatorJsonSerializerContext : JsonSerializerContext;
