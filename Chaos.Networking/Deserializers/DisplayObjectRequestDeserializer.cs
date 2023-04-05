using Chaos.IO.Memory;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Deserializers;

/// <summary>
///     Deserializes a buffer into <see cref="DisplayObjectRequestArgs" />
/// </summary>
public sealed record DisplayObjectRequestDeserializer : ClientPacketDeserializer<DisplayObjectRequestArgs>
{
    /// <inheritdoc />
    public override ClientOpCode ClientOpCode => ClientOpCode.DisplayObjectRequest;

    /// <inheritdoc />
    public override DisplayObjectRequestArgs Deserialize(ref SpanReader reader)
    {
        var targetId = reader.ReadUInt32();

        return new DisplayObjectRequestArgs(targetId);
    }
}