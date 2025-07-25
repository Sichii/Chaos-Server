#region
using Chaos.IO.Memory;
using Chaos.Networking.Abstractions.Definitions;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;
#endregion

namespace Chaos.Networking.Converters.Server;

/// <summary>
///     Provides serialization and deserialization logic for <see cref="HeartBeatArgs" />
/// </summary>
public sealed class HeartBeatConverter : PacketConverterBase<HeartBeatArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ServerOpCode.HeartBeat;

    /// <inheritdoc />
    public override HeartBeatArgs Deserialize(ref SpanReader reader)
    {
        var first = reader.ReadByte();
        var second = reader.ReadByte();

        return new HeartBeatArgs
        {
            First = first,
            Second = second
        };
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, HeartBeatArgs args)
    {
        writer.WriteByte(args.First);
        writer.WriteByte(args.Second);
    }
}