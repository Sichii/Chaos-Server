#region
using Chaos.Models.Data;
using FluentAssertions;
#endregion

namespace Chaos.Tests;

public sealed class AttributesTests
{
    #region Default Values
    [Test]
    public void DefaultValues_ShouldAllBeZero()
    {
        var attrs = new Attributes();

        attrs.Str
             .Should()
             .Be(0);

        attrs.Int
             .Should()
             .Be(0);

        attrs.Wis
             .Should()
             .Be(0);

        attrs.Con
             .Should()
             .Be(0);

        attrs.Dex
             .Should()
             .Be(0);

        attrs.Ac
             .Should()
             .Be(0);

        attrs.Dmg
             .Should()
             .Be(0);

        attrs.Hit
             .Should()
             .Be(0);

        attrs.MaximumHp
             .Should()
             .Be(0);

        attrs.MaximumMp
             .Should()
             .Be(0);

        attrs.MagicResistance
             .Should()
             .Be(0);

        attrs.AtkSpeedPct
             .Should()
             .Be(0);

        attrs.SkillDamagePct
             .Should()
             .Be(0);

        attrs.SpellDamagePct
             .Should()
             .Be(0);

        attrs.FlatSkillDamage
             .Should()
             .Be(0);

        attrs.FlatSpellDamage
             .Should()
             .Be(0);
    }

    [Test]
    public void InitProperties_ShouldSetCorrectly()
    {
        var attrs = new Attributes
        {
            Str = 10,
            Int = 20,
            Wis = 30,
            Con = 40,
            Dex = 50,
            Ac = 5,
            Dmg = 15,
            Hit = 25,
            MaximumHp = 1000,
            MaximumMp = 500,
            MagicResistance = 35,
            AtkSpeedPct = 10
        };

        attrs.Str
             .Should()
             .Be(10);

        attrs.Int
             .Should()
             .Be(20);

        attrs.Wis
             .Should()
             .Be(30);

        attrs.Con
             .Should()
             .Be(40);

        attrs.Dex
             .Should()
             .Be(50);

        attrs.Ac
             .Should()
             .Be(5);

        attrs.Dmg
             .Should()
             .Be(15);

        attrs.Hit
             .Should()
             .Be(25);

        attrs.MaximumHp
             .Should()
             .Be(1000);

        attrs.MaximumMp
             .Should()
             .Be(500);

        attrs.MagicResistance
             .Should()
             .Be(35);

        attrs.AtkSpeedPct
             .Should()
             .Be(10);
    }
    #endregion

    #region Add
    [Test]
    public void Add_ShouldAccumulateAllFields()
    {
        var a = new Attributes
        {
            Str = 1,
            Int = 2,
            Wis = 3,
            Con = 4,
            Dex = 5,
            Ac = 6,
            Dmg = 7,
            Hit = 8,
            MaximumHp = 9,
            MaximumMp = 10,
            MagicResistance = 11,
            AtkSpeedPct = 12,
            SkillDamagePct = 13,
            SpellDamagePct = 14,
            FlatSkillDamage = 15,
            FlatSpellDamage = 16
        };

        var b = new Attributes
        {
            Str = 10,
            Int = 20,
            Wis = 30,
            Con = 40,
            Dex = 50,
            Ac = 60,
            Dmg = 70,
            Hit = 80,
            MaximumHp = 90,
            MaximumMp = 100,
            MagicResistance = 110,
            AtkSpeedPct = 120,
            SkillDamagePct = 130,
            SpellDamagePct = 140,
            FlatSkillDamage = 150,
            FlatSpellDamage = 160
        };

        a.Add(b);

        a.Str
         .Should()
         .Be(11);

        a.Int
         .Should()
         .Be(22);

        a.Wis
         .Should()
         .Be(33);

        a.Con
         .Should()
         .Be(44);

        a.Dex
         .Should()
         .Be(55);

        a.Ac
         .Should()
         .Be(66);

        a.Dmg
         .Should()
         .Be(77);

        a.Hit
         .Should()
         .Be(88);

        a.MaximumHp
         .Should()
         .Be(99);

        a.MaximumMp
         .Should()
         .Be(110);

        a.MagicResistance
         .Should()
         .Be(121);

        a.AtkSpeedPct
         .Should()
         .Be(132);

        a.SkillDamagePct
         .Should()
         .Be(143);

        a.SpellDamagePct
         .Should()
         .Be(154);

        a.FlatSkillDamage
         .Should()
         .Be(165);

        a.FlatSpellDamage
         .Should()
         .Be(176);
    }

    [Test]
    public void Add_ShouldHandleNegativeValues()
    {
        var a = new Attributes
        {
            Str = 10,
            Int = 20
        };

        var b = new Attributes
        {
            Str = -5,
            Int = -25
        };

        a.Add(b);

        a.Str
         .Should()
         .Be(5);

        a.Int
         .Should()
         .Be(-5);
    }

    [Test]
    public void Add_WithZeroAttributes_ShouldNotChange()
    {
        var a = new Attributes
        {
            Str = 10,
            Int = 20,
            MaximumHp = 100
        };
        var zero = new Attributes();

        a.Add(zero);

        a.Str
         .Should()
         .Be(10);

        a.Int
         .Should()
         .Be(20);

        a.MaximumHp
         .Should()
         .Be(100);
    }
    #endregion

    #region Subtract
    [Test]
    public void Subtract_ShouldSubtractAllFields()
    {
        var a = new Attributes
        {
            Str = 100,
            Int = 200,
            Wis = 300,
            Con = 400,
            Dex = 500,
            Ac = 60,
            Dmg = 70,
            Hit = 80,
            MaximumHp = 900,
            MaximumMp = 1000,
            MagicResistance = 110,
            AtkSpeedPct = 120,
            SkillDamagePct = 130,
            SpellDamagePct = 140,
            FlatSkillDamage = 150,
            FlatSpellDamage = 160
        };

        var b = new Attributes
        {
            Str = 10,
            Int = 20,
            Wis = 30,
            Con = 40,
            Dex = 50,
            Ac = 6,
            Dmg = 7,
            Hit = 8,
            MaximumHp = 90,
            MaximumMp = 100,
            MagicResistance = 11,
            AtkSpeedPct = 12,
            SkillDamagePct = 13,
            SpellDamagePct = 14,
            FlatSkillDamage = 15,
            FlatSpellDamage = 16
        };

        a.Subtract(b);

        a.Str
         .Should()
         .Be(90);

        a.Int
         .Should()
         .Be(180);

        a.Wis
         .Should()
         .Be(270);

        a.Con
         .Should()
         .Be(360);

        a.Dex
         .Should()
         .Be(450);

        a.Ac
         .Should()
         .Be(54);

        a.Dmg
         .Should()
         .Be(63);

        a.Hit
         .Should()
         .Be(72);

        a.MaximumHp
         .Should()
         .Be(810);

        a.MaximumMp
         .Should()
         .Be(900);

        a.MagicResistance
         .Should()
         .Be(99);

        a.AtkSpeedPct
         .Should()
         .Be(108);

        a.SkillDamagePct
         .Should()
         .Be(117);

        a.SpellDamagePct
         .Should()
         .Be(126);

        a.FlatSkillDamage
         .Should()
         .Be(135);

        a.FlatSpellDamage
         .Should()
         .Be(144);
    }

    [Test]
    public void Subtract_ShouldAllowNegativeResults()
    {
        var a = new Attributes
        {
            Str = 5
        };

        var b = new Attributes
        {
            Str = 10
        };

        a.Subtract(b);

        a.Str
         .Should()
         .Be(-5);
    }

    [Test]
    public void Subtract_FromSelf_ShouldBeZero()
    {
        var a = new Attributes
        {
            Str = 10,
            Int = 20,
            Wis = 30,
            Con = 40,
            Dex = 50,
            Ac = 6,
            Dmg = 7,
            Hit = 8,
            MaximumHp = 90,
            MaximumMp = 100,
            MagicResistance = 11,
            AtkSpeedPct = 12,
            SkillDamagePct = 13,
            SpellDamagePct = 14,
            FlatSkillDamage = 15,
            FlatSpellDamage = 16
        };

        // Create equivalent attributes to subtract
        var b = new Attributes
        {
            Str = 10,
            Int = 20,
            Wis = 30,
            Con = 40,
            Dex = 50,
            Ac = 6,
            Dmg = 7,
            Hit = 8,
            MaximumHp = 90,
            MaximumMp = 100,
            MagicResistance = 11,
            AtkSpeedPct = 12,
            SkillDamagePct = 13,
            SpellDamagePct = 14,
            FlatSkillDamage = 15,
            FlatSpellDamage = 16
        };

        a.Subtract(b);

        a.Str
         .Should()
         .Be(0);

        a.Int
         .Should()
         .Be(0);

        a.Wis
         .Should()
         .Be(0);

        a.Con
         .Should()
         .Be(0);

        a.Dex
         .Should()
         .Be(0);

        a.Ac
         .Should()
         .Be(0);

        a.Dmg
         .Should()
         .Be(0);

        a.Hit
         .Should()
         .Be(0);

        a.MaximumHp
         .Should()
         .Be(0);

        a.MaximumMp
         .Should()
         .Be(0);

        a.MagicResistance
         .Should()
         .Be(0);

        a.AtkSpeedPct
         .Should()
         .Be(0);

        a.SkillDamagePct
         .Should()
         .Be(0);

        a.SpellDamagePct
         .Should()
         .Be(0);

        a.FlatSkillDamage
         .Should()
         .Be(0);

        a.FlatSpellDamage
         .Should()
         .Be(0);
    }
    #endregion
}