using Chaos.IO.Memory;
using Chaos.Networking.Abstractions.Definitions;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Converters.Client;

/// <summary>
///     Provides packet serialization and deserialization logic for <see cref="DisplayEntityRequestArgs" />
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