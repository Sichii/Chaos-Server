using Chaos.IO.Memory;
using Chaos.Networking.Model.Server;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Serializers;

public record DoorSerializer : ServerPacketSerializer<DoorArgs>
{
    public override ServerOpCode ServerOpCode => ServerOpCode.Door;

    public override void Serialize(ref SpanWriter writer, DoorArgs args)
    {
        writer.WriteByte((byte)args.Doors.Count);

        foreach (var door in args.Doors)
        {
            writer.WritePoint8((byte)door.X, (byte)door.Y);
            writer.WriteBoolean(door.Closed);
            writer.WriteBoolean(door.OpenRight);
        }
    }
}