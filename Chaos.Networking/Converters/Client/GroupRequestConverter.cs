using Chaos.Common.Definitions;
using Chaos.IO.Memory;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Converters.Client;

/// <summary>
///     Deserializes a buffer into <see cref="GroupRequestArgs" />
/// </summary>
public sealed class GroupRequestConverter : PacketConverterBase<GroupRequestArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ClientOpCode.GroupRequest;

    /// <inheritdoc />
    [SuppressMessage("ReSharper", "UnusedVariable")]
    public override GroupRequestArgs Deserialize(ref SpanReader reader)
    {
        var groupRequestType = reader.ReadByte();

        var args = new GroupRequestArgs
        {
            GroupRequestType = (GroupRequestType)groupRequestType
        };

        //TODO: finish group box stuff
        if (groupRequestType == (byte)GroupRequestType.Groupbox)
        {
            var leader = reader.ReadString8();
            var test = reader.ReadString8();
            reader.ReadByte(); //unknown
            var minLevel = reader.ReadByte();
            var maxLevel = reader.ReadByte();
            var maxOfClass = new byte[6];

            maxOfClass[(byte)BaseClass.Warrior] = reader.ReadByte();
            maxOfClass[(byte)BaseClass.Wizard] = reader.ReadByte();
            maxOfClass[(byte)BaseClass.Rogue] = reader.ReadByte();
            maxOfClass[(byte)BaseClass.Priest] = reader.ReadByte();
            maxOfClass[(byte)BaseClass.Monk] = reader.ReadByte();
        }

        var targetName = reader.ReadString8();

        args.TargetName = targetName;

        return args;
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, GroupRequestArgs args)
    {
        writer.WriteByte((byte)args.GroupRequestType);

        if (args.GroupRequestType == GroupRequestType.Groupbox)
        {
            /* TODO: finish group box stuff
            writer.WriteString8(args.Leader);
            writer.WriteString8(args.Test);
            writer.WriteByte(0); //unknown
            writer.WriteByte(args.MinLevel);
            writer.WriteByte(args.MaxLevel);
            writer.WriteByte(args.MaxOfClass[(byte)BaseClass.Warrior]);
            writer.WriteByte(args.MaxOfClass[(byte)BaseClass.Wizard]);
            writer.WriteByte(args.MaxOfClass[(byte)BaseClass.Rogue]);
            writer.WriteByte(args.MaxOfClass[(byte)BaseClass.Priest]);
            writer.WriteByte(args.MaxOfClass[(byte)BaseClass.Monk]);
            */
        }

        writer.WriteString8(args.TargetName);
    }
}