using Chaos.DarkAges.Definitions;
using Chaos.IO.Memory;
using Chaos.Networking.Abstractions.Definitions;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Converters.Client;

/// <summary>
///     Provides packet serialization and deserialization logic for <see cref="ExchangeInteractionArgs" />
/// </summary>
public sealed class ExchangeInteractionConverter : PacketConverterBase<ExchangeInteractionArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ClientOpCode.ExchangeInteraction;

    /// <inheritdoc />
    public override ExchangeInteractionArgs Deserialize(ref SpanReader reader)
    {
        var exchangeRequestType = reader.ReadByte();
        var otherPlayerId = reader.ReadUInt32();

        var args = new ExchangeInteractionArgs
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
    public override void Serialize(ref SpanWriter writer, ExchangeInteractionArgs args)
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