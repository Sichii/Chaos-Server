using Chaos.IO.Memory;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Deserializers;

/// <summary>
///     Deserializes a buffer into <see cref="CreateCharRequestArgs" />
/// </summary>
public sealed class CharacterCreationRequestConverter : PacketConverterBase<CreateCharRequestArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ClientOpCode.CreateCharRequest;

    /// <inheritdoc />
    public override CreateCharRequestArgs Deserialize(ref SpanReader reader)
    {
        var name = reader.ReadString8();
        var pw = reader.ReadString8();

        return new CreateCharRequestArgs
        {
            Name = name,
            Password = pw
        };
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, CreateCharRequestArgs args)
    {
        writer.WriteString8(args.Name);
        writer.WriteString8(args.Password);
    }
}