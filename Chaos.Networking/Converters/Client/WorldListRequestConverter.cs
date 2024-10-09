using Chaos.IO.Memory;
using Chaos.Networking.Abstractions.Definitions;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Converters.Client;

/// <summary>
///     Provides packet serialization and deserialization logic for <see cref="WorldListRequestArgs" />
/// </summary>
public sealed class WorldListRequestConverter : PacketConverterBase<WorldListRequestArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ClientOpCode.WorldListRequest;

    /// <inheritdoc />
    public override WorldListRequestArgs Deserialize(ref SpanReader reader) => new();

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, WorldListRequestArgs args) { }
}