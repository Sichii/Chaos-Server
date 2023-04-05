using Chaos.IO.Memory;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Deserializers;

/// <summary>
///     Deserializes a buffer into <see cref="SetNotepadArgs" />
/// </summary>
public sealed record SetNotepadDeserializer : ClientPacketDeserializer<SetNotepadArgs>
{
    /// <inheritdoc />
    public override ClientOpCode ClientOpCode => ClientOpCode.SetNotepad;

    /// <inheritdoc />
    public override SetNotepadArgs Deserialize(ref SpanReader reader)
    {
        var slot = reader.ReadByte();
        var message = reader.ReadString16();

        return new SetNotepadArgs(slot, message);
    }
}