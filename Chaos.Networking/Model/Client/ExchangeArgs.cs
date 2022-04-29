using Chaos.Core.Definitions;
using Chaos.Packets.Interfaces;

namespace Chaos.Networking.Model.Client;

public record ExchangeArgs(
    ExchangeRequestType ExchangeRequestType,
    uint OtherPlayerId,
    byte? SourceSlot,
    byte? ItemCount,
    uint? GoldAmount) : IReceiveArgs;