using Chaos.Common.Definitions;
using Chaos.IO.Memory;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Converters.Client;

/// <summary>
///     Provides packet serialization and deserialization logic for <see cref="MenuInteractionArgs" />
/// </summary>
public sealed class MenuInteractionConverter : PacketConverterBase<MenuInteractionArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ClientOpCode.MenuInteraction;

    /// <inheritdoc />
    public override MenuInteractionArgs Deserialize(ref SpanReader reader)
    {
        var entityType = reader.ReadByte();
        var entityId = reader.ReadUInt32();
        var pursuitId = reader.ReadUInt16();

        var args = new MenuInteractionArgs
        {
            EntityType = (EntityType)entityType,
            EntityId = entityId,
            PursuitId = pursuitId
        };

        if (reader.Remaining == 1)
        {
            var slot = reader.ReadByte();

            if (slot > 0)
                args.Slot = slot;
        } else
        {
            var textArgs = reader.ReadArgs8();

            if (textArgs.Count != 0)
                args.Args = textArgs.ToArray();
        }

        return args;
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, MenuInteractionArgs args)
    {
        writer.WriteByte((byte)args.EntityType);
        writer.WriteUInt32(args.EntityId);
        writer.WriteUInt16(args.PursuitId);

        if (args.Slot.HasValue)
            writer.WriteByte(args.Slot.Value);
        else
            foreach (var arg in args.Args?.TakeLast(2) ?? Enumerable.Empty<string>())
                writer.WriteString8(arg);
    }
}