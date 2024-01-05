using Chaos.IO.Memory;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Serializers;

/// <summary>
///     Serializes a <see cref="RemoveObjectArgs" /> into a buffer
/// </summary>
public sealed class RemoveObjectConverter : PacketConverterBase<RemoveObjectArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ServerOpCode.RemoveObject;

    /// <inheritdoc />
    public override RemoveObjectArgs Deserialize(ref SpanReader reader)
    {
        var sourceId = reader.ReadUInt32();

        return new RemoveObjectArgs
        {
            SourceId = sourceId
        };
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, RemoveObjectArgs args) => writer.WriteUInt32(args.SourceId);
}