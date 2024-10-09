using Chaos.DarkAges.Definitions;
using Chaos.IO.Memory;
using Chaos.Networking.Abstractions.Definitions;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Converters.Client;

/// <summary>
///     Provides packet serialization and deserialization logic for <see cref="RaiseStatArgs" />
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