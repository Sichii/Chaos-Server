using Chaos.IO.Memory;
using Chaos.Networking.Definitions;
using Chaos.Networking.Model.Server;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Serializers;

public record MetafileSerializer : ServerPacketSerializer<MetafileArgs>
{
    public override ServerOpCode ServerOpCode => ServerOpCode.Metafile;

    public override void Serialize(ref SpanWriter writer, MetafileArgs args)
    {
        switch (args.MetafileRequestType)
        {
            case MetafileRequestType.DataByName:
                writer.WriteByte((byte)args.MetafileRequestType);
                writer.WriteString8(args.MetafileData!.Name);
                writer.WriteUInt32(args.MetafileData!.CheckSum);
                writer.WriteData16(args.MetafileData!.Data);

                break;
            case MetafileRequestType.AllCheckSums:
                writer.WriteByte((byte)args.MetafileRequestType);
                writer.WriteUInt16((byte)args.Info!.Count);

                foreach (var info in args.Info!)
                {
                    writer.WriteString8(info.Name);
                    writer.WriteUInt32(info.CheckSum);
                }

                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(args.MetafileRequestType), args.MetafileRequestType, "Unknown enum value");
        }
    }
}