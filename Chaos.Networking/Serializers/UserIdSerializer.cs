using Chaos.IO.Memory;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Serializers;

public record UserIdSerializer : ServerPacketSerializer<UserIdArgs>
{
    public override ServerOpCode ServerOpCode => ServerOpCode.UserId;

    public override void Serialize(ref SpanWriter writer, UserIdArgs args)
    {
        writer.WriteUInt32(args.Id);
        writer.WriteByte((byte)args.Direction);
        writer.WriteByte(0); //dunno
        writer.WriteByte((byte)args.BaseClass);
        writer.WriteByte(0); //dunno
        writer.WriteByte((byte)args.Gender);
    }
}