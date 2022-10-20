using Chaos.IO.Memory;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Serializers;

public sealed record SynchronizeTicksResponseSerializer : ServerPacketSerializer<SynchronizeTicksResponseArgs>
{
    public override ServerOpCode ServerOpCode => ServerOpCode.SynchronizeTicks;

    public override void Serialize(ref SpanWriter writer, SynchronizeTicksResponseArgs responseArgs) =>
        writer.WriteInt32(responseArgs.Ticks);
}