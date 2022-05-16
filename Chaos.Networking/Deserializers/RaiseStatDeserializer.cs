using Chaos.Core.Memory;
using Chaos.Core.Utilities;
using Chaos.Networking.Model.Client;

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