using Chaos.IO.Memory;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Deserializers;

/// <summary>
///     Deserializes a buffer into <see cref="HeartBeatArgs" />
/// </summary>
public sealed record HeartBeatDeserializer : ClientPacketDeserializer<HeartBeatArgs>
{
    /// <inheritdoc />
    public override ClientOpCode ClientOpCode => ClientOpCode.HeartBeat;

    /// <inheritdoc />
    public override HeartBeatArgs Deserialize(ref SpanReader reader)
    {
        var first = reader.ReadByte();
        var second = reader.ReadByte();

        return new HeartBeatArgs(first, second);
    }
}