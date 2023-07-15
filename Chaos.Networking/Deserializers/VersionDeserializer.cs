using Chaos.IO.Memory;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Deserializers;

/// <summary>
///     Deserializes a buffer into <see cref="VersionArgs" />
/// </summary>
public sealed record VersionDeserializer : ClientPacketDeserializer<VersionArgs>
{
    /// <inheritdoc />
    public override ClientOpCode ClientOpCode => ClientOpCode.Version;

    /// <inheritdoc />
    public override VersionArgs Deserialize(ref SpanReader reader)
    {
        var version = reader.ReadUInt16();

        return new VersionArgs(version);
    }
}