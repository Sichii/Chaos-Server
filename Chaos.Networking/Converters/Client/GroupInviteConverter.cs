using Chaos.Common.Definitions;
using Chaos.IO.Memory;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Converters.Client;

/// <summary>
///     Provides packet serialization and deserialization logic for <see cref="GroupInviteArgs" />
/// </summary>
public sealed class GroupInviteConverter : PacketConverterBase<GroupInviteArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ClientOpCode.GroupInvite;

    /// <inheritdoc />
    [SuppressMessage("ReSharper", "UnusedVariable")]
    public override GroupInviteArgs Deserialize(ref SpanReader reader)
    {
        var groupRequestType = (ClientGroupSwitch)reader.ReadByte();
        var targetName = reader.ReadString8();
        var groupBoxInfo = default(CreateGroupBoxInfo);

        if (groupRequestType == ClientGroupSwitch.CreateGroupbox)
        {
            var name = reader.ReadString8();
            var note = reader.ReadString8();
            var minLevel = reader.ReadByte();
            var maxLevel = reader.ReadByte();
            var maxWarriors = reader.ReadByte();
            var maxWizards = reader.ReadByte();
            var maxRogues = reader.ReadByte();
            var maxPriests = reader.ReadByte();
            var maxMonks = reader.ReadByte();

            groupBoxInfo = new CreateGroupBoxInfo
            {
                Name = name,
                Note = note,
                MinLevel = minLevel,
                MaxLevel = maxLevel,
                MaxWarriors = maxWarriors,
                MaxWizards = maxWizards,
                MaxRogues = maxRogues,
                MaxPriests = maxPriests,
                MaxMonks = maxMonks
            };
        }

        var args = new GroupInviteArgs
        {
            ClientGroupSwitch = groupRequestType,
            TargetName = targetName,
            GroupBoxInfo = groupBoxInfo
        };

        return args;
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, GroupInviteArgs args)
    {
        writer.WriteByte((byte)args.ClientGroupSwitch);
        writer.WriteString8(args.TargetName);

        if (args.ClientGroupSwitch == ClientGroupSwitch.CreateGroupbox)
        {
            writer.WriteString8(args.GroupBoxInfo!.Name);
            writer.WriteString8(args.GroupBoxInfo.Note);
            writer.WriteByte(args.GroupBoxInfo.MinLevel);
            writer.WriteByte(args.GroupBoxInfo.MaxLevel);
            writer.WriteByte(args.GroupBoxInfo.MaxWarriors);
            writer.WriteByte(args.GroupBoxInfo.MaxWizards);
            writer.WriteByte(args.GroupBoxInfo.MaxRogues);
            writer.WriteByte(args.GroupBoxInfo.MaxPriests);
            writer.WriteByte(args.GroupBoxInfo.MaxMonks);
        }
    }
}