using Chaos.IO.Memory;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Converters.Server;

/// <summary>
///     Provides serialization and deserialization logic for <see cref="HealthBarArgs" />
/// </summary>
public sealed class HealthBarConverter : PacketConverterBase<HealthBarArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ServerOpCode.HealthBar;

    /// <inheritdoc />
    public override HealthBarArgs Deserialize(ref SpanReader reader)
    {
        var sourceId = reader.ReadUInt32();
        _ = reader.ReadByte(); //LI: what is this for?
        var healthPercent = reader.ReadByte();
        var sound = reader.ReadByte();

        return new HealthBarArgs
        {
            SourceId = sourceId,
            HealthPercent = healthPercent,
            Sound = sound == byte.MaxValue ? null : sound
        };
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, HealthBarArgs args)
    {
        writer.WriteUInt32(args.SourceId);
        writer.WriteByte(0);
        writer.WriteByte(args.HealthPercent);
        writer.WriteByte(args.Sound ?? byte.MaxValue);
    }
}