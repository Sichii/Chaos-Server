using Chaos.Extensions.Networking;
using Chaos.Geometry;
using Chaos.IO.Memory;
using Chaos.Networking.Abstractions.Definitions;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Converters.Client;

/// <summary>
///     Provides packet serialization and deserialization logic for <see cref="GoldDropArgs" />
/// </summary>
public sealed class GoldDropConverter : PacketConverterBase<GoldDropArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ClientOpCode.GoldDrop;

    /// <inheritdoc />
    public override GoldDropArgs Deserialize(ref SpanReader reader)
    {
        var amount = reader.ReadInt32();
        var destinationPoint = reader.ReadPoint16();

        return new GoldDropArgs
        {
            Amount = amount,
            DestinationPoint = (Point)destinationPoint
        };
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, GoldDropArgs args)
    {
        writer.WriteInt32(args.Amount);
        writer.WritePoint16(args.DestinationPoint);
    }
}