using Chaos.IO.Memory;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Deserializers;

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

        return new VersionArgs
        {
            Version = version
        };
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, VersionArgs args) => writer.WriteUInt16(args.Version);
}