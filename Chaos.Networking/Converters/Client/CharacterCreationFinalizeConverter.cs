using Chaos.Common.Definitions;
using Chaos.IO.Memory;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Converters.Client;

/// <summary>
///     Deserializes a buffer into <see cref="CreateCharFinalizeArgs" />
/// </summary>
public sealed class CharacterCreationFinalizeConverter : PacketConverterBase<CreateCharFinalizeArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ClientOpCode.CreateCharFinalize;

    /// <inheritdoc />
    public override CreateCharFinalizeArgs Deserialize(ref SpanReader reader)
    {
        var hairStyle = reader.ReadByte();
        var gender = reader.ReadByte();
        var hairColor = reader.ReadByte();

        return new CreateCharFinalizeArgs
        {
            HairStyle = hairStyle,
            Gender = (Gender)gender,
            HairColor = (DisplayColor)hairColor
        };
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, CreateCharFinalizeArgs args)
    {
        writer.WriteByte(args.HairStyle);
        writer.WriteByte((byte)args.Gender);
        writer.WriteByte((byte)args.HairColor);
    }
}