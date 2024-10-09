using Chaos.IO.Memory;
using Chaos.Networking.Abstractions.Definitions;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Converters.Client;

/// <summary>
///     Provides packet serialization and deserialization logic for <see cref="GoldDroppedOnCreatureArgs" />
/// </summary>
public sealed class GoldDroppedOnCreatureConverter : PacketConverterBase<GoldDroppedOnCreatureArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ClientOpCode.GoldDroppedOnCreature;

    /// <inheritdoc />
    public override GoldDroppedOnCreatureArgs Deserialize(ref SpanReader reader)
    {
        var amount = reader.ReadInt32();
        var targetId = reader.ReadUInt32();

        return new GoldDroppedOnCreatureArgs
        {
            Amount = amount,
            TargetId = targetId
        };
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, GoldDroppedOnCreatureArgs args)
    {
        writer.WriteInt32(args.Amount);
        writer.WriteUInt32(args.TargetId);
    }
}