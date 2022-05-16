using Chaos.Core.Memory;
using Chaos.Core.Utilities;
using Chaos.Networking.Model.Client;

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