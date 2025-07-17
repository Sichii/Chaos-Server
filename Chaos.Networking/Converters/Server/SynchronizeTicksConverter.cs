#region
using Chaos.IO.Memory;
using Chaos.Networking.Abstractions.Definitions;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;
#endregion

namespace Chaos.Networking.Converters.Server;

/// <summary>
///     Provides serialization and deserialization logic for <see cref="SynchronizeTicksArgs" />
/// </summary>
public sealed class SynchronizeTicksConverter : PacketConverterBase<SynchronizeTicksArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ServerOpCode.SynchronizeTicks;

    /// <inheritdoc />
    public override SynchronizeTicksArgs Deserialize(ref SpanReader reader)
    {
        var ticks = reader.ReadUInt32();

        return new SynchronizeTicksArgs
        {
            Ticks = ticks
        };
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, SynchronizeTicksArgs args) => writer.WriteUInt32(args.Ticks);
}