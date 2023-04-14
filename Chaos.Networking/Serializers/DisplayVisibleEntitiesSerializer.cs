using Chaos.Common.Definitions;
using Chaos.IO.Memory;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Serializers;

/// <summary>
///     Serializes a <see cref="DisplayVisibleEntitiesArgs" /> into a buffer
/// </summary>
public sealed record DisplayVisibleEntitiesSerializer : ServerPacketSerializer<DisplayVisibleEntitiesArgs>
{
    /// <inheritdoc />
    public override ServerOpCode ServerOpCode => ServerOpCode.DisplayVisibleEntities;

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, DisplayVisibleEntitiesArgs args)
    {
        writer.WriteUInt16((ushort)args.VisibleObjects.Count);

        foreach (var obj in args.VisibleObjects)
        {
            writer.WritePoint16((ushort)obj.X, (ushort)obj.Y);
            writer.WriteUInt32(obj.Id);
            writer.WriteUInt16(obj.Sprite);

            switch (obj)
            {
                case CreatureInfo creature:
                {
                    writer.WriteBytes(new byte[4]); //dunno
                    writer.WriteByte((byte)creature.Direction);
                    writer.WriteByte(0); //dunno
                    writer.WriteByte((byte)creature.CreatureType);

                    if (creature.CreatureType == CreatureType.Merchant)
                        writer.WriteString8(creature.Name);

                    break;
                }
                case GroundItemInfo groundItem:
                    writer.WriteByte((byte)groundItem.Color);
                    writer.WriteBytes(new byte[2]);

                    break;
            }
        }
    }
}