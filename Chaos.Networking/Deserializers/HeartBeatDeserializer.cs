using Chaos.IO.Memory;
using Chaos.Networking.Model.Client;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Deserializers;

public record HeartBeatDeserializer : ClientPacketDeserializer<HeartBeatArgs>
{
    public override ClientOpCode ClientOpCode => ClientOpCode.HeartBeat;

    public override HeartBeatArgs Deserialize(ref SpanReader reader)
    {
        var first = reader.ReadByte();
        var second = reader.ReadByte();

        return new HeartBeatArgs(first, second);
    }
}