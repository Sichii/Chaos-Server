using Chaos.IO.Memory;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Serializers;

/// <summary>
///     Serializes a <see cref="DoorArgs" /> into a buffer
/// </summary>
public sealed class DoorConverter : PacketConverterBase<DoorArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ServerOpCode.Door;

    /// <inheritdoc />
    public override DoorArgs Deserialize(ref SpanReader reader)
    {
        var count = reader.ReadByte();
        var doors = new List<DoorInfo>(count);

        for (var i = 0; i < count; i++)
        {
            var point = reader.ReadPoint8();
            var closed = reader.ReadBoolean();
            var openRight = reader.ReadBoolean();

            doors.Add(
                new DoorInfo
                {
                    X = point.X,
                    Y = point.Y,
                    Closed = closed,
                    OpenRight = openRight
                });
        }

        return new DoorArgs
        {
            Doors = doors
        };
    }

    /// <inheritdoc />
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