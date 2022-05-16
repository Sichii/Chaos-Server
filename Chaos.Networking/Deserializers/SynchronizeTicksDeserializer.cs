using Chaos.Core.Memory;
using Chaos.Core.Utilities;
using Chaos.Networking.Model.Client;

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