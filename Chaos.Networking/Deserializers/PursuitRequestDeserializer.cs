using Chaos.Common.Definitions;
using Chaos.IO.Memory;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Deserializers;

public record PursuitRequestDeserializer : ClientPacketDeserializer<PursuitRequestArgs>
{
    public override ClientOpCode ClientOpCode => ClientOpCode.PursuitRequest;

    public override PursuitRequestArgs Deserialize(ref SpanReader reader)
    {
        var gameObjectType = (GameObjectType)reader.ReadByte();
        var objectId = reader.ReadUInt32();
        var pursuitId = reader.ReadUInt16();
        var args = reader.ReadArgs().ToArray();

        if (args.Length == 0)
            args = null;

        return new PursuitRequestArgs(
            gameObjectType,
            objectId,
            pursuitId,
            args);
    }
}