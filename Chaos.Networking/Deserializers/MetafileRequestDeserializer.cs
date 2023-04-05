using Chaos.Common.Definitions;
using Chaos.IO.Memory;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Deserializers;

/// <summary>
///     Deserializes a buffer into <see cref="MetafileRequestArgs" />
/// </summary>
public sealed record MetafileRequestDeserializer : ClientPacketDeserializer<MetafileRequestArgs>
{
    /// <inheritdoc />
    public override ClientOpCode ClientOpCode => ClientOpCode.MetafileRequest;

    /// <inheritdoc />
    public override MetafileRequestArgs Deserialize(ref SpanReader reader)
    {
        var metafileRequestType = (MetafileRequestType)reader.ReadByte();
        var name = default(string?);

        switch (metafileRequestType)
        {
            case MetafileRequestType.DataByName:
                name = reader.ReadString8();

                break;
            case MetafileRequestType.AllCheckSums:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return new MetafileRequestArgs(metafileRequestType, name);
    }
}