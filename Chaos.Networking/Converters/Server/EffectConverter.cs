#region
using Chaos.DarkAges.Definitions;
using Chaos.IO.Memory;
using Chaos.Networking.Abstractions.Definitions;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;
#endregion

namespace Chaos.Networking.Converters.Server;

/// <summary>
///     Provides serialization and deserialization logic for <see cref="EffectArgs" />
/// </summary>
public sealed class EffectConverter : PacketConverterBase<EffectArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ServerOpCode.Effect;

    /// <inheritdoc />
    public override EffectArgs Deserialize(ref SpanReader reader)
    {
        var icon = reader.ReadUInt16();
        var color = reader.ReadByte();

        return new EffectArgs
        {
            EffectIcon = (byte)icon,
            EffectColor = (EffectColor)color
        };
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, EffectArgs args)
    {
        writer.WriteUInt16(args.EffectIcon);
        writer.WriteByte((byte)args.EffectColor);
        writer.WriteByte(0);
    }
}