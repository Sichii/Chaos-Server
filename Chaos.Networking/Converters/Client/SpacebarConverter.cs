using Chaos.IO.Memory;
using Chaos.Networking.Abstractions.Definitions;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Converters.Client;

/// <summary>
///     Provides packet serialization and deserialization logic for <see cref="SpacebarArgs" />
/// </summary>
public sealed class SpacebarConverter : PacketConverterBase<SpacebarArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ClientOpCode.Spacebar;

    /// <inheritdoc />
    public override SpacebarArgs Deserialize(ref SpanReader reader) => new();

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, SpacebarArgs args) { }
}