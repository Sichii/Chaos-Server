using Chaos.IO.Memory;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Converters.Client;

/// <summary>
///     Deserializes a buffer into <see cref="SynchronizeTicksArgs" />
/// </summary>
public sealed class SynchronizeTicksConverter : PacketConverterBase<SynchronizeTicksArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ClientOpCode.SynchronizeTicks;

    /// <inheritdoc />
    public override SynchronizeTicksArgs Deserialize(ref SpanReader reader)
    {
        var serverTicks = reader.ReadUInt32();
        var clientTicks = reader.ReadUInt32();

        return new SynchronizeTicksArgs
        {
            ServerTicks = serverTicks,
            ClientTicks = clientTicks
        };
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, SynchronizeTicksArgs args)
    {
        writer.WriteUInt32(args.ServerTicks);
        writer.WriteUInt32(args.ClientTicks);
    }
}