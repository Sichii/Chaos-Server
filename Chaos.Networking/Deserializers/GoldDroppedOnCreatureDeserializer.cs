using Chaos.IO.Memory;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Deserializers;

/// <summary>
///     Deserializes a buffer into <see cref="GoldDroppedOnCreatureArgs" />
/// </summary>
public sealed record GoldDroppedOnCreatureDeserializer : ClientPacketDeserializer<GoldDroppedOnCreatureArgs>
{
    /// <inheritdoc />
    public override ClientOpCode ClientOpCode => ClientOpCode.GoldDroppedOnCreature;

    /// <inheritdoc />
    public override GoldDroppedOnCreatureArgs Deserialize(ref SpanReader reader)
    {
        var amount = reader.ReadInt32();
        var targetId = reader.ReadUInt32();

        return new GoldDroppedOnCreatureArgs(amount, targetId);
    }
}