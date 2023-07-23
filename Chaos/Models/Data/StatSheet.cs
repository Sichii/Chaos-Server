// ReSharper disable InconsistentNaming

using Chaos.Common.Definitions;
using Chaos.Common.Utilities;

namespace Chaos.Models.Data;

public record StatSheet : Attributes
{
    public int Ability
    {
        get => _ability;
        init => _ability = value;
    }

    public int AcMod
    {
        get => _acMod;
        init => _acMod = value;
    }

    public int AtkSpeedPctMod
    {
        get => _atkSpeedPctMod;
        init => _atkSpeedPctMod = value;
    }

    public int ConMod
    {
        get => _conMod;
        init => _conMod = value;
    }

    public int CurrentHp
    {
        get => _currentHp;
        init => _currentHp = value;
    }

    public int CurrentMp
    {
        get => _currentMp;
        init => _currentMp = value;
    }

    public Element DefenseElement
    {
        get => _defenseElement;
        init => _defenseElement = value;
    }

    public int DexMod
    {
        get => _dexMod;
        init => _dexMod = value;
    }

    public int DmgMod
    {
        get => _dmgMod;
        init => _dmgMod = value;
    }

    public int FlatSkillDamageMod
    {
        get => _flatSkillDamageMod;
        init => _flatSkillDamageMod = value;
    }

    public int FlatSpellDamageMod
    {
        get => _flatSpellDamageMod;
        init => _flatSpellDamageMod = value;
    }

    public int HitMod
    {
        get => _hitMod;
        init => _hitMod = value;
    }

    public int IntMod
    {
        get => _intMod;
        init => _intMod = value;
    }

    public int Level
    {
        get => _level;
        init => _level = value;
    }

    public int MagicResistanceMod
    {
        get => _magicResistanceMod;
        init => _magicResistanceMod = value;
    }

    public int MaximumHpMod
    {
        get => _maximumHpMod;
        init => _maximumHpMod = value;
    }

    public int MaximumMpMod
    {
        get => _maximumMpMod;
        init => _maximumMpMod = value;
    }

    public Element OffenseElement
    {
        get => _offenseElement;
        init => _offenseElement = value;
    }

    public int SkillDamagePctMod
    {
        get => _skillDamagePctMod;
        init => _skillDamagePctMod = value;
    }

    public int SpellDamagePctMod
    {
        get => _spellDamagePctMod;
        init => _spellDamagePctMod = value;
    }

    public int StrMod
    {
        get => _strMod;
        init => _strMod = value;
    }

    public int WisMod
    {
        get => _wisMod;
        init => _wisMod = value;
    }

    public sbyte EffectiveAc => (sbyte)Math.Clamp(Ac + AcMod, sbyte.MinValue, sbyte.MaxValue);

    public int EffectiveAttackSpeedPct => Math.Clamp(AtkSpeedPct + AtkSpeedPctMod, -200, 200);

    public byte EffectiveCon => (byte)Math.Clamp(Con + ConMod, byte.MinValue, byte.MaxValue);

    public byte EffectiveDex => (byte)Math.Clamp(Dex + DexMod, byte.MinValue, byte.MaxValue);

    public byte EffectiveDmg => (byte)Math.Clamp(Dmg + DmgMod, byte.MinValue, byte.MaxValue);

    public int EffectiveFlatSkillDamage => FlatSkillDamage + FlatSkillDamageMod;

    public int EffectiveFlatSpellDamage => FlatSpellDamage + FlatSpellDamageMod;

    public byte EffectiveHit => (byte)Math.Clamp(Hit + HitMod, byte.MinValue, byte.MaxValue);

    public byte EffectiveInt => (byte)Math.Clamp(Int + IntMod, byte.MinValue, byte.MaxValue);

    public byte EffectiveMagicResistance => (byte)Math.Clamp(MagicResistance + MagicResistanceMod, byte.MinValue, byte.MaxValue);

    public uint EffectiveMaximumHp => (uint)Math.Max(MaximumHp + MaximumHpMod, 0);

    public uint EffectiveMaximumMp => (uint)Math.Max(MaximumMp + MaximumMpMod, 0);

    public int EffectiveSkillDamagePct => SkillDamagePct + SkillDamagePctMod;

    public int EffectiveSpellDamagePct => SpellDamagePct + SpellDamagePctMod;

    public byte EffectiveStr => (byte)Math.Clamp(Str + StrMod, byte.MinValue, byte.MaxValue);

    public byte EffectiveWis => (byte)Math.Clamp(Wis + WisMod, byte.MinValue, byte.MaxValue);

    public decimal HealthPercent => Math.Clamp(CurrentHp / (decimal)EffectiveMaximumHp * 100, 0, 100);

    public decimal ManaPercent => CurrentMp / (decimal)EffectiveMaximumMp * 100;

    public static StatSheet Maxed =>
        new()
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
            _ac = -100,
            _atkSpeedPct = 500
        };

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
        Interlocked.Add(ref _maximumMpMod, other.MaximumMp);
        Interlocked.Add(ref _atkSpeedPctMod, other.AtkSpeedPct);
        Interlocked.Add(ref _flatSkillDamageMod, other.FlatSkillDamage);
        Interlocked.Add(ref _flatSpellDamageMod, other.FlatSpellDamage);
        Interlocked.Add(ref _skillDamagePctMod, other.SkillDamagePct);
        Interlocked.Add(ref _spellDamagePctMod, other.SpellDamagePct);
    }

    public void AddHealthPct(int pct) => InterlockedEx.SetValue(
        ref _currentHp,
        () => (int)Math.Clamp(EffectiveMaximumHp * (pct + HealthPercent) / 100m, 0, EffectiveMaximumHp));

    public void AddHp(int amount) => InterlockedEx.SetValue(
        ref _currentHp,
        () => (int)Math.Clamp(_currentHp + amount, 0, EffectiveMaximumHp));

    public void AddLevel(int amount = 1) => Interlocked.Add(ref _level, amount);

    public void AddManaPct(int pct) => InterlockedEx.SetValue(
        ref _currentMp,
        () => (int)Math.Clamp(EffectiveMaximumMp * (pct + ManaPercent) / 100m, 0, EffectiveMaximumMp));

    public void AddMp(int amount) => InterlockedEx.SetValue(
        ref _currentMp,
        () => (int)Math.Clamp(_currentMp + amount, 0, EffectiveMaximumMp));

    public int CalculateEffectiveAssailInterval(int baseAssailIntervalMs)
    {
        var modifier = EffectiveAttackSpeedPct / 100m;

        if (modifier < 0)
            return Convert.ToInt32(baseAssailIntervalMs * Math.Abs(modifier - 1));

        return Convert.ToInt32(baseAssailIntervalMs / (1 + modifier));
    }

    public int GetBaseStat(Stat stat) => stat switch
    {
        Stat.STR => Str,
        Stat.DEX => Dex,
        Stat.INT => Int,
        Stat.WIS => Wis,
        Stat.CON => Con,
        _        => throw new ArgumentOutOfRangeException()
    };

    public int GetEffectiveStat(Stat stat) => stat switch
    {
        Stat.STR => EffectiveStr,
        Stat.DEX => EffectiveDex,
        Stat.INT => EffectiveInt,
        Stat.WIS => EffectiveWis,
        Stat.CON => EffectiveCon,
        _        => throw new ArgumentOutOfRangeException()
    };

    public void SetDefenseElement(Element element) => _defenseElement = element;

    public void SetHealthPct(decimal pct) => InterlockedEx.SetValue(
        ref _currentHp,
        () => Convert.ToInt32(Math.Clamp(EffectiveMaximumHp * pct / 100m, 0, EffectiveMaximumHp)));

    public void SetHp(int amount) => Interlocked.Exchange(ref _currentHp, amount);

    public void SetLevel(int level) => Interlocked.Exchange(ref _level, level);

    public void SetManaPct(decimal pct) => InterlockedEx.SetValue(
        ref _currentMp,
        () => Convert.ToInt32(Math.Clamp(EffectiveMaximumMp * pct / 100m, 0, EffectiveMaximumMp)));

    public void SetMp(int amount) => Interlocked.Exchange(ref _currentMp, amount);

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
        Interlocked.Add(ref _maximumMpMod, -other.MaximumMp);
        Interlocked.Add(ref _atkSpeedPctMod, -other.AtkSpeedPct);
        Interlocked.Add(ref _flatSkillDamageMod, -other.FlatSkillDamage);
        Interlocked.Add(ref _flatSpellDamageMod, -other.FlatSpellDamage);
        Interlocked.Add(ref _skillDamagePctMod, -other.SkillDamagePct);
        Interlocked.Add(ref _spellDamagePctMod, -other.SpellDamagePct);
    }

    public void SubtractHealthPct(decimal pct) => InterlockedEx.SetValue(
        ref _currentHp,
        () => Convert.ToInt32(Math.Clamp(EffectiveMaximumHp * (HealthPercent - pct) / 100m, 0, EffectiveMaximumHp)));

    public void SubtractHp(int amount)
    {
        if (Interlocked.Add(ref _currentHp, -amount) < 0)
            _currentHp = 0;
    }

    public void SubtractLevel(int amount) => Interlocked.Add(ref _level, -amount);

    public void SubtractManaPct(decimal pct) => InterlockedEx.SetValue(
        ref _currentMp,
        () => Convert.ToInt32(Math.Clamp(EffectiveMaximumMp * (ManaPercent - pct) / 100m, 0, EffectiveMaximumMp)));

    public void SubtractMp(int amount)
    {
        if (Interlocked.Add(ref _currentMp, -amount) < 0)
            _currentMp = 0;
    }

    public bool TrySubtractHealthPct(decimal pct)
    {
        if (InterlockedEx.SetValue(
                ref _currentHp,
                () => Convert.ToInt32(Math.Clamp(EffectiveMaximumHp * (HealthPercent - pct) / 100m, int.MinValue, EffectiveMaximumHp)))
            <= 0)
        {
            InterlockedEx.SetValue(
                ref _currentHp,
                () => Convert.ToInt32(Math.Clamp(EffectiveMaximumHp * (HealthPercent + pct) / 100m, int.MinValue, EffectiveMaximumHp)));

            return false;
        }

        return true;
    }

    public bool TrySubtractHp(int amount)
    {
        if (Interlocked.Add(ref _currentHp, -amount) < 0)
        {
            Interlocked.Add(ref _currentHp, amount);

            return false;
        }

        return true;
    }

    public bool TrySubtractManaPct(decimal pct)
    {
        if (InterlockedEx.SetValue(
                ref _currentMp,
                () => Convert.ToInt32(Math.Clamp(EffectiveMaximumMp * (ManaPercent - pct) / 100m, int.MinValue, EffectiveMaximumMp)))
            <= 0)
        {
            InterlockedEx.SetValue(
                ref _currentMp,
                () => Convert.ToInt32(Math.Clamp(EffectiveMaximumMp * (ManaPercent + pct) / 100m, int.MinValue, EffectiveMaximumMp)));

            return false;
        }

        return true;
    }

    public bool TrySubtractMp(int amount)
    {
        if (Interlocked.Add(ref _currentMp, -amount) < 0)
        {
            Interlocked.Add(ref _currentMp, amount);

            return false;
        }

        return true;
    }

    #region SharedAttributes
    // ReSharper disable once FieldCanBeMadeReadOnly.Global
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
    protected int _atkSpeedPctMod;
    protected int _flatSpellDamageMod;
    protected int _flatSkillDamageMod;
    protected int _spellDamagePctMod;
    protected int _skillDamagePctMod;
    #endregion
}