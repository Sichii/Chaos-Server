using Chaos.IO.Memory;
using Chaos.Networking.Model.Client;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Deserializers;

public record RaiseStatDeserializer : ClientPacketDeserializer<RaiseStatArgs>
{
    public override ClientOpCode ClientOpCode => ClientOpCode.RaiseStat;

    public override RaiseStatArgs Deserialize(ref SpanReader reader)
    {
        var stat = (Stat)reader.ReadByte();

        return new RaiseStatArgs(stat);
    }
}