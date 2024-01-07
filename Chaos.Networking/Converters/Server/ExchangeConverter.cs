using Chaos.Common.Definitions;
using Chaos.IO.Memory;
using Chaos.Networking.Definitions;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Converters.Server;

/// <summary>
///     Serializes a <see cref="ExchangeArgs" /> into a buffer
/// </summary>
public sealed class ExchangeConverter : PacketConverterBase<ExchangeArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ServerOpCode.Exchange;

    /// <inheritdoc />
    public override ExchangeArgs Deserialize(ref SpanReader reader)
    {
        var exchangeResponseType = reader.ReadByte();

        var args = new ExchangeArgs
        {
            ExchangeResponseType = (ExchangeResponseType)exchangeResponseType
        };

        switch (args.ExchangeResponseType)
        {
            case ExchangeResponseType.StartExchange:
            {
                var otherUserId = reader.ReadUInt32();
                var otherUserName = reader.ReadString8();

                args.OtherUserId = otherUserId;
                args.OtherUserName = otherUserName;

                break;
            }
            case ExchangeResponseType.RequestAmount:
            {
                var fromSlot = reader.ReadByte();

                args.FromSlot = fromSlot;

                break;
            }
            case ExchangeResponseType.AddItem:
            {
                var rightSide = reader.ReadBoolean();
                var exchangeIndex = reader.ReadByte();
                var itemSprite = reader.ReadUInt16();
                var itemColor = reader.ReadByte();
                var itemName = reader.ReadString8();

                args.RightSide = rightSide;
                args.ExchangeIndex = exchangeIndex;
                args.ItemSprite = (ushort)(itemSprite - NETWORKING_CONSTANTS.ITEM_SPRITE_OFFSET);
                args.ItemColor = (DisplayColor)itemColor;
                args.ItemName = itemName;

                break;
            }
            case ExchangeResponseType.SetGold:
            {
                var rightSide = reader.ReadBoolean();
                var goldAmount = reader.ReadInt32();

                args.RightSide = rightSide;
                args.GoldAmount = goldAmount;

                break;
            }
            case ExchangeResponseType.Cancel:
            {
                var rightSide = reader.ReadBoolean();
                var message = reader.ReadString8();

                args.RightSide = rightSide;
                args.Message = message;

                break;
            }
            case ExchangeResponseType.Accept:
            {
                var persistExchange = reader.ReadBoolean();
                var message = reader.ReadString8();

                args.PersistExchange = persistExchange;
                args.Message = message;

                break;
            }
            default:
                throw new ArgumentOutOfRangeException(
                    nameof(args.ExchangeResponseType),
                    args.ExchangeResponseType,
                    "Unknown exchange response type");
        }

        return args;
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, ExchangeArgs args)
    {
        writer.WriteByte((byte)args.ExchangeResponseType);

        switch (args.ExchangeResponseType)
        {
            case ExchangeResponseType.StartExchange:
                writer.WriteUInt32(args.OtherUserId!.Value);
                writer.WriteString8(args.OtherUserName);

                break;
            case ExchangeResponseType.RequestAmount:
                writer.WriteByte(args.FromSlot!.Value);

                break;
            case ExchangeResponseType.AddItem:
                writer.WriteBoolean(args.RightSide!.Value);
                writer.WriteByte(args.ExchangeIndex!.Value);
                writer.WriteUInt16((ushort)(args.ItemSprite!.Value + NETWORKING_CONSTANTS.ITEM_SPRITE_OFFSET));
                writer.WriteByte((byte)args.ItemColor!.Value);
                writer.WriteString8(args.ItemName!);

                break;
            case ExchangeResponseType.SetGold:
                writer.WriteBoolean(args.RightSide!.Value);
                writer.WriteInt32(args.GoldAmount!.Value);

                break;
            case ExchangeResponseType.Cancel:
                writer.WriteBoolean(args.RightSide!.Value);
                writer.WriteString8(args.Message!);

                break;
            case ExchangeResponseType.Accept:
                writer.WriteBoolean(args.PersistExchange!.Value);
                writer.WriteString8(args.Message!);

                break;
            default:
                throw new ArgumentOutOfRangeException(
                    nameof(args.ExchangeResponseType),
                    args.ExchangeResponseType,
                    "Unknown exchange response type");
        }
    }
}