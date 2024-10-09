using Chaos.IO.Memory;
using Chaos.Networking.Abstractions.Definitions;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Converters.Server;

/// <summary>
///     Provides packet serialization and deserialization logic for <see cref="CancelCastingArgs" />
/// </summary>
public class CancelCastingConverter : PacketConverterBase<CancelCastingArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ServerOpCode.CancelCasting;

    /// <inheritdoc />
    public override CancelCastingArgs Deserialize(ref SpanReader reader) => new();

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, CancelCastingArgs args) { }
}