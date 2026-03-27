#region
using Chaos.DarkAges.Definitions;
using Chaos.Models.Data;
using FluentAssertions;
#endregion

namespace Chaos.Tests;

public sealed class StatSheetTests
{
    private static StatSheet CreateSheet(
        int maxHp = 1000,
        int maxMp = 500,
        int currentHp = 1000,
        int currentMp = 500)
        => new()
        {
            MaximumHp = maxHp,
            MaximumMp = maxMp,
            CurrentHp = currentHp,
            CurrentMp = currentMp
        };

    #region Maxed
    [Test]
    public void Maxed_ShouldHaveExpectedValues()
    {
        var maxed = StatSheet.Maxed;

        maxed.CurrentHp
             .Should()
             .Be(int.MaxValue);

        maxed.CurrentMp
             .Should()
             .Be(int.MaxValue);

        maxed.Str
             .Should()
             .Be(int.MaxValue);

        maxed.Int
             .Should()
             .Be(int.MaxValue);

        maxed.Wis
             .Should()
             .Be(int.MaxValue);

        maxed.Con
             .Should()
             .Be(int.MaxValue);

        maxed.Dex
             .Should()
             .Be(int.MaxValue);

        maxed.MaximumHp
             .Should()
             .Be(int.MaxValue);

        maxed.MaximumMp
             .Should()
             .Be(int.MaxValue);

        maxed.MagicResistance
             .Should()
             .Be(int.MaxValue);

        maxed.Ac
             .Should()
             .Be(-100);

        maxed.AtkSpeedPct
             .Should()
             .Be(500);
    }
    #endregion

    #region HP Management
    [Test]
    public void AddHp_ShouldClampToMaximum()
    {
        var sheet = CreateSheet(100, currentHp: 90);

        sheet.AddHp(50);

        sheet.CurrentHp
             .Should()
             .Be(100);
    }

    [Test]
    public void AddHp_ShouldAddNormally_WhenWithinBounds()
    {
        var sheet = CreateSheet(100, currentHp: 50);

        sheet.AddHp(20);

        sheet.CurrentHp
             .Should()
             .Be(70);
    }

    [Test]
    public void SubtractHp_ShouldFloorAtZero()
    {
        var sheet = CreateSheet(currentHp: 10);

        sheet.SubtractHp(50);

        sheet.CurrentHp
             .Should()
             .Be(0);
    }

    [Test]
    public void SubtractHp_ShouldSubtractNormally()
    {
        var sheet = CreateSheet(currentHp: 100);

        sheet.SubtractHp(30);

        sheet.CurrentHp
             .Should()
             .Be(70);
    }

    [Test]
    public void SetHp_ShouldSetExactValue()
    {
        var sheet = CreateSheet();

        sheet.SetHp(42);

        sheet.CurrentHp
             .Should()
             .Be(42);
    }

    [Test]
    public void TrySubtractHp_ShouldSucceed_WhenSufficientHp()
    {
        var sheet = CreateSheet(currentHp: 100);

        var result = sheet.TrySubtractHp(50);

        result.Should()
              .BeTrue();

        sheet.CurrentHp
             .Should()
             .Be(50);
    }

    [Test]
    public void TrySubtractHp_ShouldFail_WhenInsufficientHp()
    {
        var sheet = CreateSheet(currentHp: 10);

        var result = sheet.TrySubtractHp(50);

        result.Should()
              .BeFalse();

        sheet.CurrentHp
             .Should()
             .Be(10);
    }
    #endregion

    #region HP Percentage
    [Test]
    public void AddHealthPct_ShouldAddPercentOfMaxHp()
    {
        var sheet = CreateSheet(100, currentHp: 50);

        sheet.AddHealthPct(25);

        sheet.CurrentHp
             .Should()
             .Be(75);
    }

    [Test]
    public void SubtractHealthPct_ShouldSubtractPercentOfMaxHp()
    {
        var sheet = CreateSheet(100, currentHp: 100);

        sheet.SubtractHealthPct(25);

        sheet.CurrentHp
             .Should()
             .Be(75);
    }

    [Test]
    public void SetHealthPct_ShouldSetToPercentOfMaxHp()
    {
        var sheet = CreateSheet(200, currentHp: 0);

        sheet.SetHealthPct(50);

        sheet.CurrentHp
             .Should()
             .Be(100);
    }

    [Test]
    public void TrySubtractHealthPct_ShouldSucceed_WhenSufficient()
    {
        var sheet = CreateSheet(100, currentHp: 100);

        var result = sheet.TrySubtractHealthPct(50);

        result.Should()
              .BeTrue();

        sheet.CurrentHp
             .Should()
             .Be(50);
    }

    [Test]
    public void TrySubtractHealthPct_ShouldFail_WhenInsufficient()
    {
        var sheet = CreateSheet(100, currentHp: 1);

        var result = sheet.TrySubtractHealthPct(50);

        result.Should()
              .BeFalse();
    }
    #endregion

    #region MP Management
    [Test]
    public void AddMp_ShouldClampToMaximum()
    {
        var sheet = CreateSheet(maxMp: 100, currentMp: 90);

        sheet.AddMp(50);

        sheet.CurrentMp
             .Should()
             .Be(100);
    }

    [Test]
    public void SubtractMp_ShouldFloorAtZero()
    {
        var sheet = CreateSheet(currentMp: 10);

        sheet.SubtractMp(50);

        sheet.CurrentMp
             .Should()
             .Be(0);
    }

    [Test]
    public void SetMp_ShouldSetExactValue()
    {
        var sheet = CreateSheet();

        sheet.SetMp(42);

        sheet.CurrentMp
             .Should()
             .Be(42);
    }

    [Test]
    public void TrySubtractMp_ShouldSucceed_WhenSufficientMp()
    {
        var sheet = CreateSheet(currentMp: 100);

        var result = sheet.TrySubtractMp(50);

        result.Should()
              .BeTrue();

        sheet.CurrentMp
             .Should()
             .Be(50);
    }

    [Test]
    public void TrySubtractMp_ShouldFail_WhenInsufficientMp()
    {
        var sheet = CreateSheet(currentMp: 10);

        var result = sheet.TrySubtractMp(50);

        result.Should()
              .BeFalse();

        sheet.CurrentMp
             .Should()
             .Be(10);
    }
    #endregion

    #region MP Percentage
    [Test]
    public void AddManaPct_ShouldAddPercentOfMaxMp()
    {
        var sheet = CreateSheet(maxMp: 100, currentMp: 50);

        sheet.AddManaPct(25);

        sheet.CurrentMp
             .Should()
             .Be(75);
    }

    [Test]
    public void SubtractManaPct_ShouldSubtractPercentOfMaxMp()
    {
        var sheet = CreateSheet(maxMp: 100, currentMp: 100);

        sheet.SubtractManaPct(25);

        sheet.CurrentMp
             .Should()
             .Be(75);
    }

    [Test]
    public void SetManaPct_ShouldSetToPercentOfMaxMp()
    {
        var sheet = CreateSheet(maxMp: 200, currentMp: 0);

        sheet.SetManaPct(50);

        sheet.CurrentMp
             .Should()
             .Be(100);
    }

    [Test]
    public void TrySubtractManaPct_ShouldSucceed_WhenSufficient()
    {
        var sheet = CreateSheet(maxMp: 100, currentMp: 100);

        var result = sheet.TrySubtractManaPct(50);

        result.Should()
              .BeTrue();

        sheet.CurrentMp
             .Should()
             .Be(50);
    }

    [Test]
    public void TrySubtractManaPct_ShouldFail_WhenInsufficient()
    {
        var sheet = CreateSheet(maxMp: 100, currentMp: 1);

        var result = sheet.TrySubtractManaPct(50);

        result.Should()
              .BeFalse();
    }
    #endregion

    #region Bonus System
    [Test]
    public void AddBonus_ShouldAffectModFields()
    {
        var sheet = CreateSheet();

        var bonus = new Attributes
        {
            Str = 5,
            Int = 10,
            Wis = 15,
            Con = 20,
            Dex = 25,
            Ac = -3,
            Dmg = 8,
            Hit = 12,
            MaximumHp = 100,
            MaximumMp = 50,
            MagicResistance = 7,
            AtkSpeedPct = 10
        };

        sheet.AddBonus(bonus);

        sheet.StrMod
             .Should()
             .Be(5);

        sheet.IntMod
             .Should()
             .Be(10);

        sheet.WisMod
             .Should()
             .Be(15);

        sheet.ConMod
             .Should()
             .Be(20);

        sheet.DexMod
             .Should()
             .Be(25);

        sheet.AcMod
             .Should()
             .Be(-3);

        sheet.DmgMod
             .Should()
             .Be(8);

        sheet.HitMod
             .Should()
             .Be(12);

        sheet.MaximumHpMod
             .Should()
             .Be(100);

        sheet.MaximumMpMod
             .Should()
             .Be(50);

        sheet.MagicResistanceMod
             .Should()
             .Be(7);

        sheet.AtkSpeedPctMod
             .Should()
             .Be(10);
    }

    [Test]
    public void SubtractBonus_ShouldReverseAddBonus()
    {
        var sheet = CreateSheet();

        var bonus = new Attributes
        {
            Str = 5,
            Int = 10
        };

        sheet.AddBonus(bonus);
        sheet.SubtractBonus(bonus);

        sheet.StrMod
             .Should()
             .Be(0);

        sheet.IntMod
             .Should()
             .Be(0);
    }
    #endregion

    #region Effective Properties
    [Test]
    public void EffectiveStr_ShouldBeBasePlusMod()
    {
        var sheet = new StatSheet
        {
            Str = 10
        };

        var bonus = new Attributes
        {
            Str = 5
        };

        sheet.AddBonus(bonus);

        sheet.EffectiveStr
             .Should()
             .Be(15);
    }

    [Test]
    public void EffectiveStr_ShouldClampToZero_WhenNegative()
    {
        var sheet = new StatSheet
        {
            Str = 3
        };

        var penalty = new Attributes
        {
            Str = 10
        };

        sheet.AddBonus(penalty);
        sheet.SubtractBonus(penalty);
        sheet.SubtractBonus(penalty);

        sheet.EffectiveStr
             .Should()
             .Be(0);
    }

    [Test]
    public void EffectiveMaximumHp_ShouldBeAtLeast1()
    {
        var sheet = new StatSheet
        {
            MaximumHp = 0
        };

        sheet.EffectiveMaximumHp
             .Should()
             .Be(1);
    }

    [Test]
    public void EffectiveMaximumMp_ShouldBeAtLeast1()
    {
        var sheet = new StatSheet
        {
            MaximumMp = 0
        };

        sheet.EffectiveMaximumMp
             .Should()
             .Be(1);
    }

    [Test]
    public void EffectiveAttackSpeedPct_ShouldClampToRange()
    {
        var sheet = new StatSheet
        {
            AtkSpeedPct = 400
        };

        var bonus = new Attributes
        {
            AtkSpeedPct = 200
        };

        sheet.AddBonus(bonus);

        sheet.EffectiveAttackSpeedPct
             .Should()
             .Be(500);
    }

    [Test]
    public void EffectiveAttackSpeedPct_ShouldClampNegativeToMinimum()
    {
        var sheet = new StatSheet
        {
            AtkSpeedPct = -400
        };

        var penalty = new Attributes
        {
            AtkSpeedPct = -200
        };

        sheet.AddBonus(penalty);

        sheet.EffectiveAttackSpeedPct
             .Should()
             .Be(-500);
    }
    #endregion

    #region Health/Mana Percent
    [Test]
    public void HealthPercent_ShouldReturnCorrectPercentage()
    {
        var sheet = CreateSheet(200, currentHp: 100);

        sheet.HealthPercent
             .Should()
             .Be(50);
    }

    [Test]
    public void ManaPercent_ShouldReturnCorrectPercentage()
    {
        var sheet = CreateSheet(maxMp: 200, currentMp: 100);

        sheet.ManaPercent
             .Should()
             .Be(50);
    }
    #endregion

    #region GetBaseStat / GetEffectiveStat
    //formatter:off
    [Test]
    [Arguments(Stat.STR, 10)]
    [Arguments(Stat.DEX, 20)]
    [Arguments(Stat.INT, 30)]
    [Arguments(Stat.WIS, 40)]
    [Arguments(Stat.CON, 50)]

    //formatter:on
    public void GetBaseStat_ShouldReturnCorrectValue(Stat stat, int expected)
    {
        var sheet = new StatSheet
        {
            Str = 10,
            Dex = 20,
            Int = 30,
            Wis = 40,
            Con = 50
        };

        sheet.GetBaseStat(stat)
             .Should()
             .Be(expected);
    }

    [Test]
    public void GetBaseStat_ShouldThrow_ForInvalidStat()
    {
        var sheet = new StatSheet();

        var act = () => sheet.GetBaseStat((Stat)99);

        act.Should()
           .Throw<ArgumentOutOfRangeException>();
    }

    [Test]
    public void GetEffectiveStat_ShouldIncludeMods()
    {
        var sheet = new StatSheet
        {
            Str = 10
        };

        sheet.AddBonus(
            new Attributes
            {
                Str = 5
            });

        sheet.GetEffectiveStat(Stat.STR)
             .Should()
             .Be(15);
    }

    //formatter:off
    [Test]
    [Arguments(Stat.INT)]
    [Arguments(Stat.WIS)]
    [Arguments(Stat.CON)]
    [Arguments(Stat.DEX)]

    //formatter:on
    public void GetEffectiveStat_ShouldReturnCorrectValue_ForEachStat(Stat stat)
    {
        var sheet = new StatSheet
        {
            Int = 10,
            Wis = 20,
            Con = 30,
            Dex = 40
        };

        var bonus = new Attributes
        {
            Int = 5,
            Wis = 5,
            Con = 5,
            Dex = 5
        };

        sheet.AddBonus(bonus);

        var expected = stat switch
        {
            Stat.INT => 15,
            Stat.WIS => 25,
            Stat.CON => 35,
            Stat.DEX => 45,
            _        => throw new ArgumentOutOfRangeException()
        };

        sheet.GetEffectiveStat(stat)
             .Should()
             .Be(expected);
    }

    [Test]
    public void GetEffectiveStat_ShouldThrow_ForInvalidStat()
    {
        var sheet = new StatSheet();

        var act = () => sheet.GetEffectiveStat((Stat)99);

        act.Should()
           .Throw<ArgumentOutOfRangeException>();
    }
    #endregion

    #region Level / Element
    [Test]
    public void AddLevel_ShouldIncrementLevel()
    {
        var sheet = new StatSheet
        {
            Level = 5
        };

        sheet.AddLevel(3);

        sheet.Level
             .Should()
             .Be(8);
    }

    [Test]
    public void SubtractLevel_ShouldDecrementLevel()
    {
        var sheet = new StatSheet
        {
            Level = 5
        };

        sheet.SubtractLevel(2);

        sheet.Level
             .Should()
             .Be(3);
    }

    [Test]
    public void SetLevel_ShouldSetExactLevel()
    {
        var sheet = new StatSheet
        {
            Level = 5
        };

        sheet.SetLevel(99);

        sheet.Level
             .Should()
             .Be(99);
    }

    [Test]
    public void AddAbilityLevel_ShouldIncrement()
    {
        var sheet = new StatSheet
        {
            AbilityLevel = 1
        };

        sheet.AddAbilityLevel(2);

        sheet.AbilityLevel
             .Should()
             .Be(3);
    }

    [Test]
    public void SetDefenseElement_ShouldSetElement()
    {
        var sheet = new StatSheet();

        sheet.SetDefenseElement(Element.Fire);

        sheet.DefenseElement
             .Should()
             .Be(Element.Fire);
    }

    [Test]
    public void SetOffenseElement_ShouldSetElement()
    {
        var sheet = new StatSheet();

        sheet.SetOffenseElement(Element.Water);

        sheet.OffenseElement
             .Should()
             .Be(Element.Water);
    }
    #endregion

    #region EffectiveAc
    [Test]
    public void EffectiveAc_ShouldReturnClampedValue_WhenWithinBounds()
    {
        var sheet = new StatSheet
        {
            Ac = -10
        };

        var bonus = new Attributes
        {
            Ac = 3
        };
        sheet.AddBonus(bonus);

        sheet.EffectiveAc
             .Should()
             .Be(-7);
    }

    [Test]
    public void EffectiveAc_ShouldClampToMinimum_WhenBelowMinMonsterAc()
    {
        // MinimumMonsterAc = -100 (from ServerSetup)
        var sheet = new StatSheet
        {
            Ac = -80
        };

        var bonus = new Attributes
        {
            Ac = -30
        };
        sheet.AddBonus(bonus);

        // Ac + AcMod = -80 + (-30) = -110, clamped to -100
        sheet.EffectiveAc
             .Should()
             .Be(-100);
    }

    [Test]
    public void EffectiveAc_ShouldClampToMaximum_WhenAboveMaxMonsterAc()
    {
        // MaximumMonsterAc = 100 (from ServerSetup)
        var sheet = new StatSheet
        {
            Ac = 80
        };

        var bonus = new Attributes
        {
            Ac = 30
        };
        sheet.AddBonus(bonus);

        // Ac + AcMod = 80 + 30 = 110, clamped to 100
        sheet.EffectiveAc
             .Should()
             .Be(100);
    }
    #endregion

    #region EffectiveDmg / EffectiveHit / EffectiveMagicResistance
    [Test]
    public void EffectiveDmg_ShouldClampToZero_WhenNegative()
    {
        var sheet = new StatSheet
        {
            Dmg = 3
        };

        sheet.AddBonus(
            new Attributes
            {
                Dmg = 5
            });

        sheet.SubtractBonus(
            new Attributes
            {
                Dmg = 5
            });

        sheet.SubtractBonus(
            new Attributes
            {
                Dmg = 5
            });

        // Dmg=3, DmgMod = 5-5-5 = -5, Effective = 3+(-5) = -2 → clamped to 0
        sheet.EffectiveDmg
             .Should()
             .Be(0);
    }

    [Test]
    public void EffectiveHit_ShouldClampToZero_WhenNegative()
    {
        var sheet = new StatSheet
        {
            Hit = 2
        };

        sheet.SubtractBonus(
            new Attributes
            {
                Hit = 10
            });

        sheet.EffectiveHit
             .Should()
             .Be(0);
    }

    [Test]
    public void EffectiveMagicResistance_ShouldClampToZero_WhenNegative()
    {
        var sheet = new StatSheet
        {
            MagicResistance = 5
        };

        sheet.SubtractBonus(
            new Attributes
            {
                MagicResistance = 20
            });

        sheet.EffectiveMagicResistance
             .Should()
             .Be(0);
    }
    #endregion

    #region EffectiveFlatSkillDamage / EffectiveFlatSpellDamage
    [Test]
    public void EffectiveFlatSkillDamage_ShouldReturnBasePlusMod()
    {
        var sheet = new StatSheet
        {
            FlatSkillDamage = 10
        };

        sheet.AddBonus(
            new Attributes
            {
                FlatSkillDamage = 5
            });

        sheet.EffectiveFlatSkillDamage
             .Should()
             .Be(15);
    }

    [Test]
    public void EffectiveFlatSkillDamage_ShouldClampToZero_WhenNegative()
    {
        var sheet = new StatSheet
        {
            FlatSkillDamage = 3
        };

        sheet.SubtractBonus(
            new Attributes
            {
                FlatSkillDamage = 10
            });

        sheet.EffectiveFlatSkillDamage
             .Should()
             .Be(0);
    }

    [Test]
    public void EffectiveFlatSpellDamage_ShouldReturnBasePlusMod()
    {
        var sheet = new StatSheet
        {
            FlatSpellDamage = 20
        };

        sheet.AddBonus(
            new Attributes
            {
                FlatSpellDamage = 10
            });

        sheet.EffectiveFlatSpellDamage
             .Should()
             .Be(30);
    }

    [Test]
    public void EffectiveFlatSpellDamage_ShouldClampToZero_WhenNegative()
    {
        var sheet = new StatSheet
        {
            FlatSpellDamage = 5
        };

        sheet.SubtractBonus(
            new Attributes
            {
                FlatSpellDamage = 20
            });

        sheet.EffectiveFlatSpellDamage
             .Should()
             .Be(0);
    }
    #endregion

    #region EffectiveSkillDamagePct / EffectiveSpellDamagePct
    [Test]
    public void EffectiveSkillDamagePct_ShouldReturnBasePlusMod()
    {
        var sheet = new StatSheet
        {
            SkillDamagePct = 15
        };

        sheet.AddBonus(
            new Attributes
            {
                SkillDamagePct = 10
            });

        sheet.EffectiveSkillDamagePct
             .Should()
             .Be(25);
    }

    [Test]
    public void EffectiveSkillDamagePct_ShouldAllowNegative()
    {
        var sheet = new StatSheet
        {
            SkillDamagePct = 5
        };

        sheet.SubtractBonus(
            new Attributes
            {
                SkillDamagePct = 20
            });

        // No clamping — can be negative
        sheet.EffectiveSkillDamagePct
             .Should()
             .Be(-15);
    }

    [Test]
    public void EffectiveSpellDamagePct_ShouldReturnBasePlusMod()
    {
        var sheet = new StatSheet
        {
            SpellDamagePct = 25
        };

        sheet.AddBonus(
            new Attributes
            {
                SpellDamagePct = 5
            });

        sheet.EffectiveSpellDamagePct
             .Should()
             .Be(30);
    }
    #endregion

    #region EffectiveCon / EffectiveDex / EffectiveInt / EffectiveWis
    [Test]
    public void EffectiveCon_ShouldClampToZero_WhenNegative()
    {
        var sheet = new StatSheet
        {
            Con = 3
        };

        sheet.SubtractBonus(
            new Attributes
            {
                Con = 10
            });

        sheet.EffectiveCon
             .Should()
             .Be(0);
    }

    [Test]
    public void EffectiveDex_ShouldClampToZero_WhenNegative()
    {
        var sheet = new StatSheet
        {
            Dex = 3
        };

        sheet.SubtractBonus(
            new Attributes
            {
                Dex = 10
            });

        sheet.EffectiveDex
             .Should()
             .Be(0);
    }

    [Test]
    public void EffectiveInt_ShouldClampToZero_WhenNegative()
    {
        var sheet = new StatSheet
        {
            Int = 3
        };

        sheet.SubtractBonus(
            new Attributes
            {
                Int = 10
            });

        sheet.EffectiveInt
             .Should()
             .Be(0);
    }

    [Test]
    public void EffectiveWis_ShouldClampToZero_WhenNegative()
    {
        var sheet = new StatSheet
        {
            Wis = 3
        };

        sheet.SubtractBonus(
            new Attributes
            {
                Wis = 10
            });

        sheet.EffectiveWis
             .Should()
             .Be(0);
    }
    #endregion

    #region AddBonus / SubtractBonus — Damage Mod Fields
    [Test]
    public void AddBonus_ShouldAffectDamageModFields()
    {
        var sheet = CreateSheet();

        var bonus = new Attributes
        {
            FlatSkillDamage = 10,
            FlatSpellDamage = 20,
            SkillDamagePct = 30,
            SpellDamagePct = 40
        };

        sheet.AddBonus(bonus);

        sheet.FlatSkillDamageMod
             .Should()
             .Be(10);

        sheet.FlatSpellDamageMod
             .Should()
             .Be(20);

        sheet.SkillDamagePctMod
             .Should()
             .Be(30);

        sheet.SpellDamagePctMod
             .Should()
             .Be(40);
    }

    [Test]
    public void SubtractBonus_ShouldReverseDamageModFields()
    {
        var sheet = CreateSheet();

        var bonus = new Attributes
        {
            FlatSkillDamage = 10,
            FlatSpellDamage = 20,
            SkillDamagePct = 30,
            SpellDamagePct = 40
        };

        sheet.AddBonus(bonus);
        sheet.SubtractBonus(bonus);

        sheet.FlatSkillDamageMod
             .Should()
             .Be(0);

        sheet.FlatSpellDamageMod
             .Should()
             .Be(0);

        sheet.SkillDamagePctMod
             .Should()
             .Be(0);

        sheet.SpellDamagePctMod
             .Should()
             .Be(0);
    }
    #endregion

    #region HealthPercent / ManaPercent clamp
    [Test]
    public void HealthPercent_ShouldClampToZero_WhenCurrentHpIsZero()
    {
        var sheet = CreateSheet(100, currentHp: 0);

        sheet.HealthPercent
             .Should()
             .Be(0);
    }

    [Test]
    public void ManaPercent_ShouldClampToZero_WhenCurrentMpIsZero()
    {
        var sheet = CreateSheet(maxMp: 100, currentMp: 0);

        sheet.ManaPercent
             .Should()
             .Be(0);
    }
    #endregion

    #region CalculateEffectiveAssailInterval
    [Test]
    public void CalculateEffectiveAssailInterval_PositiveModifier_ShouldDecrease()
    {
        var sheet = new StatSheet
        {
            AtkSpeedPct = 100
        };

        // modifier = 100/100 = 1.0, result = 1000 / (1 + 1) = 500
        var result = sheet.CalculateEffectiveAssailInterval(1000);

        result.Should()
              .Be(500);
    }

    [Test]
    public void CalculateEffectiveAssailInterval_NegativeModifier_ShouldIncrease()
    {
        var sheet = new StatSheet
        {
            AtkSpeedPct = -100
        };

        // modifier = -100/100 = -1.0, result = 1000 * |-1 - 1| = 1000 * 2 = 2000
        var result = sheet.CalculateEffectiveAssailInterval(1000);

        result.Should()
              .Be(2000);
    }

    [Test]
    public void CalculateEffectiveAssailInterval_ZeroModifier_ShouldReturnBase()
    {
        var sheet = new StatSheet
        {
            AtkSpeedPct = 0
        };

        var result = sheet.CalculateEffectiveAssailInterval(1000);

        result.Should()
              .Be(1000);
    }
    #endregion
}