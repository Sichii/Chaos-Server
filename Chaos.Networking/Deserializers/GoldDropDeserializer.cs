using Chaos.Core.Memory;
using Chaos.Core.Utilities;
using Chaos.Networking.Model.Client;

namespace Chaos.Networking.Deserializers;

public record GoldDropDeserializer : ClientPacketDeserializer<GoldDropArgs>
{
    public override ClientOpCode ClientOpCode => ClientOpCode.GoldDrop;

    public override GoldDropArgs Deserialize(ref SpanReader reader)
    {
        var amount = reader.ReadInt32();
        var destinationPoint = reader.ReadPoint16();

        return new GoldDropArgs(amount, destinationPoint);
    }
}