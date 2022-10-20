using Chaos.IO.Memory;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Deserializers;

public sealed record WhisperDeserializer : ClientPacketDeserializer<WhisperArgs>
{
    public override ClientOpCode ClientOpCode => ClientOpCode.Whisper;

    public override WhisperArgs Deserialize(ref SpanReader reader)
    {
        var targetName = reader.ReadString8();
        var message = reader.ReadString8();

        return new WhisperArgs(targetName, message);
    }
}