using Chaos.Common.Definitions;
using Chaos.Entities.Networking.Client;
using Chaos.IO.Memory;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

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