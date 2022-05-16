using Chaos.Core.Memory;
using Chaos.Core.Utilities;
using Chaos.Networking.Model.Server;

namespace Chaos.Networking.Serializers;

public record RedirectSerializer : ServerPacketSerializer<RedirectArgs>
{
    public override ServerOpCode ServerOpCode => ServerOpCode.Redirect;

    public override void Serialize(ref SpanWriter writer, RedirectArgs args)
    {
        writer.WriteBytes(args.Redirect.EndPoint.Address.GetAddressBytes().Reverse().ToArray());
        writer.WriteUInt16((ushort)args.Redirect.EndPoint.Port);

        var remaining = args.Redirect.CryptoClient.Key.Length;
        remaining += writer.Encoding.GetBytes(args.Redirect.Name).Length;
        remaining += 7;

        writer.WriteByte((byte)remaining);
        writer.WriteByte(args.Redirect.CryptoClient.Seed); //1
        writer.WriteData8(args.Redirect.CryptoClient.Key); //1 + length
        writer.WriteString8(args.Redirect.Name); //1 + length
        writer.WriteUInt32(args.Redirect.Id); //4
    }
}