using Chaos.IO.Memory;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Converters.Server;

/// <summary>
///     Provides serialization and deserialization logic for <see cref="SoundArgs" />
/// </summary>
public sealed class SoundConverter : PacketConverterBase<SoundArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ServerOpCode.Sound;

    /// <inheritdoc />
    public override SoundArgs Deserialize(ref SpanReader reader)
    {
        var indicatorOrIndex = reader.ReadByte();

        if (indicatorOrIndex == byte.MaxValue)
        {
            var musicIndex = reader.ReadByte();

            return new SoundArgs
            {
                IsMusic = true,
                Sound = musicIndex
            };
        }

        return new SoundArgs
        {
            IsMusic = false,
            Sound = indicatorOrIndex
        };
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, SoundArgs args)
    {
        if (args.IsMusic)
            writer.WriteByte(byte.MaxValue);

        writer.WriteByte(args.Sound);

        if (args.IsMusic)
            writer.WriteBytes(new byte[2]); //LI: what is this for?
    }
}