using Chaos.Common.Definitions;
using Chaos.IO.Memory;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Deserializers;

public sealed record ServerTableRequestDeserializer : ClientPacketDeserializer<ServerTableRequestArgs>
{
    public override ClientOpCode ClientOpCode => ClientOpCode.ServerTableRequest;

    public override ServerTableRequestArgs Deserialize(ref SpanReader reader)
    {
        var serverTableRequestType = (ServerTableRequestType)reader.ReadByte();
        var serverId = default(byte?);

        if (serverTableRequestType == ServerTableRequestType.ServerId)
            serverId = reader.ReadByte();

        return new ServerTableRequestArgs(serverTableRequestType, serverId);
    }
}