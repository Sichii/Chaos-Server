using Chaos.Entities.Networking.Server;
using Chaos.IO.Memory;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

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