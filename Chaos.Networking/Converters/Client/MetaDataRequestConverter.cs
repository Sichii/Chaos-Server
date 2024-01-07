using Chaos.Common.Definitions;
using Chaos.IO.Memory;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Converters.Client;

/// <summary>
///     Deserializes a buffer into <see cref="MetaDataRequestArgs" />
/// </summary>
public sealed class MetaDataRequestConverter : PacketConverterBase<MetaDataRequestArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ClientOpCode.MetaDataRequest;

    /// <inheritdoc />
    public override MetaDataRequestArgs Deserialize(ref SpanReader reader)
    {
        var metadataRequestType = reader.ReadByte();

        var args = new MetaDataRequestArgs
        {
            MetaDataRequestType = (MetaDataRequestType)metadataRequestType
        };

        switch (args.MetaDataRequestType)
        {
            case MetaDataRequestType.DataByName:
            {
                var name = reader.ReadString8();

                args.Name = name;

                break;
            }
            case MetaDataRequestType.AllCheckSums:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return args;
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, MetaDataRequestArgs args)
    {
        writer.WriteByte((byte)args.MetaDataRequestType);

        switch (args.MetaDataRequestType)
        {
            case MetaDataRequestType.DataByName:
                writer.WriteString8(args.Name!);

                break;
            case MetaDataRequestType.AllCheckSums:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}