using Chaos.IO.Memory;
using Chaos.Networking.Model.Server;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Serializers;

public record SelfProfileSerializer : ServerPacketSerializer<SelfProfileArgs>
{
    public override ServerOpCode ServerOpCode => ServerOpCode.SelfProfile;

    public override void Serialize(ref SpanWriter writer, SelfProfileArgs args)
    {
        writer.WriteByte((byte)args.Nation);
        writer.WriteString8(args.GuildTitle ?? string.Empty);
        writer.WriteString8(args.Titles.FirstOrDefault() ?? string.Empty);

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
        writer.WriteBoolean(args.AdvClass.HasValue);
        writer.WriteBoolean(args.IsMaster);
        writer.WriteString8(classTitle!);
        writer.WriteString8(args.GuildName ?? string.Empty);
        writer.WriteByte((byte)Math.Min(255, args.LegendMarks.Count));

        foreach (var mark in args.LegendMarks.Take(byte.MaxValue))
        {
            writer.WriteByte((byte)mark.Icon);
            writer.WriteByte((byte)mark.Color);
            writer.WriteString8(mark.Key);
            writer.WriteString8(mark.Text);
        }
    }
}