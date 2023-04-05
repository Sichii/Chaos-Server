using Chaos.IO.Memory;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Deserializers;

/// <summary>
///     Deserializes a buffer into <see cref="SynchronizeTicksArgs" />
/// </summary>
public sealed record SynchronizeTicksDeserializer : ClientPacketDeserializer<SynchronizeTicksArgs>
{
    /// <inheritdoc />
    public override ClientOpCode ClientOpCode => ClientOpCode.SynchronizeTicks;

    /// <inheritdoc />
    public override SynchronizeTicksArgs Deserialize(ref SpanReader reader)
    {
        var serverTicks = reader.ReadUInt32();
        var clientTicks = reader.ReadUInt32();

        return new SynchronizeTicksArgs(serverTicks, clientTicks);
    }
}