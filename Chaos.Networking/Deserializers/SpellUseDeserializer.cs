using Chaos.IO.Memory;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Deserializers;

/// <summary>
///     Deserializes a buffer into <see cref="SpellUseArgs" />
/// </summary>
public sealed record SpellUseDeserializer : ClientPacketDeserializer<SpellUseArgs>
{
    /// <inheritdoc />
    public override ClientOpCode ClientOpCode => ClientOpCode.UseSpell;

    /// <inheritdoc />
    public override SpellUseArgs Deserialize(ref SpanReader reader)
    {
        var sourceSlot = reader.ReadByte();
        var argsData = reader.ReadData();

        return new SpellUseArgs(sourceSlot, argsData);
    }
}