using Chaos.IO.Memory;
using Chaos.Networking.Abstractions.Definitions;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Converters.Server;

/// <summary>
///     Provides packet serialization and deserialization logic for <see cref="RefreshResponseArgs" />
/// </summary>
public class RefreshResponseConverter : PacketConverterBase<RefreshResponseArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ServerOpCode.RefreshResponse;

    /// <inheritdoc />
    public override RefreshResponseArgs Deserialize(ref SpanReader reader) => new();

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, RefreshResponseArgs args) { }
}