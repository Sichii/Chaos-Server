// ReSharper disable InconsistentNaming

using System.Threading;
using Chaos.Common.Definitions;
using Chaos.Entities.Schemas.World;

namespace Chaos.Data;

public record StatSheet : Attributes
{
    public int Ability => _ability;

    public int AcMod => _acMod;
    public int ConMod => _conMod;

    public int CurrentHp => _currentHp;

    public int CurrentMp => _currentMp;

    public Element DefenseElement => _defenseElement;
    public int DexMod => _dexMod;

    public int DmgMod => _dmgMod;
    public sbyte EffectiveAc => (sbyte)Math.Clamp(Ac + AcMod, sbyte.MinValue, sbyte.MaxValue);
    public byte EffectiveCon => (byte)Math.Clamp(Con + ConMod, byte.MinValue, byte.MaxValue);
    public byte EffectiveDex => (byte)Math.Clamp(Dex + DexMod, byte.MinValue, byte.MaxValue);
    public byte EffectiveDmg => (byte)Math.Clamp(Dmg + DmgMod, byte.MinValue, byte.MaxValue);
    public byte EffectiveHit => (byte)Math.Clamp(Hit + HitMod, byte.MinValue, byte.MaxValue);
    public byte EffectiveInt => (byte)Math.Clamp(Int + IntMod, byte.MinValue, byte.MaxValue);
    public byte EffectiveMagicResistance => (byte)Math.Clamp(MagicResistance + MagicResistantMod, sbyte.MinValue, sbyte.MaxValue);
    public uint EffectiveMaximumHp => (uint)Math.Max(MaximumHp + MaximumHpMod, 0);
    public uint EffectiveMaximumMp => (uint)Math.Max(MaximumMp + MaximumMpMod, 0);
    public byte EffectiveStr => (byte)Math.Clamp(Str + StrMod, byte.MinValue, byte.MaxValue);
    public byte EffectiveWis => (byte)Math.Clamp(Wis + WisMod, byte.MinValue, byte.MaxValue);
    public int HealthPercent => (int)(CurrentHp / EffectiveMaximumHp * 100);
    public int HitMod => _hitMod;
    public int IntMod => _intMod;

    public int Level => _level;

    public int MagicResistantMod => _magicResistanceMod;
    public int ManaPercent => (int)(CurrentMp / EffectiveMaximumMp * 100);

    public static StatSheet Maxed => new()
    {
        _currentHp = int.MaxValue,
        _currentMp = int.MaxValue,
        _str = int.MaxValue,
        _int = int.MaxValue,
        _wis = int.MaxValue,
        _con = int.MaxValue,
        _dex = int.MaxValue,
        _maximumHp = int.MaxValue,
        _maximumMp = int.MaxValue,
        _magicResistance = int.MaxValue,
        _ac = int.MinValue
    };
    public int MaximumHpMod => _maximumHpMod;
    public int MaximumMpMod => _maximumMpMod;

    public Element OffenseElement => _offenseElement;
    public int StrMod => _strMod;
    public int WisMod => _wisMod;

    public StatSheet() { }

    public StatSheet(StatSheetSchema schema)
        : base(schema)
    {
        _acMod = 0;
        _dmgMod = 0;
        _hitMod = 0;
        _strMod = 0;
        _intMod = 0;
        _wisMod = 0;
        _conMod = 0;
        _dexMod = 0;
        _magicResistanceMod = 0;
        _maximumHpMod = 0;
        _maximumMpMod = 0;

        _currentHp = schema.CurrentHp;
        _currentMp = schema.CurrentMp;
        _ability = schema.Ability;
        _level = schema.Level;
        _defenseElement = Element.None;
        _offenseElement = Element.None;
    }

    public void AddBonus(Attributes other)
    {
        Interlocked.Add(ref _acMod, other.Ac);
        Interlocked.Add(ref _dmgMod, other.Dmg);
        Interlocked.Add(ref _hitMod, other.Hit);
        Interlocked.Add(ref _strMod, other.Str);
        Interlocked.Add(ref _intMod, other.Int);
        Interlocked.Add(ref _wisMod, other.Wis);
        Interlocked.Add(ref _conMod, other.Con);
        Interlocked.Add(ref _dexMod, other.Dex);
        Interlocked.Add(ref _magicResistanceMod, other.MagicResistance);
        Interlocked.Add(ref _maximumHpMod, other.MaximumHp);
        Interlocked.Add(ref _maximumMpMod, other.MaximumHp);
    }

    public void AddHp(int amount)
    {
        if (Interlocked.Add(ref _currentHp, amount) < 0)
            _currentHp = 0;
    }

    public void AddMp(int amount)
    {
        if (Interlocked.Add(ref _currentMp, amount) < 0)
            _currentMp = 0;
    }

    public void SetDefenseElement(Element element) => _defenseElement = element;

    public void SetOffenseElement(Element element) => _offenseElement = element;

    public void SubtractBonus(Attributes other)
    {
        Interlocked.Add(ref _acMod, -other.Ac);
        Interlocked.Add(ref _dmgMod, -other.Dmg);
        Interlocked.Add(ref _hitMod, -other.Hit);
        Interlocked.Add(ref _strMod, -other.Str);
        Interlocked.Add(ref _intMod, -other.Int);
        Interlocked.Add(ref _wisMod, -other.Wis);
        Interlocked.Add(ref _conMod, -other.Con);
        Interlocked.Add(ref _dexMod, -other.Dex);
        Interlocked.Add(ref _magicResistanceMod, -other.MagicResistance);
        Interlocked.Add(ref _maximumHpMod, -other.MaximumHp);
        Interlocked.Add(ref _maximumMpMod, -other.MaximumHp);
    }

    #region SharedAttributes
    protected int _ability;
    protected int _currentHp;
    protected int _currentMp;
    protected int _level;
    protected Element _defenseElement;
    protected Element _offenseElement;
    #endregion

    #region Mods
    protected int _acMod;
    protected int _conMod;
    protected int _dexMod;
    protected int _intMod;
    protected int _magicResistanceMod;
    protected int _maximumHpMod;
    protected int _maximumMpMod;
    protected int _strMod;
    protected int _wisMod;
    protected int _dmgMod;
    protected int _hitMod;
    #endregion
}