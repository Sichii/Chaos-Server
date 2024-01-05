using Chaos.Common.Definitions;
using Chaos.IO.Memory;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Deserializers;

/// <summary>
///     Deserializes a buffer into <see cref="ExchangeArgs" />
/// </summary>
public sealed class ExchangeConverter : PacketConverterBase<ExchangeArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ClientOpCode.Exchange;

    /// <inheritdoc />
    public override ExchangeArgs Deserialize(ref SpanReader reader)
    {
        var exchangeRequestType = reader.ReadByte();
        var otherPlayerId = reader.ReadUInt32();

        var args = new ExchangeArgs
        {
            ExchangeRequestType = (ExchangeRequestType)exchangeRequestType,
            OtherPlayerId = otherPlayerId
        };

        switch (args.ExchangeRequestType)
        {
            case ExchangeRequestType.StartExchange:
                break;
            case ExchangeRequestType.AddItem:
            {
                var sourceSlot = reader.ReadByte();

                args.SourceSlot = sourceSlot;
                args.ItemCount = 1;

                break;
            }
            case ExchangeRequestType.AddStackableItem:
            {
                var sourceSlot = reader.ReadByte();
                var itemCount = reader.ReadByte();

                args.SourceSlot = sourceSlot;
                args.ItemCount = itemCount;

                break;
            }
            case ExchangeRequestType.SetGold:
            {
                var goldAmount = reader.ReadInt32();

                args.GoldAmount = goldAmount;

                break;
            }
            case ExchangeRequestType.Cancel:
                break;
            case ExchangeRequestType.Accept:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(exchangeRequestType), exchangeRequestType, "Unknown enum value");
        }

        return args;
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, ExchangeArgs args)
    {
        writer.WriteByte((byte)args.ExchangeRequestType);
        writer.WriteUInt32(args.OtherPlayerId);

        switch (args.ExchangeRequestType)
        {
            case ExchangeRequestType.StartExchange:
                break;
            case ExchangeRequestType.AddItem:
                writer.WriteByte(args.SourceSlot!.Value);

                break;
            case ExchangeRequestType.AddStackableItem:
                writer.WriteByte(args.SourceSlot!.Value);
                writer.WriteByte(args.ItemCount!.Value);

                break;
            case ExchangeRequestType.SetGold:
                writer.WriteInt32(args.GoldAmount!.Value);

                break;
            case ExchangeRequestType.Cancel:
                break;
            case ExchangeRequestType.Accept:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(args.ExchangeRequestType), args.ExchangeRequestType, "Unknown enum value");
        }
    }
}