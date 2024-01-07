using Chaos.Common.Definitions;
using Chaos.IO.Memory;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Converters.Client;

/// <summary>
///     Deserializes a buffer into <see cref="RaiseStatArgs" />
/// </summary>
public sealed class RaiseStatConverter : PacketConverterBase<RaiseStatArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ClientOpCode.RaiseStat;

    /// <inheritdoc />
    public override RaiseStatArgs Deserialize(ref SpanReader reader)
    {
        var stat = reader.ReadByte();

        return new RaiseStatArgs
        {
            Stat = (Stat)stat
        };
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, RaiseStatArgs args) => writer.WriteByte((byte)args.Stat);
}