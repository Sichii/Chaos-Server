using Chaos.IO.Memory;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Serializers;

/// <summary>
///     Serializes a <see cref="RemoveItemFromPaneArgs" /> into a buffer
/// </summary>
public sealed record RemoveItemFromPaneSerializer : ServerPacketSerializer<RemoveItemFromPaneArgs>
{
    /// <inheritdoc />
    public override ServerOpCode ServerOpCode => ServerOpCode.RemoveItemFromPane;

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, RemoveItemFromPaneArgs args) => writer.WriteByte(args.Slot);
}