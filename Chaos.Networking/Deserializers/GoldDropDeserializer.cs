using Chaos.Geometry;
using Chaos.IO.Memory;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Deserializers;

/// <summary>
///     Deserializes a buffer into <see cref="GoldDropArgs" />
/// </summary>
public sealed record GoldDropDeserializer : ClientPacketDeserializer<GoldDropArgs>
{
    /// <inheritdoc />
    public override ClientOpCode ClientOpCode => ClientOpCode.GoldDrop;

    /// <inheritdoc />
    public override GoldDropArgs Deserialize(ref SpanReader reader)
    {
        var amount = reader.ReadInt32();
        Point destinationPoint = reader.ReadPoint16();

        return new GoldDropArgs(amount, destinationPoint);
    }
}