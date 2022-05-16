using Chaos.Core.Memory;
using Chaos.Core.Utilities;
using Chaos.Networking.Model.Client;

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