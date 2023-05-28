using Chaos.Common.Definitions;
using Chaos.IO.Memory;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Serializers;

/// <summary>
///     Serializes a <see cref="SelfProfileArgs" /> into a buffer
/// </summary>
public sealed record SelfProfileSerializer : ServerPacketSerializer<SelfProfileArgs>
{
    /// <inheritdoc />
    public override ServerOpCode ServerOpCode => ServerOpCode.SelfProfile;

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, SelfProfileArgs args)
    {
        writer.WriteByte((byte)args.Nation);
        writer.WriteString8(args.GuildRank ?? string.Empty);
        writer.WriteString8(args.Title ?? string.Empty);

        var str = args.GroupString;

        if (string.IsNullOrEmpty(str))
            str = !string.IsNullOrEmpty(args.SpouseName) ? $"Spouse: {args.SpouseName}" : "Adventuring Alone";

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

        var classTitle = args.BaseClass.ToString();

        if (args.AdvClass.HasValue && (args.AdvClass != AdvClass.None))
            classTitle = args.AdvClass.ToString();

        writer.WriteByte((byte)args.BaseClass);
        writer.WriteBoolean(args.AdvClass.HasValue && (args.AdvClass != AdvClass.None));
        writer.WriteBoolean(args.IsMaster);
        writer.WriteString8(classTitle!);
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