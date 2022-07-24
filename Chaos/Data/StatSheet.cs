// ReSharper disable InconsistentNaming

using System.Text.Json.Serialization;
using System.Threading;

namespace Chaos.Data;

public record StatSheet : Attributes
{
    protected int _ability;
    protected int _currentHp;
    protected int _currentMp;
    protected int _level;

    [JsonInclude]
    public int Ability
    {
        get => _ability;
        private set => _ability = value;
    }

    [JsonInclude]
    public int CurrentHp
    {
        get => _currentHp;
        private set => _currentHp = value;
    }

    [JsonInclude]
    public int CurrentMp
    {
        get => _currentMp;
        private set => _currentMp = value;
    }

    public Element DefenseElement { get; set; }

    [JsonInclude]
    public int Level
    {
        get => _level;
        private set => _level = value;
    }

    public Element OffenseElement { get; set; }

    public int AcMod => _acMod;
    public int AttackSpeedMod => _attackSpeedMod;
    public int ConMod => _conMod;
    public int CooldownReductionMod => _cooldownReductionMod;
    public int DexMod => _dexMod;

    public int DmgMod => _dmgMod;
    public sbyte EffectiveAc => (sbyte)Math.Clamp(Ac + AcMod, sbyte.MinValue, sbyte.MaxValue);
    public int EffectiveAttackSpeed => AttackSpeed + AttackSpeedMod;
    public byte EffectiveCon => (byte)Math.Clamp(Con + ConMod, byte.MinValue, byte.MaxValue);
    public int EffectiveCooldownReduction => CooldownReduction + CooldownReductionMod;
    public byte EffectiveDex => (byte)Math.Clamp(Dex + DexMod, byte.MinValue, byte.MaxValue);
    public byte EffectiveDmg => (byte)Math.Clamp(Dmg + DmgMod, byte.MinValue, byte.MaxValue);
    public byte EffectiveHit => (byte)Math.Clamp(Hit + HitMod, byte.MinValue, byte.MaxValue);
    public byte EffectiveInt => (byte)Math.Clamp(Int + IntMod, byte.MinValue, byte.MaxValue);
    public sbyte EffectiveMagicResistance => (sbyte)Math.Clamp(MagicResistance + MagicResistantMod, sbyte.MinValue, sbyte.MaxValue);
    public int EffectiveMaximumHp => Math.Max(MaximumHp + MaximumHpMod, 0);
    public int EffectiveMaximumMp => Math.Max(MaximumMp + MaximumMpMod, 0);
    public byte EffectiveStr => (byte)Math.Clamp(Str + StrMod, byte.MinValue, byte.MaxValue);
    public byte EffectiveWis => (byte)Math.Clamp(Wis + WisMod, byte.MinValue, byte.MaxValue);
    public int HealthPercent => CurrentHp / EffectiveMaximumHp * 100;
    public int HitMod => _hitMod;
    public int IntMod => _intMod;

    public int MagicResistantMod => _magicResistanceMod;
    public int ManaPercent => CurrentMp / EffectiveMaximumMp * 100;

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
        _ac = int.MinValue,
        _cooldownReduction = int.MaxValue,
        _attackSpeed = int.MaxValue
    };
    public int MaximumHpMod => _maximumHpMod;
    public int MaximumMpMod => _maximumMpMod;
    public int StrMod => _strMod;
    public int WisMod => _wisMod;

    public override void Add(Attributes other)
    {
        Interlocked.Add(ref _acMod, other.Ac);
        Interlocked.Add(ref _attackSpeedMod, other.AttackSpeed);
        Interlocked.Add(ref _cooldownReductionMod, other.CooldownReduction);
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

    public override void Subtract(Attributes other)
    {
        Interlocked.Add(ref _acMod, -other.Ac);
        Interlocked.Add(ref _attackSpeedMod, -other.AttackSpeed);
        Interlocked.Add(ref _cooldownReductionMod, -other.CooldownReduction);
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

    #region Mods
    protected int _acMod;
    protected int _attackSpeedMod;
    protected int _conMod;
    protected int _cooldownReductionMod;
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