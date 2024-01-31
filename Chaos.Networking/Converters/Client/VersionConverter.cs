using Chaos.IO.Memory;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Converters.Client;

/// <summary>
///     Deserializes a buffer into <see cref="VersionArgs" />
/// </summary>
public sealed class VersionConverter : PacketConverterBase<VersionArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ClientOpCode.Version;

    /// <inheritdoc />
    public override VersionArgs Deserialize(ref SpanReader reader)
    {
        var version = reader.ReadUInt16();

        //4C 4B 00 TODO: what are these?

        return new VersionArgs
        {
            Version = version
        };
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, VersionArgs args)
    {
        writer.WriteUInt16(args.Version);

        writer.WriteBytes(
            [
                0x4C,
                0x4B,
                0x00
            ]);
    }
}