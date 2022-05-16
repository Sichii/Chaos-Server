using Chaos.Core.Memory;
using Chaos.Core.Utilities;
using Chaos.Networking.Model.Server;

namespace Chaos.Networking.Serializers;

public record DoorSerializer : ServerPacketSerializer<DoorArgs>
{
    public override ServerOpCode ServerOpCode => ServerOpCode.Door;

    public override void Serialize(ref SpanWriter writer, DoorArgs args)
    {
        writer.WriteByte((byte)args.Doors.Count);

        foreach (var door in args.Doors)
        {
            writer.WritePoint8(door.Point);
            writer.WriteBoolean(door.Closed);
            writer.WriteBoolean(door.OpenRight);
        }
    }
}