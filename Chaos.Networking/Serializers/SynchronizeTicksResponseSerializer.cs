using Chaos.Core.Utilities;
using Chaos.Networking.Model.Server;
using Chaos.Packets.Definitions;

namespace Chaos.Networking.Serializers;

public record SynchronizeTicksResponseSerializer : ServerPacketSerializer<SynchronizeTicksResponseArgs>
{
    public override ServerOpCode ServerOpCode => ServerOpCode.SynchronizeTicks;

    public override void Serialize(ref SpanWriter writer, SynchronizeTicksResponseArgs responseArgs) =>
        writer.WriteInt32(responseArgs.Ticks);
}