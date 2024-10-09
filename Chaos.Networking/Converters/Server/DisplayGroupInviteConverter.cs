using Chaos.DarkAges.Definitions;
using Chaos.IO.Memory;
using Chaos.Networking.Abstractions.Definitions;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Converters.Server;

/// <summary>
///     Provides serialization and deserialization logic for <see cref="DisplayGroupInviteArgs" />
/// </summary>
public sealed class DisplayGroupInviteConverter : PacketConverterBase<DisplayGroupInviteArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ServerOpCode.DisplayGroupInvite;

    /// <inheritdoc />
    public override DisplayGroupInviteArgs Deserialize(ref SpanReader reader)
    {
        var groupRequestType = (ServerGroupSwitch)reader.ReadByte();
        var sourceName = reader.ReadString8();
        var groupBoxInfo = default(DisplayGroupBoxInfo);

        if (groupRequestType == ServerGroupSwitch.Invite)
        {
            var name = reader.ReadString8();
            var note = reader.ReadString8();
            var minLevel = reader.ReadByte();
            var maxLevel = reader.ReadByte();

            var maxWarriors = reader.ReadByte();
            var currentWarriors = reader.ReadByte();

            var maxWizards = reader.ReadByte();
            var currentWizards = reader.ReadByte();

            var maxMonks = reader.ReadByte();
            var currentMonks = reader.ReadByte();

            var maxPriests = reader.ReadByte();
            var currentPriests = reader.ReadByte();

            var maxRogues = reader.ReadByte();
            var currentRogues = reader.ReadByte();

            groupBoxInfo = new DisplayGroupBoxInfo
            {
                Name = name,
                Note = note,
                MinLevel = minLevel,
                MaxLevel = maxLevel,
                MaxWarriors = maxWarriors,
                CurrentWarriors = currentWarriors,
                MaxWizards = maxWizards,
                CurrentWizards = currentWizards,
                MaxMonks = maxMonks,
                CurrentMonks = currentMonks,
                MaxPriests = maxPriests,
                CurrentPriests = currentPriests,
                MaxRogues = maxRogues,
                CurrentRogues = currentRogues
            };
        }

        return new DisplayGroupInviteArgs
        {
            ServerGroupSwitch = groupRequestType,
            SourceName = sourceName,
            GroupBoxInfo = groupBoxInfo
        };
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, DisplayGroupInviteArgs args)
    {
        writer.WriteByte((byte)args.ServerGroupSwitch);
        writer.WriteString8(args.SourceName);

        if (args.ServerGroupSwitch == ServerGroupSwitch.ShowGroupBox)
        {
            writer.WriteString8(args.GroupBoxInfo!.Name);
            writer.WriteString8(args.GroupBoxInfo.Note);
            writer.WriteByte(args.GroupBoxInfo.MinLevel);
            writer.WriteByte(args.GroupBoxInfo.MaxLevel);

            writer.WriteByte(args.GroupBoxInfo.MaxWarriors);
            writer.WriteByte(args.GroupBoxInfo.CurrentWarriors);

            writer.WriteByte(args.GroupBoxInfo.MaxWizards);
            writer.WriteByte(args.GroupBoxInfo.CurrentWizards);

            writer.WriteByte(args.GroupBoxInfo.MaxRogues);
            writer.WriteByte(args.GroupBoxInfo.CurrentRogues);

            writer.WriteByte(args.GroupBoxInfo.MaxPriests);
            writer.WriteByte(args.GroupBoxInfo.CurrentPriests);

            writer.WriteByte(args.GroupBoxInfo.MaxMonks);
            writer.WriteByte(args.GroupBoxInfo.CurrentMonks);

            writer.WriteByte(0);
        }
    }
}