using Chaos.IO.Memory;
using Chaos.Networking.Model.Client;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Deserializers;

public record IgnoreDeserializer : ClientPacketDeserializer<IgnoreArgs>
{
    public override ClientOpCode ClientOpCode => ClientOpCode.Ignore;

    public override IgnoreArgs Deserialize(ref SpanReader reader)
    {
        var ignoreType = (IgnoreType)reader.ReadByte();
        var targetName = default(string);

        if (ignoreType != IgnoreType.Request)
            targetName = reader.ReadString8();

        return new IgnoreArgs(ignoreType, targetName);
    }
}