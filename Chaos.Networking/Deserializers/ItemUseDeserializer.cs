using Chaos.IO.Memory;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Deserializers;

/// <summary>
///     Deserializes a buffer into <see cref="ItemUseArgs" />
/// </summary>
public sealed record ItemUseDeserializer : ClientPacketDeserializer<ItemUseArgs>
{
    /// <inheritdoc />
    public override ClientOpCode ClientOpCode => ClientOpCode.UseItem;

    /// <inheritdoc />
    public override ItemUseArgs Deserialize(ref SpanReader reader)
    {
        var sourceSlot = reader.ReadByte();

        return new ItemUseArgs(sourceSlot);
    }
}