using Chaos.Core.Definitions;
using Chaos.Core.Utilities;
using Chaos.Networking.Model.Client;
using Chaos.Packets.Definitions;

namespace Chaos.Networking.Deserializers;

public record ExchangeDeserializer : ClientPacketDeserializer<ExchangeArgs>
{
    public override ClientOpCode ClientOpCode => ClientOpCode.Exchange;

    public override ExchangeArgs Deserialize(ref SpanReader reader)
    {
        var exchangeType = (ExchangeRequestType)reader.ReadByte();
        var otherPlayerId = reader.ReadUInt32();
        var sourceSlot = default(byte?);
        var goldAmount = default(int?);
        var itemCount = default(byte?);

        switch (exchangeType)
        {
            case ExchangeRequestType.StartExchange:
                break;
            case ExchangeRequestType.AddItem:
                sourceSlot = reader.ReadByte();
                itemCount = 1;

                break;
            case ExchangeRequestType.AddStackableItem:
                sourceSlot = reader.ReadByte();
                itemCount = reader.ReadByte();

                break;
            case ExchangeRequestType.SetGold:
                goldAmount = reader.ReadInt32();

                break;
            case ExchangeRequestType.Cancel:
                break;
            case ExchangeRequestType.Accept:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(exchangeType), exchangeType, "Unknown enum value");
        }

        return new ExchangeArgs(
            exchangeType,
            otherPlayerId,
            sourceSlot,
            itemCount,
            goldAmount);
    }
}