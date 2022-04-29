using Chaos.Core.Definitions;
using Chaos.Core.Utilities;
using Chaos.Networking.Model.Server;
using Chaos.Packets.Definitions;

namespace Chaos.Networking.Serializers;

public record ExchangeSerializer : ServerPacketSerializer<ExchangeArgs>
{
    public override ServerOpCode ServerOpCode => ServerOpCode.Exchange;

    public override void Serialize(ref SpanWriter writer, ExchangeArgs args)
    {
        writer.WriteByte((byte)args.ExchangeResponseType);

        switch (args.ExchangeResponseType)
        {
            case ExchangeResponseType.StartExchange:
                writer.WriteUInt32(args.FromId!.Value);
                writer.WriteString8(args.FromName);

                break;
            case ExchangeResponseType.RequestAmount:
                writer.WriteByte(args.FromSlot!.Value);

                break;
            case ExchangeResponseType.AddItem:
                writer.WriteBoolean(args.IsFromSelf!.Value);
                writer.WriteByte(args.ExchangeIndex!.Value);
                writer.WriteUInt16(args.ItemSprite!.Value);
                writer.WriteByte((byte)args.ItemColor!.Value);
                writer.WriteString8(args.ItemName!);

                break;
            case ExchangeResponseType.SetGold:
                writer.WriteBoolean(args.IsFromSelf!.Value);
                writer.WriteUInt32(args.GoldAmount!.Value);

                break;
            case ExchangeResponseType.Cancel:
                writer.WriteBoolean(!args.IsFromSelf!.Value);
                writer.WriteString8("Exchange cancelled.");

                break;
            case ExchangeResponseType.Accept:
                writer.WriteBoolean(!args.IsFromSelf!.Value);
                writer.WriteString8("Exchange completed.");

                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(args.ExchangeResponseType),
                    args.ExchangeResponseType,
                    "Unknown exchange response type");
        }
    }
}