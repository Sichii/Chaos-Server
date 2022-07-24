using Chaos.IO.Memory;
using Chaos.Networking.Model.Client;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Deserializers;

public record ExitRequestDeserializer : ClientPacketDeserializer<ExitRequestArgs>
{
    public override ClientOpCode ClientOpCode => ClientOpCode.ExitRequest;

    public override ExitRequestArgs Deserialize(ref SpanReader reader)
    {
        var isRequest = reader.ReadBoolean();

        return new ExitRequestArgs(isRequest);
    }
}