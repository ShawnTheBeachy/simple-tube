using System.Text.Json.Serialization;
using SimpleTube.Shared.Commands;
using SimpleTube.Shared.Queries;

namespace SimpleTube.Shared.Mediator.Internal;

[JsonSerializable(typeof(SubscribeCommand))]
[JsonSerializable(
    typeof(SubscribeCommand.Result),
    TypeInfoPropertyName = $"{nameof(Commands.SubscribeCommand)}{nameof(Commands.SubscribeCommand.Result)}"
)]
[JsonSerializable(typeof(SubscriptionByChannelHandleQuery))]
[JsonSerializable(
    typeof(SubscriptionByChannelHandleQuery.Result),
    TypeInfoPropertyName = $"{nameof(Queries.SubscriptionByChannelHandleQuery)}{nameof(Queries.SubscriptionByChannelHandleQuery.Result)}"
)]
[JsonSerializable(typeof(SubscriptionsQuery))]
[JsonSerializable(
    typeof(SubscriptionsQuery.Result),
    TypeInfoPropertyName = $"{nameof(Queries.SubscriptionsQuery)}{nameof(Queries.SubscriptionsQuery.Result)}"
)]
internal sealed partial class MediatorJsonSerializerContext : JsonSerializerContext;
