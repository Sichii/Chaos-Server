using Chaos.Common.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

public sealed record ExchangeArgs(
    ExchangeRequestType ExchangeRequestType,
    uint OtherPlayerId,
    byte? SourceSlot,
    byte? ItemCount,
    int? GoldAmount
) : IReceiveArgs;