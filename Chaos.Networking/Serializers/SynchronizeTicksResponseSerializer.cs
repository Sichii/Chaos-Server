using Chaos.Core.Memory;
using Chaos.Core.Utilities;
using Chaos.Networking.Model.Server;

namespace Chaos.Networking.Serializers;

public record SynchronizeTicksResponseSerializer : ServerPacketSerializer<SynchronizeTicksResponseArgs>
{
    public override ServerOpCode ServerOpCode => ServerOpCode.SynchronizeTicks;

    public override void Serialize(ref SpanWriter writer, SynchronizeTicksResponseArgs responseArgs) =>
        writer.WriteInt32(responseArgs.Ticks);
}