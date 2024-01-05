using Chaos.IO.Memory;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Deserializers;

/// <summary>
///     Deserializes a buffer into <see cref="DisplayEntityRequestArgs" />
/// </summary>
public sealed class DisplayEntityRequestConverter : PacketConverterBase<DisplayEntityRequestArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ClientOpCode.DisplayEntityRequest;

    /// <inheritdoc />
    public override DisplayEntityRequestArgs Deserialize(ref SpanReader reader)
    {
        var targetId = reader.ReadUInt32();

        return new DisplayEntityRequestArgs
        {
            TargetId = targetId
        };
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, DisplayEntityRequestArgs args) => writer.WriteUInt32(args.TargetId);
}