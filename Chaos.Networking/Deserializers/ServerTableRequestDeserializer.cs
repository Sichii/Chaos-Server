using Chaos.Common.Definitions;
using Chaos.IO.Memory;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Deserializers;

/// <summary>
///     Deserializes a buffer into <see cref="ServerTableRequestArgs" />
/// </summary>
public sealed record ServerTableRequestDeserializer : ClientPacketDeserializer<ServerTableRequestArgs>
{
    /// <inheritdoc />
    public override ClientOpCode ClientOpCode => ClientOpCode.ServerTableRequest;

    /// <inheritdoc />
    public override ServerTableRequestArgs Deserialize(ref SpanReader reader)
    {
        var serverTableRequestType = (ServerTableRequestType)reader.ReadByte();
        var serverId = default(byte?);

        if (serverTableRequestType == ServerTableRequestType.ServerId)
            serverId = reader.ReadByte();

        return new ServerTableRequestArgs(serverTableRequestType, serverId);
    }
}