using Chaos.IO.Memory;
using Chaos.Networking.Model.Client;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Deserializers;

public record ProfileDeserializer : ClientPacketDeserializer<ProfileArgs>
{
    public override ClientOpCode ClientOpCode => ClientOpCode.Profile;

    public override ProfileArgs Deserialize(ref SpanReader reader)
    {
        var totalLength = reader.ReadUInt16();
        var portraitLength = reader.ReadUInt16();
        var portraitData = reader.ReadBytes(portraitLength);
        var profileMessage = reader.ReadString16();

        return new ProfileArgs(portraitData, profileMessage);
    }
}