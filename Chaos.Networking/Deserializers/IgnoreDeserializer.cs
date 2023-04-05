using Chaos.Common.Definitions;
using Chaos.IO.Memory;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Deserializers;

/// <summary>
///     Deserializes a buffer into <see cref="IgnoreArgs" />
/// </summary>
public sealed record IgnoreDeserializer : ClientPacketDeserializer<IgnoreArgs>
{
    /// <inheritdoc />
    public override ClientOpCode ClientOpCode => ClientOpCode.Ignore;

    /// <inheritdoc />
    public override IgnoreArgs Deserialize(ref SpanReader reader)
    {
        var ignoreType = (IgnoreType)reader.ReadByte();
        var targetName = default(string);

        if (ignoreType != IgnoreType.Request)
            targetName = reader.ReadString8();

        return new IgnoreArgs(ignoreType, targetName);
    }
}