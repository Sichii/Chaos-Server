using Chaos.Entities.Networking.Client;
using Chaos.IO.Memory;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Deserializers;

public record SynchronizeTicksDeserializer : ClientPacketDeserializer<SynchronizeTicksArgs>
{
    public override ClientOpCode ClientOpCode => ClientOpCode.SynchronizeTicks;

    public override SynchronizeTicksArgs Deserialize(ref SpanReader reader)
    {
        var serverTicks = reader.ReadUInt32();
        var clientTicks = reader.ReadUInt32();

        return new SynchronizeTicksArgs(serverTicks, clientTicks);
    }
}