using Chaos.Entities.Networking.Server;
using Chaos.IO.Memory;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Serializers;

public record SynchronizeTicksResponseSerializer : ServerPacketSerializer<SynchronizeTicksResponseArgs>
{
    public override ServerOpCode ServerOpCode => ServerOpCode.SynchronizeTicks;

    public override void Serialize(ref SpanWriter writer, SynchronizeTicksResponseArgs responseArgs) =>
        writer.WriteInt32(responseArgs.Ticks);
}