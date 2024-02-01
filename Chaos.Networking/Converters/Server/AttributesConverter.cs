using Chaos.Common.Definitions;
using Chaos.IO.Memory;
using Chaos.Networking.Entities.Server;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Converters.Server;

/// <summary>
///     Provides serialization and deserialization logic for <see cref="AttributesArgs" />
/// </summary>
public sealed class AttributesConverter : PacketConverterBase<AttributesArgs>
{
    /// <inheritdoc />
    public override byte OpCode => (byte)ServerOpCode.Attributes;

    /// <inheritdoc />
    public override AttributesArgs Deserialize(ref SpanReader reader)
    {
        var attributesArgs = new AttributesArgs
        {
            StatUpdateType = (StatUpdateType)reader.ReadByte()
        };

        if (attributesArgs.StatUpdateType.HasFlag(StatUpdateType.Primary))
        {
            _ = reader.ReadBytes(3); //LI: what is this for?
            attributesArgs.Level = reader.ReadByte();
            attributesArgs.Ability = reader.ReadByte();
            attributesArgs.MaximumHp = reader.ReadUInt32();
            attributesArgs.MaximumMp = reader.ReadUInt32();
            attributesArgs.Str = reader.ReadByte();
            attributesArgs.Int = reader.ReadByte();
            attributesArgs.Wis = reader.ReadByte();
            attributesArgs.Con = reader.ReadByte();
            attributesArgs.Dex = reader.ReadByte();
            _ = reader.ReadBoolean(); //HasUnspentPoints
            attributesArgs.UnspentPoints = reader.ReadByte();
            attributesArgs.MaxWeight = reader.ReadInt16();
            attributesArgs.CurrentWeight = reader.ReadInt16();
            reader.ReadBytes(4); //LI: what is this for? 42 00 88 2E
        }

        if (attributesArgs.StatUpdateType.HasFlag(StatUpdateType.Vitality))
        {
            attributesArgs.CurrentHp = reader.ReadUInt32();
            attributesArgs.CurrentMp = reader.ReadUInt32();
        }

        if (attributesArgs.StatUpdateType.HasFlag(StatUpdateType.ExpGold))
        {
            attributesArgs.TotalExp = reader.ReadUInt32();
            attributesArgs.ToNextLevel = reader.ReadUInt32();
            attributesArgs.TotalAbility = reader.ReadUInt32();
            attributesArgs.ToNextAbility = reader.ReadUInt32();
            attributesArgs.GamePoints = reader.ReadUInt32();
            attributesArgs.Gold = reader.ReadUInt32();
        }

        if (attributesArgs.StatUpdateType.HasFlag(StatUpdateType.Secondary))
        {
            _ = reader.ReadByte(); //LI: what is this for?
            attributesArgs.Blind = reader.ReadByte() == 8;
            _ = reader.ReadBytes(3); //LI: what is this for?
            attributesArgs.HasUnreadMail = reader.ReadByte() == 16;
            attributesArgs.OffenseElement = (Element)reader.ReadByte();
            attributesArgs.DefenseElement = (Element)reader.ReadByte();
            attributesArgs.MagicResistance = reader.ReadByte();
            _ = reader.ReadByte(); //LI: what is this for?
            attributesArgs.Ac = reader.ReadSByte();
            attributesArgs.Dmg = reader.ReadByte();
            attributesArgs.Hit = reader.ReadByte();
        }

        return attributesArgs;
    }

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
            writer.WriteBytes(1, 0, 0); //LI: what is this for?
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
            writer.WriteBytes(new byte[4]); //LI: what is this for?  42 00 88 2E
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
            writer.WriteByte(0); //LI: what is this for?
            writer.WriteByte((byte)(args.Blind ? 8 : 0));
            writer.WriteBytes(new byte[3]); //LI: what is this for?
            writer.WriteByte((byte)(args.HasUnreadMail ? MailFlag.HasMail : MailFlag.None));
            writer.WriteByte((byte)args.OffenseElement);
            writer.WriteByte((byte)args.DefenseElement);
            writer.WriteByte(args.MagicResistance);
            writer.WriteByte(0); //LI: what is this for?
            writer.WriteSByte(args.Ac);
            writer.WriteByte(args.Dmg);
            writer.WriteByte(args.Hit);
        }
    }
}