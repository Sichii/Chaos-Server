#region
using Chaos.DarkAges.Definitions;
using Chaos.Models.Data;
using FluentAssertions;
#endregion

namespace Chaos.Tests;

public sealed class UserStatSheetTests
{
    #region NewCharacter
    [Test]
    public void NewCharacter_ShouldHaveExpectedDefaults()
    {
        var sheet = UserStatSheet.NewCharacter;

        sheet.Ac
             .Should()
             .Be(100);

        sheet.MaxWeight
             .Should()
             .Be(40);

        sheet.ToNextLevel
             .Should()
             .Be(100);

        sheet.Str
             .Should()
             .Be(1);

        sheet.Int
             .Should()
             .Be(1);

        sheet.Wis
             .Should()
             .Be(1);

        sheet.Con
             .Should()
             .Be(1);

        sheet.Dex
             .Should()
             .Be(1);

        sheet.CurrentHp
             .Should()
             .Be(100);

        sheet.MaximumHp
             .Should()
             .Be(100);

        sheet.CurrentMp
             .Should()
             .Be(50);

        sheet.MaximumMp
             .Should()
             .Be(50);

        sheet.Level
             .Should()
             .Be(1);

        sheet.Master
             .Should()
             .BeFalse();

        sheet.BaseClass
             .Should()
             .Be(BaseClass.Peasant);

        sheet.AdvClass
             .Should()
             .Be(AdvClass.None);
    }
    #endregion

    #region SubtractTotalAbility normal path
    [Test]
    public void SubtractTotalAbility_ShouldSubtractNormally_WhenResultIsPositive()
    {
        var sheet = new UserStatSheet
        {
            TotalAbility = 100
        };

        sheet.SubtractTotalAbility(30);

        sheet.TotalAbility
             .Should()
             .Be(70);
    }
    #endregion

    #region TotalExp
    [Test]
    public void AddTotalExp_ShouldAddNormally()
    {
        var sheet = new UserStatSheet
        {
            TotalExp = 100
        };

        sheet.AddTotalExp(50);

        sheet.TotalExp
             .Should()
             .Be(150);
    }

    [Test]
    public void AddTotalExp_ShouldCapAtUintMaxValue()
    {
        var sheet = new UserStatSheet
        {
            TotalExp = uint.MaxValue - 10
        };

        sheet.AddTotalExp(100);

        sheet.TotalExp
             .Should()
             .Be(uint.MaxValue);
    }

    [Test]
    public void SubtractTotalExp_ShouldSubtractNormally()
    {
        var sheet = new UserStatSheet
        {
            TotalExp = 100
        };

        sheet.SubtractTotalExp(50);

        sheet.TotalExp
             .Should()
             .Be(50);
    }

    [Test]
    public void SubtractTotalExp_ShouldFloorAtZero()
    {
        var sheet = new UserStatSheet
        {
            TotalExp = 10
        };

        sheet.SubtractTotalExp(100);

        sheet.TotalExp
             .Should()
             .Be(0);
    }

    [Test]
    public void TrySubtractTotalExp_ShouldSucceed_WhenSufficient()
    {
        var sheet = new UserStatSheet
        {
            TotalExp = 100
        };

        var result = sheet.TrySubtractTotalExp(50);

        result.Should()
              .BeTrue();

        sheet.TotalExp
             .Should()
             .Be(50);
    }

    [Test]
    public void TrySubtractTotalExp_ShouldFail_WhenInsufficient()
    {
        var sheet = new UserStatSheet
        {
            TotalExp = 10
        };

        var result = sheet.TrySubtractTotalExp(50);

        result.Should()
              .BeFalse();

        sheet.TotalExp
             .Should()
             .Be(10);
    }
    #endregion

    #region TotalAbility
    [Test]
    public void AddTotalAbility_ShouldAddNormally()
    {
        var sheet = new UserStatSheet
        {
            TotalAbility = 100
        };

        sheet.AddTotalAbility(50);

        sheet.TotalAbility
             .Should()
             .Be(150);
    }

    [Test]
    public void AddTotalAbility_ShouldCapAtUintMaxValue()
    {
        var sheet = new UserStatSheet
        {
            TotalAbility = uint.MaxValue - 10
        };

        sheet.AddTotalAbility(100);

        sheet.TotalAbility
             .Should()
             .Be(uint.MaxValue);
    }

    [Test]
    public void SubtractTotalAbility_ShouldFloorAtZero()
    {
        var sheet = new UserStatSheet
        {
            TotalAbility = 10
        };

        sheet.SubtractTotalAbility(100);

        sheet.TotalAbility
             .Should()
             .Be(0);
    }

    [Test]
    public void TrySubtractTotalAbility_ShouldSucceed_WhenSufficient()
    {
        var sheet = new UserStatSheet
        {
            TotalAbility = 100
        };

        var result = sheet.TrySubtractTotalAbility(50);

        result.Should()
              .BeTrue();

        sheet.TotalAbility
             .Should()
             .Be(50);
    }

    [Test]
    public void TrySubtractTotalAbility_ShouldFail_WhenInsufficient()
    {
        var sheet = new UserStatSheet
        {
            TotalAbility = 10
        };

        var result = sheet.TrySubtractTotalAbility(50);

        result.Should()
              .BeFalse();

        sheet.TotalAbility
             .Should()
             .Be(10);
    }
    #endregion

    #region Tnl / Tna
    [Test]
    public void AddTnl_ShouldAddNormally()
    {
        var sheet = new UserStatSheet
        {
            ToNextLevel = 100
        };

        sheet.AddTnl(50);

        sheet.ToNextLevel
             .Should()
             .Be(150);
    }

    [Test]
    public void AddTnl_ShouldCapAtUintMaxValue()
    {
        var sheet = new UserStatSheet
        {
            ToNextLevel = uint.MaxValue - 10
        };

        sheet.AddTnl(100);

        sheet.ToNextLevel
             .Should()
             .Be(uint.MaxValue);
    }

    [Test]
    public void SubtractTnl_ShouldFloorAtZero()
    {
        var sheet = new UserStatSheet
        {
            ToNextLevel = 10
        };

        sheet.SubtractTnl(100);

        sheet.ToNextLevel
             .Should()
             .Be(0);
    }

    [Test]
    public void AddTna_ShouldAddNormally()
    {
        var sheet = new UserStatSheet
        {
            ToNextAbility = 100
        };

        sheet.AddTna(50);

        sheet.ToNextAbility
             .Should()
             .Be(150);
    }

    [Test]
    public void AddTna_ShouldCapAtUintMaxValue()
    {
        var sheet = new UserStatSheet
        {
            ToNextAbility = uint.MaxValue - 10
        };

        sheet.AddTna(100);

        sheet.ToNextAbility
             .Should()
             .Be(uint.MaxValue);
    }

    [Test]
    public void SubtractTna_ShouldFloorAtZero()
    {
        var sheet = new UserStatSheet
        {
            ToNextAbility = 10
        };

        sheet.SubtractTna(100);

        sheet.ToNextAbility
             .Should()
             .Be(0);
    }
    #endregion

    #region EffectiveAc
    [Test]
    public void EffectiveAc_ShouldReturnClampedValue_WhenWithinBounds()
    {
        var sheet = new UserStatSheet
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
    public void EffectiveAc_ShouldClampToMinimum_WhenBelowMinAislingAc()
    {
        // MinimumAislingAc = -100 (from ServerSetup)
        var sheet = new UserStatSheet
        {
            Ac = -90
        };

        sheet.AddBonus(
            new Attributes
            {
                Ac = -20
            });

        // Ac + AcMod = -90 + (-20) = -110, clamped to -100
        sheet.EffectiveAc
             .Should()
             .Be(-100);
    }

    [Test]
    public void EffectiveAc_ShouldClampToMaximum_WhenAboveMaxAislingAc()
    {
        // MaximumAislingAc = 100 (from ServerSetup)
        var sheet = new UserStatSheet
        {
            Ac = 90
        };

        sheet.AddBonus(
            new Attributes
            {
                Ac = 20
            });

        // Ac + AcMod = 90 + 20 = 110, clamped to 100
        sheet.EffectiveAc
             .Should()
             .Be(100);
    }
    #endregion

    #region SubtractTnl / SubtractTna normal paths
    [Test]
    public void SubtractTnl_ShouldReturnRemainder_WhenResultIsPositive()
    {
        var sheet = new UserStatSheet
        {
            ToNextLevel = 100
        };

        var result = sheet.SubtractTnl(30);

        result.Should()
              .Be(70);

        sheet.ToNextLevel
             .Should()
             .Be(70);
    }

    [Test]
    public void SubtractTna_ShouldReturnRemainder_WhenResultIsPositive()
    {
        var sheet = new UserStatSheet
        {
            ToNextAbility = 200
        };

        var result = sheet.SubtractTna(50);

        result.Should()
              .Be(150);

        sheet.ToNextAbility
             .Should()
             .Be(150);
    }
    #endregion

    #region IncrementStat
    //formatter:off
    [Test]
    [Arguments(Stat.STR)]
    [Arguments(Stat.DEX)]
    [Arguments(Stat.INT)]
    [Arguments(Stat.WIS)]
    [Arguments(Stat.CON)]

    //formatter:on
    public void IncrementStat_ShouldSucceed_WithUnspentPoints(Stat stat)
    {
        var sheet = new UserStatSheet
        {
            UnspentPoints = 3
        };

        var result = sheet.IncrementStat(stat);

        result.Should()
              .BeTrue();

        sheet.UnspentPoints
             .Should()
             .Be(2);

        sheet.GetBaseStat(stat)
             .Should()
             .Be(1);
    }

    [Test]
    public void IncrementStat_ShouldFail_WhenNoUnspentPoints()
    {
        var sheet = new UserStatSheet
        {
            UnspentPoints = 0
        };

        var result = sheet.IncrementStat(Stat.STR);

        result.Should()
              .BeFalse();

        sheet.Str
             .Should()
             .Be(0);
    }

    [Test]
    public void IncrementStat_ShouldThrow_ForInvalidStat()
    {
        var sheet = new UserStatSheet
        {
            UnspentPoints = 1
        };

        var act = () => sheet.IncrementStat((Stat)99);

        act.Should()
           .Throw<ArgumentOutOfRangeException>();
    }
    #endregion

    #region GivePoints / AddWeight
    [Test]
    public void GivePoints_ShouldIncrementUnspentPoints()
    {
        var sheet = new UserStatSheet
        {
            UnspentPoints = 0
        };

        sheet.GivePoints(5);

        sheet.UnspentPoints
             .Should()
             .Be(5);
    }

    [Test]
    public void AddWeight_ShouldIncrementCurrentWeight()
    {
        var sheet = new UserStatSheet
        {
            CurrentWeight = 10
        };

        sheet.AddWeight(5);

        sheet.CurrentWeight
             .Should()
             .Be(15);
    }
    #endregion

    #region Setters
    [Test]
    public void SetBaseClass_ShouldSetClass()
    {
        var sheet = new UserStatSheet();

        sheet.SetBaseClass(BaseClass.Warrior);

        sheet.BaseClass
             .Should()
             .Be(BaseClass.Warrior);
    }

    [Test]
    public void SetAdvClass_ShouldSetClass()
    {
        var sheet = new UserStatSheet();

        sheet.SetAdvClass(AdvClass.Gladiator);

        sheet.AdvClass
             .Should()
             .Be(AdvClass.Gladiator);
    }

    [Test]
    public void SetIsMaster_ShouldSetMasterFlag()
    {
        var sheet = new UserStatSheet();

        sheet.SetIsMaster(true);

        sheet.Master
             .Should()
             .BeTrue();
    }

    [Test]
    public void SetMaxWeight_ShouldSetMaxWeight()
    {
        var sheet = new UserStatSheet();

        sheet.SetMaxWeight(100);

        sheet.MaxWeight
             .Should()
             .Be(100);
    }
    #endregion
}