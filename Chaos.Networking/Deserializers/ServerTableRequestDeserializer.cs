using Chaos.Core.Definitions;
using Chaos.Core.Utilities;
using Chaos.Networking.Model.Client;
using Chaos.Packets.Definitions;

namespace Chaos.Networking.Deserializers;

public record ServerTableRequestDeserializer : ClientPacketDeserializer<ServerTableRequestArgs>
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