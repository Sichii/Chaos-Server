using Chaos.IO.Memory;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Converters.Client;

/// <summary>
///     Provides packet serialization and deserialization logic for <see cref="CreateCharInitialArgs" />
/// </summary>
public sealed class CreateCharInitialConverter : PacketConverterBase<CreateCharInitialArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ClientOpCode.CreateCharInitial;

    /// <inheritdoc />
    public override CreateCharInitialArgs Deserialize(ref SpanReader reader)
    {
        var name = reader.ReadString8();
        var pw = reader.ReadString8();

        return new CreateCharInitialArgs
        {
            Name = name,
            Password = pw
        };
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, CreateCharInitialArgs args)
    {
        writer.WriteString8(args.Name);
        writer.WriteString8(args.Password);
    }
}