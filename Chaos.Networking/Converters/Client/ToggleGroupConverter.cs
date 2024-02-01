using Chaos.IO.Memory;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Converters.Client;

/// <summary>
///     Provides packet serialization and deserialization logic for <see cref="ToggleGroupArgs" />
/// </summary>
public sealed class ToggleGroupConverter : PacketConverterBase<ToggleGroupArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ClientOpCode.ToggleGroup;

    /// <inheritdoc />
    public override ToggleGroupArgs Deserialize(ref SpanReader reader) => new();

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, ToggleGroupArgs args) { }
}