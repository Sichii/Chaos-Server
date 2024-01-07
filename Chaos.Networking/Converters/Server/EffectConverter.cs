using Chaos.Common.Definitions;
using Chaos.IO.Memory;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Converters.Server;

/// <summary>
///     Serializes a <see cref="EffectArgs" /> into a buffer
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
    }
}