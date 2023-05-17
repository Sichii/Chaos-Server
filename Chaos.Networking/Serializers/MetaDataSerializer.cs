using Chaos.Common.Definitions;
using Chaos.IO.Memory;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Serializers;

/// <summary>
///     Serializes a <see cref="MetaDataArgs" /> into a buffer
/// </summary>
public sealed record MetaDataSerializer : ServerPacketSerializer<MetaDataArgs>
{
    /// <inheritdoc />
    public override ServerOpCode ServerOpCode => ServerOpCode.MetaData;

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, MetaDataArgs args)
    {
        writer.WriteByte((byte)args.MetaDataRequestType);

        switch (args.MetaDataRequestType)
        {
            case MetaDataRequestType.DataByName:
                writer.WriteString8(args.MetaDataInfo!.Name);
                writer.WriteUInt32(args.MetaDataInfo!.CheckSum);
                writer.WriteData16(args.MetaDataInfo!.Data);

                break;
            case MetaDataRequestType.AllCheckSums:
                writer.WriteUInt16((byte)args.MetaDataCollection!.Count);

                foreach (var info in args.MetaDataCollection!)
                {
                    writer.WriteString8(info.Name);
                    writer.WriteUInt32(info.CheckSum);
                }

                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(args.MetaDataRequestType), args.MetaDataRequestType, "Unknown enum value");
        }
    }
}