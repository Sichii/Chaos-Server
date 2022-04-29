using Chaos.Core.Definitions;

namespace Chaos.Core.Data;

public class Attributes
{
    public byte Ability { get; set; }

    public uint AbilityExp { get; set; }

    public byte BaseCon { get; set; }

    public byte BaseDex { get; set; }

    public uint BaseHP { get; set; }

    public byte BaseInt { get; set; }

    public uint BaseMP { get; set; }
    public byte BaseStr { get; set; }

    public byte BaseWis { get; set; }
    public int ConMod { get; set; }

    //Vitality

    public uint CurrentHP { get; set; }

    public uint CurrentMP { get; set; }
    public int DexMod { get; set; }
    public int DmgMod { get; set; }

    //Experience

    public uint Experience { get; set; }

    public uint GamePoints { get; set; }

    public uint Gold { get; set; }
    public int HitMod { get; set; }
    public int IntMod { get; set; }

    //Primary
    public byte Level { get; set; }
    public int MagicResistanceMod { get; set; }
    public int MaxHPMod { get; set; }
    public int MaxMPMod { get; set; }

    //addedValues
    public int StrMod { get; set; }

    public uint ToNextAbility { get; set; }

    public uint ToNextLevel { get; set; }

    public byte UnspentPoints { get; set; }
    public int WisMod { get; set; }
    public sbyte ArmorClass => 50;

    //Secondary
    public byte Blind => 0;
    public byte CurrentCon => (byte)Math.Clamp(BaseCon + ConMod, 0, byte.MaxValue);
    public byte CurrentDex => (byte)Math.Clamp(BaseDex + DexMod, 0, byte.MaxValue);
    public byte CurrentInt => (byte)Math.Clamp(BaseInt + IntMod, 0, byte.MaxValue);
    public byte CurrentStr => (byte)Math.Clamp(BaseStr + StrMod, 0, byte.MaxValue);
    public short CurrentWeight => 0;
    public byte CurrentWis => (byte)Math.Clamp(BaseWis + WisMod, 0, byte.MaxValue);
    public Element DefenseElement => Element.None;
    public byte Dmg => 0;
    public bool HasUnspentPoints => UnspentPoints != 0;
    public byte Hit => 0;
    public byte MagicResistance => 0;
    public MailFlag MailFlags => MailFlag.None;

    public uint MaximumHP => (uint)Math.Clamp(BaseHP + MaxHPMod, 0, uint.MaxValue);
    public uint MaximumMP => (uint)Math.Clamp(BaseMP + MaxMPMod, 0, uint.MaxValue);
    public short MaximumWeight => (short)(40 + BaseStr / 2);
    public Element OffenseElement => Element.None;
}