using Chaos.DarkAges.Definitions;
using Chaos.IO.Memory;
using Chaos.Networking.Abstractions.Definitions;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Converters.Client;

/// <summary>
///     Provides packet serialization and deserialization logic for <see cref="CreateCharFinalizeArgs" />
/// </summary>
public sealed class CreateCharFinalizeConverter : PacketConverterBase<CreateCharFinalizeArgs>
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