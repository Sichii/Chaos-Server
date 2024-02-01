using System.Text;
using Chaos.IO.Memory;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Converters.Client;

/// <summary>
///     Provides packet serialization and deserialization logic for <see cref="ClientRedirectedArgs" />
/// </summary>
public sealed class ClientRedirectedConverter : PacketConverterBase<ClientRedirectedArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ClientOpCode.ClientRedirected;

    /// <inheritdoc />
    public override ClientRedirectedArgs Deserialize(ref SpanReader reader)
    {
        var seed = reader.ReadByte();
        var key = reader.ReadString8();
        var name = reader.ReadString8();
        var id = reader.ReadUInt32();

        return new ClientRedirectedArgs
        {
            Seed = seed,
            Key = Encoding.ASCII.GetBytes(key),
            Name = name,
            Id = id
        };
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, ClientRedirectedArgs args)
    {
        writer.WriteByte(args.Seed);
        writer.WriteString8(Encoding.ASCII.GetString(args.Key));
        writer.WriteString8(args.Name);
        writer.WriteUInt32(args.Id);
    }
}