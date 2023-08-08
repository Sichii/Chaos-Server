using Chaos.IO.Memory;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Deserializers;

/// <summary>
///     Deserializes a buffer into <see cref="DisplayEntityRequestArgs" />
/// </summary>
public sealed record DisplayEntityRequestDeserializer : ClientPacketDeserializer<DisplayEntityRequestArgs>
{
    /// <inheritdoc />
    public override ClientOpCode ClientOpCode => ClientOpCode.DisplayEntityRequest;

    /// <inheritdoc />
    public override DisplayEntityRequestArgs Deserialize(ref SpanReader reader)
    {
        var targetId = reader.ReadUInt32();

        return new DisplayEntityRequestArgs(targetId);
    }
}