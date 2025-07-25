#region
using Chaos.IO.Memory;
using Chaos.Networking.Abstractions.Definitions;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;
#endregion

namespace Chaos.Networking.Converters.Client;

/// <summary>
///     Provides packet serialization and deserialization logic for <see cref="SynchronizeTicksResponseArgs" />
/// </summary>
public sealed class SynchronizeTicksResponseConverter : PacketConverterBase<SynchronizeTicksResponseArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ClientOpCode.SynchronizeTicksResponse;

    /// <inheritdoc />
    public override SynchronizeTicksResponseArgs Deserialize(ref SpanReader reader)
    {
        var serverTicks = reader.ReadUInt32();
        var clientTicks = reader.ReadUInt32();

        return new SynchronizeTicksResponseArgs
        {
            ServerTicks = serverTicks,
            ClientTicks = clientTicks
        };
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, SynchronizeTicksResponseArgs responseArgs)
    {
        writer.WriteUInt32(responseArgs.ServerTicks);
        writer.WriteUInt32(responseArgs.ClientTicks);
    }
}