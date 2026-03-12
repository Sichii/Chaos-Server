#region
using Chaos.Collections;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Scripting.AislingScripts.Abstractions;
using Chaos.Testing.Infrastructure.Mocks;
using FluentAssertions;
using Moq;
#endregion

namespace Chaos.Tests;

[NotInParallel]
public sealed class TargetFilterExtensionsTests
{
    private readonly MapInstance Map = MockMapInstance.Create();

    #region Composite Relationship Filters
    [Test]
    public void IsValidTarget_CompositeRelationshipFilter_ShouldReturnFalse_WhenAnyRelationshipFlagFails()
    {
        var source = MockAisling.Create(Map, "Source");
        var target = MockMonster.Create(Map, "Target");

        var scriptMock = Mock.Get(source.Script);
        scriptMock.Reset();

        // FriendlyOnly = true, HostileOnly = false => composite fails because HostileOnly fails
        scriptMock.Setup(s => s.IsFriendlyTo(target))
                  .Returns(true);

        scriptMock.Setup(s => s.IsHostileTo(target))
                  .Returns(false);

        var compositeFilter = TargetFilter.FriendlyOnly | TargetFilter.HostileOnly;

        compositeFilter.IsValidTarget(source, target)
                       .Should()
                       .BeFalse();
    }
    #endregion

    #region GroupOnly — target is Monster in group check
    [Test]
    public void IsValidTarget_GroupOnly_ShouldReturnFalse_WhenTargetIsMonsterAndSourceHasGroup()
    {
        var source = MockAisling.Create(Map, "Source");
        var member = MockAisling.Create(Map, "Member");
        var target = MockMonster.Create(Map, "Monster");
        var channelService = MockChannelService.Create();
        var logger = MockLogger.Create<Group>();

        var group = new Group(
            source,
            member,
            channelService,
            logger.Object);
        source.Group = group;

        // Monster is never in an aisling group
        TargetFilter.GroupOnly
                    .IsValidTarget(source, target)
                    .Should()
                    .BeFalse();
    }
    #endregion

    #region None
    [Test]
    public void IsValidTarget_None_ShouldAlwaysReturnTrue()
    {
        var source = MockAisling.Create(Map, "Source");
        var target = MockAisling.Create(Map, "Target");

        TargetFilter.None
                    .IsValidTarget(source, target)
                    .Should()
                    .BeTrue();
    }
    #endregion

    #region Creature Type Filters
    [Test]
    public void IsValidTarget_AislingsOnly_ShouldReturnTrue_ForAisling()
    {
        var source = MockAisling.Create(Map, "Source");
        var target = MockAisling.Create(Map, "Target");

        TargetFilter.AislingsOnly
                    .IsValidTarget(source, target)
                    .Should()
                    .BeTrue();
    }

    [Test]
    public void IsValidTarget_AislingsOnly_ShouldReturnFalse_ForMonster()
    {
        var source = MockAisling.Create(Map, "Source");
        var target = MockMonster.Create(Map, "Monster");

        TargetFilter.AislingsOnly
                    .IsValidTarget(source, target)
                    .Should()
                    .BeFalse();
    }

    [Test]
    public void IsValidTarget_MonstersOnly_ShouldReturnTrue_ForMonster()
    {
        var source = MockAisling.Create(Map, "Source");
        var target = MockMonster.Create(Map, "Monster");

        TargetFilter.MonstersOnly
                    .IsValidTarget(source, target)
                    .Should()
                    .BeTrue();
    }

    [Test]
    public void IsValidTarget_MonstersOnly_ShouldReturnFalse_ForAisling()
    {
        var source = MockAisling.Create(Map, "Source");
        var target = MockAisling.Create(Map, "Target");

        TargetFilter.MonstersOnly
                    .IsValidTarget(source, target)
                    .Should()
                    .BeFalse();
    }

    [Test]
    public void IsValidTarget_MerchantsOnly_ShouldReturnTrue_ForMerchant()
    {
        var source = MockAisling.Create(Map, "Source");
        var target = MockMerchant.Create(Map, "Merchant");

        TargetFilter.MerchantsOnly
                    .IsValidTarget(source, target)
                    .Should()
                    .BeTrue();
    }

    [Test]
    public void IsValidTarget_MerchantsOnly_ShouldReturnFalse_ForAisling()
    {
        var source = MockAisling.Create(Map, "Source");
        var target = MockAisling.Create(Map, "Target");

        TargetFilter.MerchantsOnly
                    .IsValidTarget(source, target)
                    .Should()
                    .BeFalse();
    }

    [Test]
    public void IsValidTarget_NonAislingsOnly_ShouldReturnTrue_ForMonster()
    {
        var source = MockAisling.Create(Map, "Source");
        var target = MockMonster.Create(Map, "Monster");

        TargetFilter.NonAislingsOnly
                    .IsValidTarget(source, target)
                    .Should()
                    .BeTrue();
    }

    [Test]
    public void IsValidTarget_NonAislingsOnly_ShouldReturnFalse_ForAisling()
    {
        var source = MockAisling.Create(Map, "Source");
        var target = MockAisling.Create(Map, "Target");

        TargetFilter.NonAislingsOnly
                    .IsValidTarget(source, target)
                    .Should()
                    .BeFalse();
    }

    [Test]
    public void IsValidTarget_NonMonstersOnly_ShouldReturnTrue_ForAisling()
    {
        var source = MockAisling.Create(Map, "Source");
        var target = MockAisling.Create(Map, "Target");

        TargetFilter.NonMonstersOnly
                    .IsValidTarget(source, target)
                    .Should()
                    .BeTrue();
    }

    [Test]
    public void IsValidTarget_NonMonstersOnly_ShouldReturnFalse_ForMonster()
    {
        var source = MockAisling.Create(Map, "Source");
        var target = MockMonster.Create(Map, "Monster");

        TargetFilter.NonMonstersOnly
                    .IsValidTarget(source, target)
                    .Should()
                    .BeFalse();
    }

    [Test]
    public void IsValidTarget_NonMerchantsOnly_ShouldReturnTrue_ForAisling()
    {
        var source = MockAisling.Create(Map, "Source");
        var target = MockAisling.Create(Map, "Target");

        TargetFilter.NonMerchantsOnly
                    .IsValidTarget(source, target)
                    .Should()
                    .BeTrue();
    }

    [Test]
    public void IsValidTarget_NonMerchantsOnly_ShouldReturnFalse_ForMerchant()
    {
        var source = MockAisling.Create(Map, "Source");
        var target = MockMerchant.Create(Map, "Merchant");

        TargetFilter.NonMerchantsOnly
                    .IsValidTarget(source, target)
                    .Should()
                    .BeFalse();
    }
    #endregion

    #region Self / Others
    [Test]
    public void IsValidTarget_SelfOnly_ShouldReturnTrue_ForSelf()
    {
        var source = MockAisling.Create(Map, "Source");

        TargetFilter.SelfOnly
                    .IsValidTarget(source, source)
                    .Should()
                    .BeTrue();
    }

    [Test]
    public void IsValidTarget_SelfOnly_ShouldReturnFalse_ForOther()
    {
        var source = MockAisling.Create(Map, "Source");
        var other = MockAisling.Create(Map, "Other");

        TargetFilter.SelfOnly
                    .IsValidTarget(source, other)
                    .Should()
                    .BeFalse();
    }

    [Test]
    public void IsValidTarget_OthersOnly_ShouldReturnTrue_ForOther()
    {
        var source = MockAisling.Create(Map, "Source");
        var other = MockAisling.Create(Map, "Other");

        TargetFilter.OthersOnly
                    .IsValidTarget(source, other)
                    .Should()
                    .BeTrue();
    }

    [Test]
    public void IsValidTarget_OthersOnly_ShouldReturnFalse_ForSelf()
    {
        var source = MockAisling.Create(Map, "Source");

        TargetFilter.OthersOnly
                    .IsValidTarget(source, source)
                    .Should()
                    .BeFalse();
    }
    #endregion

    #region Alive / Dead
    [Test]
    public void IsValidTarget_AliveOnly_ShouldReturnTrue_WhenTargetAlive()
    {
        var source = MockAisling.Create(Map, "Source");
        var target = MockAisling.Create(Map, "Target");

        // Aisling is alive by default (CurrentHp > 0)
        target.UserStatSheet.SetHp(100);

        TargetFilter.AliveOnly
                    .IsValidTarget(source, target)
                    .Should()
                    .BeTrue();
    }

    [Test]
    public void IsValidTarget_DeadOnly_ShouldReturnTrue_WhenTargetDead()
    {
        var source = MockAisling.Create(Map, "Source");
        var target = MockAisling.Create(Map, "Target");

        target.IsDead = true;

        TargetFilter.DeadOnly
                    .IsValidTarget(source, target)
                    .Should()
                    .BeTrue();
    }
    #endregion

    #region Composite Flags
    [Test]
    public void IsValidTarget_CompositeFlags_ShouldRequireAllFlagsToMatch()
    {
        var source = MockAisling.Create(Map, "Source");
        var target = MockAisling.Create(Map, "Target");
        target.UserStatSheet.SetHp(100);

        // AislingsOnly | AliveOnly — both must be satisfied
        var filter = TargetFilter.AislingsOnly | TargetFilter.AliveOnly;

        filter.IsValidTarget(source, target)
              .Should()
              .BeTrue();
    }

    [Test]
    public void IsValidTarget_CompositeFlags_ShouldReturnFalse_WhenAnyFlagFails()
    {
        var source = MockAisling.Create(Map, "Source");
        var target = MockMonster.Create(Map, "Monster");

        // AislingsOnly | AliveOnly — Monster fails AislingsOnly
        var filter = TargetFilter.AislingsOnly | TargetFilter.AliveOnly;

        filter.IsValidTarget(source, target)
              .Should()
              .BeFalse();
    }
    #endregion

    #region Relationship Filters
    [Test]
    public void IsValidTarget_FriendlyOnly_ShouldReturnTrue_WhenFriendly()
    {
        var source = MockAisling.Create(Map, "Source");
        var target = MockMonster.Create(Map, "Target");

        var scriptMock = Mock.Get(source.Script);
        scriptMock.Reset();

        scriptMock.Setup(s => s.IsFriendlyTo(target))
                  .Returns(true);

        TargetFilter.FriendlyOnly
                    .IsValidTarget(source, target)
                    .Should()
                    .BeTrue();
    }

    [Test]
    public void IsValidTarget_FriendlyOnly_ShouldReturnFalse_WhenNotFriendly()
    {
        var source = MockAisling.Create(Map, "Source");
        var target = MockMonster.Create(Map, "Target");

        var scriptMock = Mock.Get(source.Script);
        scriptMock.Reset();

        scriptMock.Setup(s => s.IsFriendlyTo(target))
                  .Returns(false);

        TargetFilter.FriendlyOnly
                    .IsValidTarget(source, target)
                    .Should()
                    .BeFalse();
    }

    [Test]
    public void IsValidTarget_HostileOnly_ShouldReturnTrue_WhenHostile()
    {
        var source = MockAisling.Create(Map, "Source");
        var target = MockMonster.Create(Map, "Target");

        var scriptMock = Mock.Get(source.Script);
        scriptMock.Reset();

        scriptMock.Setup(s => s.IsHostileTo(target))
                  .Returns(true);

        TargetFilter.HostileOnly
                    .IsValidTarget(source, target)
                    .Should()
                    .BeTrue();
    }

    [Test]
    public void IsValidTarget_HostileOnly_ShouldReturnFalse_WhenNotHostile()
    {
        var source = MockAisling.Create(Map, "Source");
        var target = MockMonster.Create(Map, "Target");

        var scriptMock = Mock.Get(source.Script);
        scriptMock.Reset();

        scriptMock.Setup(s => s.IsHostileTo(target))
                  .Returns(false);

        TargetFilter.HostileOnly
                    .IsValidTarget(source, target)
                    .Should()
                    .BeFalse();
    }

    [Test]
    public void IsValidTarget_NeutralOnly_ShouldReturnTrue_WhenNeitherFriendlyNorHostile()
    {
        var source = MockAisling.Create(Map, "Source");
        var target = MockMonster.Create(Map, "Target");

        var scriptMock = Mock.Get(source.Script);
        scriptMock.Reset();

        scriptMock.Setup(s => s.IsFriendlyTo(target))
                  .Returns(false);

        scriptMock.Setup(s => s.IsHostileTo(target))
                  .Returns(false);

        TargetFilter.NeutralOnly
                    .IsValidTarget(source, target)
                    .Should()
                    .BeTrue();
    }

    [Test]
    public void IsValidTarget_NeutralOnly_ShouldReturnFalse_WhenFriendly()
    {
        var source = MockAisling.Create(Map, "Source");
        var target = MockMonster.Create(Map, "Target");

        var scriptMock = Mock.Get(source.Script);
        scriptMock.Reset();

        scriptMock.Setup(s => s.IsFriendlyTo(target))
                  .Returns(true);

        scriptMock.Setup(s => s.IsHostileTo(target))
                  .Returns(false);

        TargetFilter.NeutralOnly
                    .IsValidTarget(source, target)
                    .Should()
                    .BeFalse();
    }

    [Test]
    public void IsValidTarget_NeutralOnly_ShouldReturnFalse_WhenHostile()
    {
        var source = MockAisling.Create(Map, "Source");
        var target = MockMonster.Create(Map, "Target");

        var scriptMock = Mock.Get(source.Script);
        scriptMock.Reset();

        scriptMock.Setup(s => s.IsFriendlyTo(target))
                  .Returns(false);

        scriptMock.Setup(s => s.IsHostileTo(target))
                  .Returns(true);

        TargetFilter.NeutralOnly
                    .IsValidTarget(source, target)
                    .Should()
                    .BeFalse();
    }

    [Test]
    public void IsValidTarget_NonFriendlyOnly_ShouldReturnTrue_WhenNotFriendly()
    {
        var source = MockAisling.Create(Map, "Source");
        var target = MockMonster.Create(Map, "Target");

        var scriptMock = Mock.Get(source.Script);
        scriptMock.Reset();

        scriptMock.Setup(s => s.IsFriendlyTo(target))
                  .Returns(false);

        TargetFilter.NonFriendlyOnly
                    .IsValidTarget(source, target)
                    .Should()
                    .BeTrue();
    }

    [Test]
    public void IsValidTarget_NonFriendlyOnly_ShouldReturnFalse_WhenFriendly()
    {
        var source = MockAisling.Create(Map, "Source");
        var target = MockMonster.Create(Map, "Target");

        var scriptMock = Mock.Get(source.Script);
        scriptMock.Reset();

        scriptMock.Setup(s => s.IsFriendlyTo(target))
                  .Returns(true);

        TargetFilter.NonFriendlyOnly
                    .IsValidTarget(source, target)
                    .Should()
                    .BeFalse();
    }

    [Test]
    public void IsValidTarget_NonHostileOnly_ShouldReturnTrue_WhenNotHostile()
    {
        var source = MockAisling.Create(Map, "Source");
        var target = MockMonster.Create(Map, "Target");

        var scriptMock = Mock.Get(source.Script);
        scriptMock.Reset();

        scriptMock.Setup(s => s.IsHostileTo(target))
                  .Returns(false);

        TargetFilter.NonHostileOnly
                    .IsValidTarget(source, target)
                    .Should()
                    .BeTrue();
    }

    [Test]
    public void IsValidTarget_NonHostileOnly_ShouldReturnFalse_WhenHostile()
    {
        var source = MockAisling.Create(Map, "Source");
        var target = MockMonster.Create(Map, "Target");

        var scriptMock = Mock.Get(source.Script);
        scriptMock.Reset();

        scriptMock.Setup(s => s.IsHostileTo(target))
                  .Returns(true);

        TargetFilter.NonHostileOnly
                    .IsValidTarget(source, target)
                    .Should()
                    .BeFalse();
    }

    [Test]
    public void IsValidTarget_NonNeutralOnly_ShouldReturnTrue_WhenFriendly()
    {
        var source = MockAisling.Create(Map, "Source");
        var target = MockMonster.Create(Map, "Target");

        var scriptMock = Mock.Get(source.Script);
        scriptMock.Reset();

        scriptMock.Setup(s => s.IsFriendlyTo(target))
                  .Returns(true);

        scriptMock.Setup(s => s.IsHostileTo(target))
                  .Returns(false);

        TargetFilter.NonNeutralOnly
                    .IsValidTarget(source, target)
                    .Should()
                    .BeTrue();
    }

    [Test]
    public void IsValidTarget_NonNeutralOnly_ShouldReturnTrue_WhenHostile()
    {
        var source = MockAisling.Create(Map, "Source");
        var target = MockMonster.Create(Map, "Target");

        var scriptMock = Mock.Get(source.Script);
        scriptMock.Reset();

        scriptMock.Setup(s => s.IsFriendlyTo(target))
                  .Returns(false);

        scriptMock.Setup(s => s.IsHostileTo(target))
                  .Returns(true);

        TargetFilter.NonNeutralOnly
                    .IsValidTarget(source, target)
                    .Should()
                    .BeTrue();
    }

    [Test]
    public void IsValidTarget_NonNeutralOnly_ShouldReturnFalse_WhenNeitherFriendlyNorHostile()
    {
        var source = MockAisling.Create(Map, "Source");
        var target = MockMonster.Create(Map, "Target");

        var scriptMock = Mock.Get(source.Script);
        scriptMock.Reset();

        scriptMock.Setup(s => s.IsFriendlyTo(target))
                  .Returns(false);

        scriptMock.Setup(s => s.IsHostileTo(target))
                  .Returns(false);

        TargetFilter.NonNeutralOnly
                    .IsValidTarget(source, target)
                    .Should()
                    .BeFalse();
    }
    #endregion

    #region GroupOnly
    [Test]
    public void IsValidTarget_GroupOnly_ShouldReturnTrue_ForSelf()
    {
        var source = MockAisling.Create(Map, "Source");

        TargetFilter.GroupOnly
                    .IsValidTarget(source, source)
                    .Should()
                    .BeTrue();
    }

    [Test]
    public void IsValidTarget_GroupOnly_ShouldReturnTrue_WhenTargetInGroup()
    {
        var source = MockAisling.Create(Map, "Source");
        var target = MockAisling.Create(Map, "Target");
        var channelService = MockChannelService.Create();
        var logger = MockLogger.Create<Group>();

        var group = new Group(
            source,
            target,
            channelService,
            logger.Object);
        source.Group = group;
        target.Group = group;

        TargetFilter.GroupOnly
                    .IsValidTarget(source, target)
                    .Should()
                    .BeTrue();
    }

    [Test]
    public void IsValidTarget_GroupOnly_ShouldReturnFalse_WhenSourceIsNotAisling()
    {
        var source = MockMonster.Create(Map, "Source");
        var target = MockAisling.Create(Map, "Target");

        TargetFilter.GroupOnly
                    .IsValidTarget(source, target)
                    .Should()
                    .BeFalse();
    }

    [Test]
    public void IsValidTarget_GroupOnly_ShouldReturnFalse_WhenSourceHasNoGroup()
    {
        var source = MockAisling.Create(Map, "Source");
        var target = MockAisling.Create(Map, "Target");

        // source.Group is null by default
        TargetFilter.GroupOnly
                    .IsValidTarget(source, target)
                    .Should()
                    .BeFalse();
    }

    [Test]
    public void IsValidTarget_GroupOnly_ShouldReturnFalse_WhenTargetNotInSourceGroup()
    {
        var source = MockAisling.Create(Map, "Source");
        var groupMember = MockAisling.Create(Map, "GroupMember");
        var target = MockAisling.Create(Map, "Target");
        var channelService = MockChannelService.Create();
        var logger = MockLogger.Create<Group>();

        // Source has a group, but target is NOT in it
        var group = new Group(
            source,
            groupMember,
            channelService,
            logger.Object);
        source.Group = group;
        groupMember.Group = group;

        TargetFilter.GroupOnly
                    .IsValidTarget(source, target)
                    .Should()
                    .BeFalse();
    }
    #endregion

    #region NeutralOnly / NonNeutralOnly — both conditions evaluated
    [Test]
    public void IsValidTarget_NeutralOnly_ShouldReturnTrue_WhenBothFriendlyAndHostileFalse()
    {
        // Exercises full AND evaluation: !false && !false → true
        var source = MockAisling.Create(Map, "Source");
        var target = MockAisling.Create(Map, "Target");

        var scriptMock = Mock.Get(source.Script);
        scriptMock.Reset();

        scriptMock.Setup(s => s.IsFriendlyTo(target))
                  .Returns(false);

        scriptMock.Setup(s => s.IsHostileTo(target))
                  .Returns(false);

        TargetFilter.NeutralOnly
                    .IsValidTarget(source, target)
                    .Should()
                    .BeTrue();
    }

    [Test]
    public void IsValidTarget_NonNeutralOnly_ShouldReturnFalse_WhenBothFalse_MonsterSource()
    {
        // Monster source exercises the switch case differently
        var source = MockMonster.Create(Map, "Source");
        var target = MockAisling.Create(Map, "Target");

        var scriptMock = Mock.Get(source.Script);
        scriptMock.Reset();

        scriptMock.Setup(s => s.IsFriendlyTo(target))
                  .Returns(false);

        scriptMock.Setup(s => s.IsHostileTo(target))
                  .Returns(false);

        TargetFilter.NonNeutralOnly
                    .IsValidTarget(source, target)
                    .Should()
                    .BeFalse();
    }
    #endregion

    #region AliveOnly / DeadOnly False Cases
    [Test]
    public void IsValidTarget_AliveOnly_ShouldReturnFalse_WhenTargetDead()
    {
        var source = MockAisling.Create(Map, "Source");
        var target = MockAisling.Create(Map, "Target");

        // CurrentHp defaults to 0 for Aisling stat sheet, so IsAlive is false
        TargetFilter.AliveOnly
                    .IsValidTarget(source, target)
                    .Should()
                    .BeFalse();
    }

    [Test]
    public void IsValidTarget_DeadOnly_ShouldReturnFalse_WhenTargetAlive()
    {
        var source = MockAisling.Create(Map, "Source");
        var target = MockAisling.Create(Map, "Target");

        // IsDead is false by default
        target.UserStatSheet.SetHp(100);

        TargetFilter.DeadOnly
                    .IsValidTarget(source, target)
                    .Should()
                    .BeFalse();
    }
    #endregion
}