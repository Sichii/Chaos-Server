#region
using Chaos.IO.Memory;
using Chaos.Networking.Abstractions.Definitions;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;
#endregion

namespace Chaos.Networking.Converters.Client;

/// <summary>
///     Provides packet serialization and deserialization logic for <see cref="HeartBeatResponseArgs" />
/// </summary>
public sealed class HeartBeatResponseConverter : PacketConverterBase<HeartBeatResponseArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ClientOpCode.HeartBeatResponse;

    /// <inheritdoc />
    public override HeartBeatResponseArgs Deserialize(ref SpanReader reader)
    {
        var first = reader.ReadByte();
        var second = reader.ReadByte();

        return new HeartBeatResponseArgs
        {
            First = first,
            Second = second
        };
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, HeartBeatResponseArgs responseArgs)
    {
        writer.WriteByte(responseArgs.First);
        writer.WriteByte(responseArgs.Second);
    }
}