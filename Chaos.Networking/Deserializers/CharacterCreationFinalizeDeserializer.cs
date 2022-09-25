using Chaos.Common.Definitions;
using Chaos.IO.Memory;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Deserializers;

public record CharacterCreationFinalizeDeserializer : ClientPacketDeserializer<CreateCharFinalizeArgs>
{
    public override ClientOpCode ClientOpCode => ClientOpCode.CreateCharFinalize;

    public override CreateCharFinalizeArgs Deserialize(ref SpanReader reader)
    {
        var hairStyle = reader.ReadByte();
        var gender = (Gender)reader.ReadByte();
        var hairColor = (DisplayColor)reader.ReadByte();

        return new CreateCharFinalizeArgs(hairStyle, gender, hairColor);
    }
}