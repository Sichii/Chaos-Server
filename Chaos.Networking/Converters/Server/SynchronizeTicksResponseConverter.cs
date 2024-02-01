using Chaos.IO.Memory;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Converters.Server;

/// <summary>
///     Provides serialization and deserialization logic for <see cref="SynchronizeTicksResponseArgs" />
/// </summary>
public sealed class SynchronizeTicksResponseConverter : PacketConverterBase<SynchronizeTicksResponseArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ServerOpCode.SynchronizeTicksResponse;

    /// <inheritdoc />
    public override SynchronizeTicksResponseArgs Deserialize(ref SpanReader reader)
    {
        var ticks = reader.ReadInt32();

        return new SynchronizeTicksResponseArgs
        {
            Ticks = ticks
        };
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, SynchronizeTicksResponseArgs responseArgs)
        => writer.WriteInt32(responseArgs.Ticks);
}