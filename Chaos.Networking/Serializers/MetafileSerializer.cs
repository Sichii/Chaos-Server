using Chaos.Common.Definitions;
using Chaos.IO.Memory;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Serializers;

/// <summary>
///     Serializes a <see cref="MetafileArgs" /> into a buffer
/// </summary>
public sealed record MetafileSerializer : ServerPacketSerializer<MetafileArgs>
{
    /// <inheritdoc />
    public override ServerOpCode ServerOpCode => ServerOpCode.Metafile;

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, MetafileArgs args)
    {
        writer.WriteByte((byte)args.MetafileRequestType);

        switch (args.MetafileRequestType)
        {
            case MetafileRequestType.DataByName:
                writer.WriteString8(args.MetafileData!.Name);
                writer.WriteUInt32(args.MetafileData!.CheckSum);
                writer.WriteData16(args.MetafileData!.Data);

                break;
            case MetafileRequestType.AllCheckSums:
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