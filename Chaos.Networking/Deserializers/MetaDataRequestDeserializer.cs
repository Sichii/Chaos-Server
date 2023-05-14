using Chaos.Common.Definitions;
using Chaos.IO.Memory;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Deserializers;

/// <summary>
///     Deserializes a buffer into <see cref="MetaDataRequestArgs" />
/// </summary>
public sealed record MetaDataRequestDeserializer : ClientPacketDeserializer<MetaDataRequestArgs>
{
    /// <inheritdoc />
    public override ClientOpCode ClientOpCode => ClientOpCode.MetaDataRequest;

    /// <inheritdoc />
    public override MetaDataRequestArgs Deserialize(ref SpanReader reader)
    {
        var metadataRequestType = (MetaDataRequestType)reader.ReadByte();
        var name = default(string?);

        switch (metadataRequestType)
        {
            case MetaDataRequestType.DataByName:
                name = reader.ReadString8();

                break;
            case MetaDataRequestType.AllCheckSums:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return new MetaDataRequestArgs(metadataRequestType, name);
    }
}