using Chaos.IO.Memory;
using Chaos.Networking.Model.Server;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Serializers;

public record WorldListSerializer : ServerPacketSerializer<WorldListArgs>
{
    public override ServerOpCode ServerOpCode => ServerOpCode.WorldList;

    public override void Serialize(ref SpanWriter writer, WorldListArgs args)
    {
        writer.WriteUInt16((ushort)args.WorldList.Count);
        writer.WriteUInt16((ushort)args.WorldList.Count);

        foreach (var user in args.WorldList)
        {
            writer.WriteByte((byte)user.BaseClass);
            writer.WriteByte((byte)user.Color);
            writer.WriteByte((byte)user.SocialStatus);
            writer.WriteString8(user.Title ?? string.Empty);
            writer.WriteBoolean(user.IsMaster);
            writer.WriteString8(user.Name);
        }
    }
}