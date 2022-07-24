using Chaos.IO.Memory;
using Chaos.Networking.Model.Client;
using Chaos.Packets.Abstractions;

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