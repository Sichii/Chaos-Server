using Chaos.Common.Definitions;
using Chaos.IO.Memory;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Serializers;

/// <summary>
///     Serializes a <see cref="SelfProfileArgs" /> into a buffer
/// </summary>
public sealed class SelfProfileConverter : PacketConverterBase<SelfProfileArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ServerOpCode.SelfProfile;

    /// <inheritdoc />
    public override SelfProfileArgs Deserialize(ref SpanReader reader)
    {
        var nation = reader.ReadByte();
        var guildRank = reader.ReadString8();
        var title = reader.ReadString8();
        var groupString = reader.ReadString8();
        var groupOpen = reader.ReadBoolean();
        _ = reader.ReadBoolean(); //groupbox fml

        //TODO: read groupbox shit
        var baseClass = reader.ReadByte();
        var enableMasterAbilitityMetadata = reader.ReadBoolean();
        var enableMasterQuestMetadata = reader.ReadBoolean();
        var displayClass = reader.ReadString8();
        var guildName = reader.ReadString8();
        var legendMarkCount = reader.ReadByte();
        var legendMarks = new List<LegendMarkInfo>(legendMarkCount);

        for (var i = 0; i < legendMarkCount; i++)
        {
            var icon = reader.ReadByte();
            var color = reader.ReadByte();
            var key = reader.ReadString8();
            var text = reader.ReadString8();

            legendMarks.Add(
                new LegendMarkInfo
                {
                    Icon = (MarkIcon)icon,
                    Color = (MarkColor)color,
                    Key = key,
                    Text = text
                });
        }

        return new SelfProfileArgs
        {
            Nation = (Nation)nation,
            GuildRank = guildRank,
            Title = title,
            GroupString = groupString,
            GroupOpen = groupOpen,
            BaseClass = (BaseClass)baseClass,
            EnableMasterAbilityMetaData = enableMasterAbilitityMetadata,
            EnableMasterQuestMetaData = enableMasterQuestMetadata,
            DisplayClass = displayClass,
            GuildName = guildName,
            LegendMarks = legendMarks
        };
    }

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, SelfProfileArgs args)
    {
        writer.WriteByte((byte)args.Nation);
        writer.WriteString8(args.GuildRank ?? string.Empty);
        writer.WriteString8(args.Title ?? string.Empty);

        var str = args.GroupString;

        if (string.IsNullOrEmpty(str))
            str = !string.IsNullOrEmpty(args.SpouseName) ? $"Spouse: {args.SpouseName}" : "Adventuring alone";

        writer.WriteString8(str);
        writer.WriteBoolean(args.GroupOpen);
        writer.WriteBoolean(false); //TODO: groupbox fml
        /*
         *  if(user.Group?.Box != null)
            {
                packet.WriteString8(user.Group.Leader.Name);
                packet.WriteString8(user.Group.Box.Text);
                packet.Write(new byte[13]); //other groupbox stuff will add later
            }
         */

        writer.WriteByte((byte)args.BaseClass);
        writer.WriteBoolean(args.EnableMasterAbilityMetaData);
        writer.WriteBoolean(args.EnableMasterQuestMetaData);
        writer.WriteString8(args.DisplayClass);
        writer.WriteString8(args.GuildName ?? string.Empty);
        writer.WriteByte((byte)Math.Min(byte.MaxValue, args.LegendMarks.Count));

        foreach (var mark in args.LegendMarks.Take(byte.MaxValue))
        {
            writer.WriteByte((byte)mark.Icon);
            writer.WriteByte((byte)mark.Color);
            writer.WriteString8(mark.Key);
            writer.WriteString8(mark.Text);
        }
    }
}