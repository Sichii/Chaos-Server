using Chaos.IO.Memory;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Serializers;

/// <summary>
///     Serializes a <see cref="RedirectArgs" /> into a buffer
/// </summary>
public sealed record RedirectSerializer : ServerPacketSerializer<RedirectArgs>
{
    /// <inheritdoc />
    public override ServerOpCode ServerOpCode => ServerOpCode.Redirect;

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, RedirectArgs args)
    {
        writer.WriteBytes(
            args.Redirect
                .EndPoint
                .Address
                .GetAddressBytes()
                .Reverse()
                .ToArray());
        writer.WriteUInt16((ushort)args.Redirect.EndPoint.Port);

        var remaining = args.Redirect.Key.Length;

        remaining += writer.Encoding.GetBytes(args.Redirect.Name)
                           .Length;
        remaining += 7;

        writer.WriteByte((byte)remaining);
        writer.WriteByte(args.Redirect.Seed); //1
        writer.WriteData8(args.Redirect.Key); //1 + length
        writer.WriteString8(args.Redirect.Name); //1 + length
        writer.WriteUInt32(args.Redirect.Id); //4
    }
}