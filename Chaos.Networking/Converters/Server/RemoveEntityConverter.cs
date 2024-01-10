using Chaos.IO.Memory;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Converters.Server;

/// <summary>
///     Serializes a <see cref="RemoveEntityArgs" /> into a buffer
/// </summary>
public sealed class RemoveEntityConverter : PacketConverterBase<RemoveEntityArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ServerOpCode.RemoveEntity;

    /// <inheritdoc />
    public override RemoveEntityArgs Deserialize(ref SpanReader reader)
    {
        var sourceId = reader.ReadUInt32();

        return new RemoveEntityArgs
        {
            SourceId = sourceId
        };
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, RemoveEntityArgs args) => writer.WriteUInt32(args.SourceId);
}