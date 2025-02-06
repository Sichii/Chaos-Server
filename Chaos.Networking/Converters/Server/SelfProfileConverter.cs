#region
using Chaos.DarkAges.Definitions;
using Chaos.Extensions.Common;
using Chaos.IO.Memory;
using Chaos.Networking.Abstractions.Definitions;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;
#endregion

namespace Chaos.Networking.Converters.Server;

/// <summary>
///     Provides serialization and deserialization logic for <see cref="SelfProfileArgs" />
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
        var groupBox = reader.ReadBoolean();

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

        var legendMarks = args.LegendMarks;
        var legendMarkCount = legendMarks.Count;

        if (legendMarkCount > byte.MaxValue)
        {
            legendMarkCount = byte.MaxValue;

            legendMarks = legendMarks.TakeRandom(byte.MaxValue)
                                     .ToList();
        }

        writer.WriteByte((byte)legendMarkCount);

        foreach (var mark in legendMarks)
        {
            writer.WriteByte((byte)mark.Icon);
            writer.WriteByte((byte)mark.Color);
            writer.WriteString8(mark.Key);
            writer.WriteString8(mark.Text);
        }
    }
}