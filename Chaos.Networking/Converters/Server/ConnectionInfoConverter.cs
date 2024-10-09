using Chaos.IO.Memory;
using Chaos.Networking.Abstractions.Definitions;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Converters.Server;

/// <summary>
///     Provides serialization and deserialization logic for <see cref="ConnectionInfoArgs" />
/// </summary>
public sealed class ConnectionInfoConverter : PacketConverterBase<ConnectionInfoArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ServerOpCode.ConnectionInfo;

    /// <inheritdoc />
    public override ConnectionInfoArgs Deserialize(ref SpanReader reader)
    {
        _ = reader.ReadByte(); //LI: what does this do?
        var tableCheckSum = reader.ReadUInt32();
        var seed = reader.ReadByte();
        var key = reader.ReadString8();

        return new ConnectionInfoArgs
        {
            TableCheckSum = tableCheckSum,
            Seed = seed,
            Key = key
        };
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, ConnectionInfoArgs args)
    {
        writer.WriteByte(0); //LI: what does this do?
        writer.WriteUInt32(args.TableCheckSum);
        writer.WriteByte(args.Seed);
        writer.WriteString8(args.Key);
    }
}