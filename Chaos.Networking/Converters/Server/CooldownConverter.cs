using Chaos.IO.Memory;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Converters.Server;

/// <summary>
///     Provides serialization and deserialization logic for <see cref="CooldownArgs" />
/// </summary>
public sealed class CooldownConverter : PacketConverterBase<CooldownArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ServerOpCode.Cooldown;

    /// <inheritdoc />
    public override CooldownArgs Deserialize(ref SpanReader reader)
    {
        var isSkill = reader.ReadBoolean();
        var slot = reader.ReadByte();
        var cooldownSecs = reader.ReadUInt32();

        return new CooldownArgs
        {
            IsSkill = isSkill,
            Slot = slot,
            CooldownSecs = cooldownSecs
        };
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, CooldownArgs args)
    {
        writer.WriteBoolean(args.IsSkill);
        writer.WriteByte(args.Slot);
        writer.WriteUInt32(args.CooldownSecs);
    }
}