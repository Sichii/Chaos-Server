#region
using System.Net;
using Chaos.IO.Memory;
using Chaos.Networking.Abstractions.Definitions;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;
#endregion

namespace Chaos.Networking.Converters.Server;

/// <summary>
///     Provides serialization and deserialization logic for <see cref="RedirectArgs" />
/// </summary>
public sealed class RedirectConverter : PacketConverterBase<RedirectArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ServerOpCode.Redirect;

    /// <inheritdoc />
    public override RedirectArgs Deserialize(ref SpanReader reader)
    {
        var address = reader.ReadBytes(4);
        var port = reader.ReadUInt16();

        _ = reader.ReadByte(); //remaining bytes in packet
        var seed = reader.ReadByte();
        var key = reader.ReadString8();
        var name = reader.ReadString8();
        var id = reader.ReadUInt32();

        Array.Reverse(address);

        return new RedirectArgs
        {
            EndPoint = new IPEndPoint(new IPAddress(address), port),
            Seed = seed,
            Key = key,
            Name = name,
            Id = id
        };
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, RedirectArgs args)
    {
        var address = args.EndPoint.Address.GetAddressBytes();
        Array.Reverse(address);

        writer.WriteBytes(address);
        writer.WriteUInt16((ushort)args.EndPoint.Port);

        var remaining = args.Key.Length;

        remaining += writer.Encoding.GetBytes(args.Name)
                           .Length;
        remaining += 7;

        writer.WriteByte((byte)remaining);
        writer.WriteByte(args.Seed); //1
        writer.WriteString8(args.Key); //1 + length
        writer.WriteString8(args.Name); //1 + length
        writer.WriteUInt32(args.Id); //4
    }
}