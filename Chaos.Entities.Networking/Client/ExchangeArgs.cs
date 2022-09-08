using Chaos.Common.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Entities.Networking.Client;

public record ExchangeArgs(
    ExchangeRequestType ExchangeRequestType,
    uint OtherPlayerId,
    byte? SourceSlot,
    byte? ItemCount,
    int? GoldAmount
) : IReceiveArgs;