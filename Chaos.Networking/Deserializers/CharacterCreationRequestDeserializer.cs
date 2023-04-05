using Chaos.IO.Memory;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Deserializers;

/// <summary>
///     Deserializes a buffer into <see cref="CreateCharRequestArgs" />
/// </summary>
public sealed record CharacterCreationRequestDeserializer : ClientPacketDeserializer<CreateCharRequestArgs>
{
    /// <inheritdoc />
    public override ClientOpCode ClientOpCode => ClientOpCode.CreateCharRequest;

    /// <inheritdoc />
    public override CreateCharRequestArgs Deserialize(ref SpanReader reader)
    {
        var name = reader.ReadString8();
        var pw = reader.ReadString8();

        return new CreateCharRequestArgs(name, pw);
    }
}