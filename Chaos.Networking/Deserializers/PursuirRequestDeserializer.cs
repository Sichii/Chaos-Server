using Chaos.Core.Memory;
using Chaos.Core.Utilities;
using Chaos.Networking.Model.Client;

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