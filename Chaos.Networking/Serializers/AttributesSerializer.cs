using Chaos.Common.Definitions;
using Chaos.IO.Memory;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Serializers;

/// <summary>
///     Serializes a <see cref="AttributesArgs" /> into a buffer
/// </summary>
public sealed record AttributesSerializer : ServerPacketSerializer<AttributesArgs>
{
    /// <inheritdoc />
    public override ServerOpCode ServerOpCode => ServerOpCode.Attributes;

    /// <inheritdoc />
    public override void Serialize(ref SpanWriter writer, AttributesArgs args)
    {
        var updateType = args.StatUpdateType;

        if (args.IsAdmin)
            updateType |= StatUpdateType.GameMasterA;

        if (args.HasUnreadMail)
            updateType |= StatUpdateType.UnreadMail;

        writer.WriteByte((byte)updateType);

        if (args.StatUpdateType.HasFlag(StatUpdateType.Primary))
        {
            writer.WriteBytes(1, 0, 0); //dunno
            writer.WriteByte(args.Level);
            writer.WriteByte(args.Ability);
            writer.WriteUInt32(args.MaximumHp);
            writer.WriteUInt32(args.MaximumMp);
            writer.WriteByte(args.Str);
            writer.WriteByte(args.Int);
            writer.WriteByte(args.Wis);
            writer.WriteByte(args.Con);
            writer.WriteByte(args.Dex);
            writer.WriteBoolean(args.HasUnspentPoints);
            writer.WriteByte(args.UnspentPoints);
            writer.WriteInt16(args.MaxWeight);
            writer.WriteInt16(args.CurrentWeight);
            writer.WriteBytes(new byte[4]); //dunno  42 00 88 2E
        }

        if (args.StatUpdateType.HasFlag(StatUpdateType.Vitality))
        {
            writer.WriteUInt32(args.CurrentHp);
            writer.WriteUInt32(args.CurrentMp);
        }

        if (args.StatUpdateType.HasFlag(StatUpdateType.ExpGold))
        {
            writer.WriteUInt32(args.TotalExp);
            writer.WriteUInt32(args.ToNextLevel);
            writer.WriteUInt32(args.TotalAbility);
            writer.WriteUInt32(args.ToNextAbility);
            writer.WriteUInt32(args.GamePoints);
            writer.WriteUInt32(args.Gold);
        }

        if (args.StatUpdateType.HasFlag(StatUpdateType.Secondary))
        {
            writer.WriteByte(0); //dunno
            writer.WriteByte((byte)(args.Blind ? 8 : 0));
            writer.WriteBytes(new byte[3]); //dunno
            writer.WriteByte((byte)(args.HasUnreadMail ? MailFlag.HasMail : MailFlag.None));
            writer.WriteByte((byte)args.OffenseElement);
            writer.WriteByte((byte)args.DefenseElement);
            writer.WriteByte(args.MagicResistance);
            writer.WriteByte(0); //dunno
            writer.WriteSByte(args.Ac);
            writer.WriteByte(args.Dmg);
            writer.WriteByte(args.Hit);
        }
    }
}