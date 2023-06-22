using Chaos.Common.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the <see cref="Chaos.Packets.Abstractions.Definitions.ClientOpCode.Exchange" />
///     packet
/// </summary>
/// <param name="ExchangeRequestType">The type of action being requested</param>
/// <param name="OtherPlayerId">The id of the player on the other side of the exchange</param>
/// <param name="SourceSlot">If specified, the slot of the item being added</param>
/// <param name="ItemCount">If specified, the count of the item being added</param>
/// <param name="GoldAmount">If specified, the amount of gold being set in the exchange (set, not added!)</param>
public sealed record ExchangeArgs(
    ExchangeRequestType ExchangeRequestType,
    uint OtherPlayerId,
    byte? SourceSlot,
    byte? ItemCount,
    int? GoldAmount
) : IReceiveArgs;