using Chaos.Geometry;
using Chaos.IO.Memory;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Deserializers;

/// <summary>
///     Deserializes a buffer into <see cref="ItemDropArgs" />
/// </summary>
public sealed record ItemDropDeserializer : ClientPacketDeserializer<ItemDropArgs>
{
    /// <inheritdoc />
    public override ClientOpCode ClientOpCode => ClientOpCode.ItemDrop;

    /// <inheritdoc />
    public override ItemDropArgs Deserialize(ref SpanReader reader)
    {
        var sourceSlot = reader.ReadByte();
        Point destinationPoint = reader.ReadPoint16();
        var count = reader.ReadInt32();

        return new ItemDropArgs(sourceSlot, destinationPoint, count);
    }
}