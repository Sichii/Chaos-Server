using Chaos.Entities.Networking.Client;
using Chaos.IO.Memory;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Deserializers;

public record WhisperDeserializer : ClientPacketDeserializer<WhisperArgs>
{
    public override ClientOpCode ClientOpCode => ClientOpCode.Whisper;

    public override WhisperArgs Deserialize(ref SpanReader reader)
    {
        var targetName = reader.ReadString8();
        var message = reader.ReadString8();

        return new WhisperArgs(targetName, message);
    }
}