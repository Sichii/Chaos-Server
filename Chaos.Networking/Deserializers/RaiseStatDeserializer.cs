using Chaos.Common.Definitions;
using Chaos.IO.Memory;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Deserializers;

/// <summary>
///     Deserializes a buffer into <see cref="RaiseStatArgs" />
/// </summary>
public sealed record RaiseStatDeserializer : ClientPacketDeserializer<RaiseStatArgs>
{
    /// <inheritdoc />
    public override ClientOpCode ClientOpCode => ClientOpCode.RaiseStat;

    /// <inheritdoc />
    public override RaiseStatArgs Deserialize(ref SpanReader reader)
    {
        var stat = (Stat)reader.ReadByte();

        return new RaiseStatArgs(stat);
    }
}