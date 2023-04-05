using Chaos.Common.Definitions;
using Chaos.IO.Memory;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Deserializers;

/// <summary>
///     Deserializes a buffer into <see cref="PublicMessageArgs" />
/// </summary>
public sealed record PublicMessageDeserializer : ClientPacketDeserializer<PublicMessageArgs>
{
    /// <inheritdoc />
    public override ClientOpCode ClientOpCode => ClientOpCode.PublicMessage;

    /// <inheritdoc />
    public override PublicMessageArgs Deserialize(ref SpanReader reader)
    {
        var publicMessageType = (PublicMessageType)reader.ReadByte();
        var message = reader.ReadString8();

        return new PublicMessageArgs(publicMessageType, message);
    }
}