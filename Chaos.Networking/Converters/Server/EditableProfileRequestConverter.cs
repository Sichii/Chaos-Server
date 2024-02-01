using Chaos.IO.Memory;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Converters.Server;

/// <summary>
///     Provides packet serialization and deserialization logic for <see cref="EditableProfileRequestArgs" />
/// </summary>
public class EditableProfileRequestConverter : PacketConverterBase<EditableProfileRequestArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ServerOpCode.EditableProfileRequest;

    /// <inheritdoc />
    public override EditableProfileRequestArgs Deserialize(ref SpanReader reader) => new();

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, EditableProfileRequestArgs args)
        => writer.WriteBytes(
            3,
            0,
            0,
            0,
            0,
            0);
}