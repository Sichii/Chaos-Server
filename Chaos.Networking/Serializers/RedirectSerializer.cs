using Chaos.Entities.Networking.Server;
using Chaos.IO.Memory;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Serializers;

public record RedirectSerializer : ServerPacketSerializer<RedirectArgs>
{
    public override ServerOpCode ServerOpCode => ServerOpCode.Redirect;

    public override void Serialize(ref SpanWriter writer, RedirectArgs args)
    {
        writer.WriteBytes(args.Redirect.EndPoint.Address.GetAddressBytes().Reverse().ToArray());
        writer.WriteUInt16((ushort)args.Redirect.EndPoint.Port);

        var remaining = args.Redirect.Key.Length;
        remaining += writer.Encoding.GetBytes(args.Redirect.Name).Length;
        remaining += 7;

        writer.WriteByte((byte)remaining);
        writer.WriteByte(args.Redirect.Seed); //1
        writer.WriteData8(args.Redirect.Key); //1 + length
        writer.WriteString8(args.Redirect.Name); //1 + length
        writer.WriteUInt32(args.Redirect.Id); //4
    }
}