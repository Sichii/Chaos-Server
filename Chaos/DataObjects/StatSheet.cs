using System;
using Chaos.Core.Definitions;

namespace Chaos.DataObjects;

public record StatSheet : Attributes
{
    public int Ability { get; set; } = 0;
    public int AcMod { get; set; }
    public int AttackSpeedMod { get; set; }
    public int ConMod { get; set; }
    public int CooldownReductionMod { get; set; }
    public int CurrentHp { get; set; }
    public int CurrentMp { get; set; }
    public Element DefenseElement { get; set; }
    public int DexMod { get; set; }
    public int IntMod { get; set; }
    public int Level { get; set; } = 1;
    public int MagicResistantMod { get; set; }
    public int MaximumHpMod { get; set; }
    public int MaximumMpMod { get; set; }
    public Element OffenseElement { get; set; }
    public int StrMod { get; set; }
    public int WisMod { get; set; }
    public sbyte EffectiveAc => (sbyte)Math.Clamp(Ac + AcMod, sbyte.MinValue, sbyte.MaxValue);
    public int EffectiveAttackSpeedMode => AttackSpeed + AttackSpeedMod;
    public byte EffectiveCon => (byte)Math.Clamp(Con + ConMod, byte.MinValue, byte.MaxValue);
    public int EffectiveCooldownReduction => CooldownReduction + CooldownReductionMod;
    public byte EffectiveDex => (byte)Math.Clamp(Dex + DexMod, byte.MinValue, byte.MaxValue);
    public byte EffectiveInt => (byte)Math.Clamp(Int + IntMod, byte.MinValue, byte.MaxValue);
    public sbyte EffectiveMagicResistance => (sbyte)Math.Clamp(MagicResistance + MagicResistantMod, sbyte.MinValue, sbyte.MaxValue);
    public int EffectiveMaximumHp => Math.Max(MaximumHp + MaximumHpMod, 0);
    public int EffectiveMaximumMp => Math.Max(MaximumMp + MaximumMpMod, 0);

    public byte EffectiveStr => (byte)Math.Clamp(Str + StrMod, byte.MinValue, byte.MaxValue);
    public byte EffectiveWis => (byte)Math.Clamp(Wis + WisMod, byte.MinValue, byte.MaxValue);
    public int HealthPercent => CurrentHp / EffectiveMaximumHp * 100;
    public int ManaPercent => CurrentMp / EffectiveMaximumMp * 100;

    public static StatSheet Maxed => new()
    {
        CurrentHp = int.MaxValue,
        CurrentMp = int.MaxValue,
        Str = int.MaxValue,
        Int = int.MaxValue,
        Wis = int.MaxValue,
        Con = int.MaxValue,
        Dex = int.MaxValue,
        MaximumHp = int.MaxValue,
        MaximumMp = int.MaxValue,
        MagicResistance = int.MaxValue,
        Ac = int.MinValue,
        CooldownReduction = int.MaxValue,
        AttackSpeed = int.MaxValue
    };
}