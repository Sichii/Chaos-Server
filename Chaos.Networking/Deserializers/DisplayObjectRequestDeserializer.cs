using Chaos.Entities.Networking.Client;
using Chaos.IO.Memory;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Deserializers;

public record DisplayObjectRequestDeserializer : ClientPacketDeserializer<DisplayObjectRequestArgs>
{
    public override ClientOpCode ClientOpCode => ClientOpCode.DisplayObjectRequest;

    public override DisplayObjectRequestArgs Deserialize(ref SpanReader reader)
    {
        var targetId = reader.ReadUInt32();

        return new DisplayObjectRequestArgs(targetId);
    }
}