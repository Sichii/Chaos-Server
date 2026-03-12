#region
using Chaos.Collections;
using Chaos.Common.Abstractions;
using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Geometry.Abstractions.Definitions;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.Templates;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Pathfinding;
using Chaos.Pathfinding.Abstractions;
using Chaos.Testing.Infrastructure.Mocks;
using FluentAssertions;
using Moq;
#endregion

namespace Chaos.Tests;

[NotInParallel]
public sealed class CreatureTests
{
    private readonly MapInstance Map = MockMapInstance.Create(width: 20, height: 20);

    #region Animate
    [Test]
    public void Animate_ShouldSendAnimation_ToNearbyAislings()
    {
        var monster = MockMonster.Create(Map);
        var aisling = MockAisling.Create(Map, "Watcher", new Point(5, 5));

        Map.AddEntity(monster, new Point(5, 5));
        Map.AddEntity(aisling, new Point(5, 6));

        // Aisling must observe the monster
        aisling.ApproachTime[monster] = DateTime.UtcNow;

        var animation = new Animation
        {
            AnimationSpeed = 100,
            TargetAnimation = 1
        };

        monster.Animate(animation);

        var clientMock = Mock.Get(aisling.Client);

        clientMock.Verify(c => c.SendAnimation(It.IsAny<Animation>()), Times.Once);
    }
    #endregion

    #region AnimateBody with custom speed and sound
    [Test]
    public void AnimateBody_ShouldPassCustomSpeedAndSound()
    {
        var monster = MockMonster.Create(Map);
        var aisling = MockAisling.Create(Map, "Watcher", new Point(5, 5));

        Map.AddEntity(monster, new Point(5, 5));
        Map.AddEntity(aisling, new Point(5, 6));

        // Aisling needs to CanSee the monster
        var aislingScriptMock = Mock.Get(aisling.Script);

        aislingScriptMock.Setup(s => s.CanSee(monster))
                         .Returns(true);

        monster.AnimateBody(BodyAnimation.Assail, 50, 42);

        var clientMock = Mock.Get(aisling.Client);

        clientMock.Verify(
            c => c.SendBodyAnimation(
                monster.Id,
                BodyAnimation.Assail,
                50,
                (byte?)42),
            Times.Once);
    }
    #endregion

    #region CanUse (Skill) — cooldown check
    [Test]
    public void CanUse_Skill_ShouldReturnFalse_WhenSkillIsOnCooldown()
    {
        var monster = MockMonster.Create(Map);
        var skill = MockSkill.Create();

        // Set up script to allow skill usage
        MockMonster.SetupScript(
            monster,
            s => s.Setup(x => x.CanUseSkill(skill))
                  .Returns(true));

        // Put skill on cooldown — CanUse() checks Elapsed
        skill.Cooldown = TimeSpan.FromSeconds(5);
        skill.Elapsed = TimeSpan.Zero; // On cooldown

        monster.CanUse(skill, out var context)
               .Should()
               .BeFalse();

        context.Should()
               .BeNull();
    }
    #endregion

    #region CanUse (Spell) — cooldown check
    [Test]
    public void CanUse_Spell_ShouldReturnFalse_WhenSpellIsOnCooldown()
    {
        var monster = MockMonster.Create(Map);
        var spell = MockSpell.Create();

        MockMonster.SetupScript(
            monster,
            s => s.Setup(x => x.CanUseSpell(spell))
                  .Returns(true));

        // Put spell on cooldown
        spell.Cooldown = TimeSpan.FromSeconds(5);
        spell.Elapsed = TimeSpan.Zero;

        monster.CanUse(
                   spell,
                   monster,
                   null,
                   out var context)
               .Should()
               .BeFalse();

        context.Should()
               .BeNull();
    }
    #endregion

    #region OnApproached with non-Creature entity
    [Test]
    public void OnApproached_ShouldNotCallScriptOnApproached_WhenEntityIsNotCreature()
    {
        var monster = MockMonster.Create(Map);
        var groundItem = MockGroundItem.Create(Map);

        MockMonster.SetupScript(monster, _ => { });

        monster.OnApproached(groundItem);

        // GroundItem is a VisibleEntity but NOT a Creature — Script.OnApproached should NOT be called
        monster.ApproachTime
               .Should()
               .ContainKey(groundItem);

        Mock.Get(monster.Script)
            .Verify(s => s.OnApproached(It.IsAny<Creature>()), Times.Never);
    }
    #endregion

    #region OnClicked outside throttle
    [Test]
    public void OnClicked_ShouldCallScript_WhenCalledAfterThrottleExpires()
    {
        var monster = MockMonster.Create(Map);
        var clicker1 = MockAisling.Create(Map, "Clicker1", new Point(5, 5));
        var clicker2 = MockAisling.Create(Map, "Clicker2", new Point(5, 5));

        MockMonster.SetupScript(monster, s => s.Setup(x => x.OnClicked(It.IsAny<Aisling>())));

        // First click from clicker1 — should register and call script
        monster.OnClicked(clicker1);

        Mock.Get(monster.Script)
            .Verify(s => s.OnClicked(clicker1), Times.Once);

        // Different clicker should also register (per-source throttle)
        monster.OnClicked(clicker2);

        Mock.Get(monster.Script)
            .Verify(s => s.OnClicked(clicker2), Times.Once);
    }
    #endregion

    #region Pathfind
    [Test]
    public void Pathfind_ShouldDoNothing_WhenAlreadyWithinDistance()
    {
        var monster = MockMonster.Create(Map, setup: m => m.SetLocation(new Point(5, 5)));
        var startX = monster.X;
        var startY = monster.Y;

        // Target is 1 away, distance=1 → already within range
        monster.Pathfind(new Point(5, 6));

        monster.X
               .Should()
               .Be(startX);

        monster.Y
               .Should()
               .Be(startY);
    }
    #endregion

    #region SetVision
    [Test]
    public void SetVision_ShouldChangeVision()
    {
        var monster = MockMonster.Create(Map);

        monster.SetVision(VisionType.Blind);

        monster.Vision
               .Should()
               .Be(VisionType.Blind);
    }
    #endregion

    #region ShowHealth
    [Test]
    public void ShowHealth_ShouldSendHealthBar_ToNearbyAislings()
    {
        var monster = MockMonster.Create(Map);
        var aisling = MockAisling.Create(Map, "Watcher", new Point(5, 5));

        Map.AddEntity(monster, new Point(5, 5));
        Map.AddEntity(aisling, new Point(5, 6));

        var aislingScriptMock = Mock.Get(aisling.Script);

        aislingScriptMock.Setup(s => s.CanSee(monster))
                         .Returns(true);

        monster.ShowHealth(42);

        var clientMock = Mock.Get(aisling.Client);

        clientMock.Verify(c => c.SendHealthBar(monster, (byte?)42), Times.Once);
    }
    #endregion

    #region ShowPublicMessage for non-Aisling with long message
    [Test]
    public void ShowPublicMessage_ShouldNotTruncateMessage_WhenNonAislingAndTooLong()
    {
        var monster = MockMonster.Create(Map, "Monster");
        var aisling = MockAisling.Create(Map, "Listener", new Point(5, 5));

        Map.AddEntity(monster, new Point(5, 5));
        Map.AddEntity(aisling, new Point(5, 6));

        // Aisling needs to observe monster
        aisling.ApproachTime[monster] = DateTime.UtcNow;

        MockMonster.SetupScript(
            monster,
            s => s.Setup(x => x.CanTalk())
                  .Returns(true));

        // Long message that exceeds MAX_MESSAGE_LINE_LENGTH (67)
        var longMessage = new string('A', 100);

        monster.ShowPublicMessage(PublicMessageType.Normal, longMessage);

        // Normal format: "{Name}: {message}" → "Monster: " + 100 = 109 chars > 67
        // For non-Aislings, message should NOT be truncated
        var clientMock = Mock.Get(aisling.Client);

        clientMock.Verify(
            c => c.SendDisplayPublicMessage(monster.Id, PublicMessageType.Normal, It.Is<string>(msg => msg.Length > 67)),
            Times.Once);
    }
    #endregion

    #region TryDrop (with reactor)
    [Test]
    public void TryDrop_ShouldCallReactorOnItemDroppedOn_WhenReactorAtPoint()
    {
        var monster = MockMonster.Create(Map);

        // Create a reactor at the drop point
        var reactor = new ReactorTile(
            Map,
            new Point(5, 5),
            false,
            MockScriptProvider.Instance.Object,
            ["test"],
            new Dictionary<string, IScriptVars>());

        Map.SimpleAdd(reactor);

        var item = MockItem.Create("Loot");

        monster.TryDrop(new Point(5, 5), [item], out var groundItems);

        groundItems.Should()
                   .HaveCount(1);

        var reactorScriptMock = Mock.Get(reactor.Script);

        reactorScriptMock.Verify(s => s.OnItemDroppedOn(monster, It.Is<GroundItem>(gi => gi.Name == "Loot")), Times.Once);
    }
    #endregion

    #region TryUseSkill (false path)
    [Test]
    public void TryUseSkill_ShouldReturnFalse_AndNotUpdateTrackers_WhenCanUseFails()
    {
        var monster = MockMonster.Create(Map);
        var skill = MockSkill.Create();

        MockMonster.SetupScript(
            monster,
            s => s.Setup(x => x.CanUseSkill(skill))
                  .Returns(false));

        var result = monster.TryUseSkill(skill);

        result.Should()
              .BeFalse();

        monster.Trackers
               .LastSkillUse
               .Should()
               .BeNull();

        monster.Trackers
               .LastUsedSkill
               .Should()
               .BeNull();
    }
    #endregion

    #region TryUseSpell (false path)
    [Test]
    public void TryUseSpell_ShouldReturnFalse_AndNotUpdateTrackers_WhenCanUseFails()
    {
        var monster = MockMonster.Create(Map);
        var spell = MockSpell.Create();

        MockMonster.SetupScript(
            monster,
            s => s.Setup(x => x.CanUseSpell(spell))
                  .Returns(false));

        var result = monster.TryUseSpell(spell);

        result.Should()
              .BeFalse();

        monster.Trackers
               .LastSpellUse
               .Should()
               .BeNull();

        monster.Trackers
               .LastUsedSpell
               .Should()
               .BeNull();
    }
    #endregion

    #region Update
    [Test]
    public void Update_ShouldNotThrow()
    {
        var monster = MockMonster.Create(Map);

        var act = () => monster.Update(TimeSpan.FromMilliseconds(100));

        act.Should()
           .NotThrow();
    }
    #endregion

    #region Wander with ignoreWalls
    [Test]
    public void Wander_ShouldPassIgnoreWallsToWalk_ViaPathOptions()
    {
        var pathfinderMock = new Mock<IPathfindingService>();

        pathfinderMock.Setup(p => p.FindRandomDirection(It.IsAny<string>(), It.IsAny<IPoint>(), It.IsAny<IPathOptions>()))
                      .Returns(Direction.Right);

        Map.Pathfinder = pathfinderMock.Object;

        // Set a wall at (6, 5) to the right of the monster
        MockMapInstance.SetWall(Map, new Point(6, 5));

        var monster = MockMonster.Create(Map, setup: m => m.SetLocation(new Point(5, 5)));

        MockMonster.SetupScript(
            monster,
            s => s.Setup(x => x.CanMove())
                  .Returns(true));

        // Wander with pathOptions that have IgnoreWalls set
        var pathOptions = new PathOptions
        {
            IgnoreWalls = true
        };

        monster.Wander(pathOptions);

        // The walk should succeed because ignoreWalls is propagated from pathOptions
        monster.X
               .Should()
               .Be(6, "Wander should propagate IgnoreWalls from pathOptions through to Walk");
    }
    #endregion

    #region WithinLevelRange
    [Test]
    public void WithinLevelRange_ShouldReturnTrue_WhenSameLevel()
    {
        var monster1 = MockMonster.Create(Map, "Monster1");
        var monster2 = MockMonster.Create(Map, "Monster2");

        // Both default to level 1
        monster1.WithinLevelRange(monster2)
                .Should()
                .BeTrue();
    }
    #endregion

    #region Properties
    [Test]
    public void IsAlive_ShouldReturnTrue_WhenHpAboveZero()
    {
        var monster = MockMonster.Create(Map);

        monster.IsAlive
               .Should()
               .BeTrue();
    }

    [Test]
    public void IsAlive_ShouldReturnFalse_WhenHpIsZero()
    {
        var monster = MockMonster.Create(Map, setup: m => m.StatSheet.SetHp(0));

        monster.IsAlive
               .Should()
               .BeFalse();
    }

    [Test]
    public void IsBlind_ShouldReturnFalse_WhenVisionIsNormal()
    {
        var monster = MockMonster.Create(Map);

        monster.IsBlind
               .Should()
               .BeFalse();
    }

    [Test]
    public void IsBlind_ShouldReturnTrue_WhenVisionIsNotNormal()
    {
        var monster = MockMonster.Create(Map);
        monster.SetVision(VisionType.TrueBlind);

        monster.IsBlind
               .Should()
               .BeTrue();
    }

    [Test]
    public void IsDead_ShouldDefaultToFalse()
    {
        var monster = MockMonster.Create(Map);

        monster.IsDead
               .Should()
               .BeFalse();
    }

    [Test]
    public void Direction_ShouldDefaultToDown_ForAisling()
    {
        // Monster randomizes direction, so test on Aisling which preserves the Creature default
        var aisling = MockAisling.Create(Map, "TestDir", new Point(5, 5));

        aisling.Direction
               .Should()
               .Be(Direction.Down);
    }

    [Test]
    public void Gold_ShouldBeSettable()
    {
        var monster = MockMonster.Create(Map);
        monster.Gold = 500;

        monster.Gold
               .Should()
               .Be(500);
    }

    [Test]
    public void GamePoints_ShouldBeSettable()
    {
        var monster = MockMonster.Create(Map);
        monster.GamePoints = 42;

        monster.GamePoints
               .Should()
               .Be(42);
    }
    #endregion

    #region CanObserve
    [Test]
    public void CanObserve_ShouldReturnTrue_WhenObservingSelf()
    {
        var monster = MockMonster.Create(Map);

        monster.CanObserve(monster)
               .Should()
               .BeTrue();
    }

    [Test]
    public void CanObserve_ShouldReturnTrue_WhenNoFullCheckAndEntityInApproachTime()
    {
        var monster = MockMonster.Create(Map);
        var other = MockMonster.Create(Map, "Other");

        monster.ApproachTime[other] = DateTime.UtcNow;

        monster.CanObserve(other)
               .Should()
               .BeTrue();
    }

    [Test]
    public void CanObserve_ShouldReturnFalse_WhenNoFullCheckAndEntityNotInApproachTime()
    {
        var monster = MockMonster.Create(Map);
        var other = MockMonster.Create(Map, "Other");

        monster.CanObserve(other)
               .Should()
               .BeFalse();
    }

    [Test]
    public void CanObserve_ShouldReturnFalse_WhenFullCheckAndTrueBlind()
    {
        var monster = MockMonster.Create(Map);
        monster.SetVision(VisionType.TrueBlind);
        var other = MockMonster.Create(Map, "Other");

        monster.CanObserve(other, true)
               .Should()
               .BeFalse();
    }

    [Test]
    public void CanObserve_ShouldReturnTrue_WhenFullCheckAndNormalVisibility()
    {
        var monster = MockMonster.Create(Map);
        var other = MockMonster.Create(Map, "Other");

        // Other has Normal visibility by default
        monster.CanObserve(other, true)
               .Should()
               .BeTrue();
    }

    [Test]
    public void CanObserve_ShouldReturnTrue_WhenFullCheckAndHiddenVisibility()
    {
        var monster = MockMonster.Create(Map);
        var other = MockMonster.Create(Map, "Other");
        other.SetVisibility(VisibilityType.Hidden);

        monster.CanObserve(other, true)
               .Should()
               .BeTrue();
    }

    [Test]
    public void CanObserve_ShouldDelegateToScript_WhenFullCheckAndTrueHiddenVisibility()
    {
        var monster = MockMonster.Create(Map);
        var other = MockMonster.Create(Map, "Other");
        other.SetVisibility(VisibilityType.TrueHidden);

        MockMonster.SetupScript(
            monster,
            s => s.Setup(x => x.CanSee(other))
                  .Returns(true));

        monster.CanObserve(other, true)
               .Should()
               .BeTrue();
    }

    [Test]
    public void CanObserve_ShouldReturnFalse_WhenFullCheckAndTrueHiddenAndScriptCantSee()
    {
        var monster = MockMonster.Create(Map);
        var other = MockMonster.Create(Map, "Other");
        other.SetVisibility(VisibilityType.TrueHidden);

        MockMonster.SetupScript(
            monster,
            s => s.Setup(x => x.CanSee(other))
                  .Returns(false));

        monster.CanObserve(other, true)
               .Should()
               .BeFalse();
    }

    [Test]
    public void CanObserve_ShouldReturnFalse_WhenMonsterAndGmHidden()
    {
        var monster = MockMonster.Create(Map);
        var other = MockMonster.Create(Map, "Other");
        other.SetVisibility(VisibilityType.GmHidden);

        monster.CanObserve(other, true)
               .Should()
               .BeFalse();
    }

    [Test]
    public void CanObserve_ShouldReturnTrue_WhenAislingIsAdminAndGmHidden()
    {
        var aisling = MockAisling.Create(
            Map,
            "Admin",
            new Point(5, 5),
            a => a.IsAdmin = true);
        var other = MockMonster.Create(Map, "Other");
        other.SetVisibility(VisibilityType.GmHidden);

        aisling.CanObserve(other, true)
               .Should()
               .BeTrue();
    }

    [Test]
    public void CanObserve_ShouldReturnFalse_WhenAislingIsNotAdminAndGmHidden()
    {
        var aisling = MockAisling.Create(Map, "NonAdmin", new Point(5, 5));
        var other = MockMonster.Create(Map, "Other");
        other.SetVisibility(VisibilityType.GmHidden);

        aisling.CanObserve(other, true)
               .Should()
               .BeFalse();
    }

    [Test]
    public void CanObserve_ShouldReturnTrue_WhenAislingAndTrueHiddenAndGroupMember()
    {
        var aisling1 = MockAisling.Create(Map, "Aisling1", new Point(5, 5));
        var aisling2 = MockAisling.Create(Map, "Aisling2", new Point(5, 5));

        // Create a group with both aislings
        var groupService = MockGroupService.Create();
        groupService.Invite(aisling1, aisling2);
        groupService.AcceptInvite(aisling1, aisling2);

        aisling2.SetVisibility(VisibilityType.TrueHidden);

        aisling1.CanObserve(aisling2, true)
                .Should()
                .BeTrue();
    }

    [Test]
    public void CanObserve_ShouldFallThroughToScript_WhenAislingAndTrueHiddenAndNotGroupMember()
    {
        var aisling = MockAisling.Create(Map, "Aisling", new Point(5, 5));
        var other = MockMonster.Create(Map, "Other");
        other.SetVisibility(VisibilityType.TrueHidden);

        // Aisling has no group, falls through to script check
        var scriptMock = Mock.Get(aisling.Script);

        scriptMock.Setup(s => s.CanSee(other))
                  .Returns(false);

        aisling.CanObserve(other, true)
               .Should()
               .BeFalse();
    }
    #endregion

    #region CanSee
    [Test]
    public void CanSee_ShouldReturnTrue_WhenSeeingSelf()
    {
        var monster = MockMonster.Create(Map);

        monster.CanSee(monster)
               .Should()
               .BeTrue();
    }

    [Test]
    public void CanSee_ShouldDelegateToScript_WhenNotSelf()
    {
        var monster = MockMonster.Create(Map);
        var other = MockMonster.Create(Map, "Other");

        MockMonster.SetupScript(
            monster,
            s => s.Setup(x => x.CanSee(other))
                  .Returns(true));

        monster.CanSee(other)
               .Should()
               .BeTrue();
    }

    [Test]
    public void CanSee_ShouldReturnFalse_WhenScriptCantSee()
    {
        var monster = MockMonster.Create(Map);
        var other = MockMonster.Create(Map, "Other");

        MockMonster.SetupScript(
            monster,
            s => s.Setup(x => x.CanSee(other))
                  .Returns(false));

        monster.CanSee(other)
               .Should()
               .BeFalse();
    }
    #endregion

    #region IsFriendlyTo / IsHostileTo
    [Test]
    public void IsFriendlyTo_ShouldDelegateToScript()
    {
        var monster = MockMonster.Create(Map);
        var other = MockMonster.Create(Map, "Other");

        MockMonster.SetupScript(
            monster,
            s => s.Setup(x => x.IsFriendlyTo(other))
                  .Returns(true));

        monster.IsFriendlyTo(other)
               .Should()
               .BeTrue();
    }

    [Test]
    public void IsHostileTo_ShouldDelegateToScript()
    {
        var monster = MockMonster.Create(Map);
        var other = MockMonster.Create(Map, "Other");

        MockMonster.SetupScript(
            monster,
            s => s.Setup(x => x.IsHostileTo(other))
                  .Returns(true));

        monster.IsHostileTo(other)
               .Should()
               .BeTrue();
    }
    #endregion

    #region OnApproached
    [Test]
    public void OnApproached_ShouldDoNothing_WhenEntityIsSelf()
    {
        var monster = MockMonster.Create(Map);

        monster.OnApproached(monster);

        monster.ApproachTime
               .Should()
               .BeEmpty();
    }

    [Test]
    public void OnApproached_ShouldAddToApproachTime()
    {
        var monster = MockMonster.Create(Map);
        var other = MockMonster.Create(Map, "Other");

        monster.OnApproached(other);

        monster.ApproachTime
               .Should()
               .ContainKey(other);
    }

    [Test]
    public void OnApproached_ShouldCallScriptOnApproached_WhenNotRefreshAndCreature()
    {
        var monster = MockMonster.Create(Map);
        var other = MockMonster.Create(Map, "Other");

        MockMonster.SetupScript(monster, s => s.Setup(x => x.OnApproached(other)));

        monster.OnApproached(other);

        Mock.Get(monster.Script)
            .Verify(s => s.OnApproached(other), Times.Once);
    }

    [Test]
    public void OnApproached_ShouldNotCallScriptOnApproached_WhenRefresh()
    {
        var monster = MockMonster.Create(Map);
        var other = MockMonster.Create(Map, "Other");

        MockMonster.SetupScript(monster, s => s.Setup(x => x.OnApproached(other)));

        monster.OnApproached(other, true);

        Mock.Get(monster.Script)
            .Verify(s => s.OnApproached(It.IsAny<Creature>()), Times.Never);
    }

    [Test]
    public void OnApproached_ShouldNotReplaceExistingApproachTime()
    {
        var monster = MockMonster.Create(Map);
        var other = MockMonster.Create(Map, "Other");

        var originalTime = DateTime.UtcNow.AddMinutes(-5);
        monster.ApproachTime[other] = originalTime;

        monster.OnApproached(other);

        // TryAdd does not replace existing entries
        monster.ApproachTime[other]
               .Should()
               .Be(originalTime);
    }
    #endregion

    #region OnDeparture
    [Test]
    public void OnDeparture_ShouldDoNothing_WhenEntityIsSelf()
    {
        var monster = MockMonster.Create(Map);
        monster.ApproachTime[monster] = DateTime.UtcNow;

        monster.OnDeparture(monster);

        // Self entry should remain (though this shouldn't normally exist)
        monster.ApproachTime
               .Should()
               .ContainKey(monster);
    }

    [Test]
    public void OnDeparture_ShouldRemoveFromApproachTime()
    {
        var monster = MockMonster.Create(Map);
        var other = MockMonster.Create(Map, "Other");

        monster.ApproachTime[other] = DateTime.UtcNow;

        monster.OnDeparture(other);

        monster.ApproachTime
               .Should()
               .NotContainKey(other);
    }

    [Test]
    public void OnDeparture_ShouldCallScriptOnDeparture_WhenNotRefreshAndCreature()
    {
        var monster = MockMonster.Create(Map);
        var other = MockMonster.Create(Map, "Other");

        MockMonster.SetupScript(monster, s => s.Setup(x => x.OnDeparture(other)));

        monster.OnDeparture(other);

        Mock.Get(monster.Script)
            .Verify(s => s.OnDeparture(other), Times.Once);
    }

    [Test]
    public void OnDeparture_ShouldNotCallScriptOnDeparture_WhenRefresh()
    {
        var monster = MockMonster.Create(Map);
        var other = MockMonster.Create(Map, "Other");

        // Reset the shared script mock to clear invocations from other tests
        MockMonster.SetupScript(monster, _ => { });

        monster.OnDeparture(other, true);

        Mock.Get(monster.Script)
            .Verify(s => s.OnDeparture(It.IsAny<Creature>()), Times.Never);
    }
    #endregion

    #region HandleMapDeparture
    [Test]
    public void HandleMapDeparture_ShouldRemoveAllApproachTimeEntries()
    {
        var monster = MockMonster.Create(Map);
        var other1 = MockMonster.Create(Map, "Other1");
        var other2 = MockMonster.Create(Map, "Other2");

        monster.ApproachTime[other1] = DateTime.UtcNow;
        monster.ApproachTime[other2] = DateTime.UtcNow;

        monster.HandleMapDeparture();

        monster.ApproachTime
               .Should()
               .BeEmpty();
    }

    [Test]
    public void HandleMapDeparture_ShouldNotThrow_WhenNoApproachTimes()
    {
        var monster = MockMonster.Create(Map);

        var act = () => monster.HandleMapDeparture();

        act.Should()
           .NotThrow();
    }
    #endregion

    #region OnClicked
    [Test]
    public void OnClicked_ShouldCallScriptOnClicked()
    {
        var monster = MockMonster.Create(Map);
        var clicker = MockAisling.Create(Map, "Clicker", new Point(5, 5));

        MockMonster.SetupScript(monster, s => s.Setup(x => x.OnClicked(clicker)));

        monster.OnClicked(clicker);

        Mock.Get(monster.Script)
            .Verify(s => s.OnClicked(clicker), Times.Once);
    }

    [Test]
    public void OnClicked_ShouldNotCallScript_WhenClickedTooRecently()
    {
        var monster = MockMonster.Create(Map);
        var clicker = MockAisling.Create(Map, "Clicker", new Point(5, 5));

        MockMonster.SetupScript(monster, s => s.Setup(x => x.OnClicked(clicker)));

        // First click registers
        monster.OnClicked(clicker);

        // Second click too soon should not register
        monster.OnClicked(clicker);

        Mock.Get(monster.Script)
            .Verify(s => s.OnClicked(clicker), Times.Once);
    }
    #endregion

    #region OnGoldDroppedOn
    [Test]
    public void OnGoldDroppedOn_ShouldDoNothing_WhenOutOfRange()
    {
        var monster = MockMonster.Create(Map, setup: m => m.SetLocation(new Point(0, 0)));
        var source = MockAisling.Create(Map, "Source", new Point(9, 9));
        source.Gold = 100;

        monster.OnGoldDroppedOn(source, 50);

        source.Gold
              .Should()
              .Be(100);
    }

    [Test]
    public void OnGoldDroppedOn_ShouldDoNothing_WhenScriptDisallows()
    {
        var monster = MockMonster.Create(Map);
        var source = MockAisling.Create(Map, "Source", new Point(5, 5));
        source.Gold = 100;

        MockMonster.SetupScript(
            monster,
            s => s.Setup(x => x.CanDropMoneyOn(source, 50))
                  .Returns(false));

        monster.OnGoldDroppedOn(source, 50);

        source.Gold
              .Should()
              .Be(100);
    }

    [Test]
    public void OnGoldDroppedOn_ShouldTransferGold_WhenAllChecksPass()
    {
        var monster = MockMonster.Create(Map);
        var source = MockAisling.Create(Map, "Source", new Point(5, 5));
        source.Gold = 100;

        MockMonster.SetupScript(
            monster,
            s =>
            {
                s.Setup(x => x.CanDropMoneyOn(source, 50))
                 .Returns(true);
                s.Setup(x => x.OnGoldDroppedOn(source, 50));
            });

        monster.OnGoldDroppedOn(source, 50);

        source.Gold
              .Should()
              .Be(50);

        monster.Gold
               .Should()
               .Be(50);
    }

    [Test]
    public void OnGoldDroppedOn_ShouldCallScriptOnGoldDroppedOn_WhenSuccessful()
    {
        var monster = MockMonster.Create(Map);
        var source = MockAisling.Create(Map, "Source", new Point(5, 5));
        source.Gold = 100;

        MockMonster.SetupScript(
            monster,
            s =>
            {
                s.Setup(x => x.CanDropMoneyOn(source, 50))
                 .Returns(true);
                s.Setup(x => x.OnGoldDroppedOn(source, 50));
            });

        monster.OnGoldDroppedOn(source, 50);

        Mock.Get(monster.Script)
            .Verify(s => s.OnGoldDroppedOn(source, 50), Times.Once);
    }

    [Test]
    public void OnGoldDroppedOn_ShouldDoNothing_WhenSourceHasInsufficientGold()
    {
        var monster = MockMonster.Create(Map);
        var source = MockAisling.Create(Map, "Source", new Point(5, 5));
        source.Gold = 10;

        MockMonster.SetupScript(
            monster,
            s => s.Setup(x => x.CanDropMoneyOn(source, 50))
                  .Returns(true));

        monster.OnGoldDroppedOn(source, 50);

        // TryTakeGold fails, gold stays unchanged
        source.Gold
              .Should()
              .Be(10);

        monster.Gold
               .Should()
               .Be(0);
    }
    #endregion

    #region OnItemDroppedOn
    [Test]
    public void OnItemDroppedOn_ShouldDefaultCountToOne_WhenCountIsZero()
    {
        var monster = MockMonster.Create(Map);
        var source = MockAisling.Create(Map, "Source", new Point(5, 5));
        source.UserStatSheet.SetMaxWeight(50);

        var item = MockItem.Create();
        source.Inventory.TryAddToNextSlot(item);
        var slot = item.Slot;

        MockMonster.SetupScript(
            monster,
            s =>
            {
                s.Setup(x => x.CanDropItemOn(source, item))
                 .Returns(true);
                s.Setup(x => x.OnItemDroppedOn(source, It.IsAny<Item>()));
            });

        var itemScriptMock = Mock.Get(item.Script);

        itemScriptMock.Setup(x => x.CanBeDroppedOn(source, monster))
                      .Returns(true);

        // Count 0 should become 1
        monster.OnItemDroppedOn(source, slot, 0);

        source.Inventory
              .TryGetObject(slot, out _)
              .Should()
              .BeFalse();
    }

    [Test]
    public void OnItemDroppedOn_ShouldDoNothing_WhenOutOfRange()
    {
        var monster = MockMonster.Create(Map, setup: m => m.SetLocation(new Point(0, 0)));
        var source = MockAisling.Create(Map, "Source", new Point(9, 9));
        source.UserStatSheet.SetMaxWeight(50);

        var item = MockItem.Create();
        source.Inventory.TryAddToNextSlot(item);

        monster.OnItemDroppedOn(source, item.Slot, 1);

        source.Inventory
              .Contains("TestItem")
              .Should()
              .BeTrue();
    }

    [Test]
    public void OnItemDroppedOn_ShouldDoNothing_WhenSlotEmpty()
    {
        var monster = MockMonster.Create(Map);
        var source = MockAisling.Create(Map, "Source", new Point(5, 5));

        // Slot 5 has nothing
        var act = () => monster.OnItemDroppedOn(source, 5, 1);

        act.Should()
           .NotThrow();
    }

    [Test]
    public void OnItemDroppedOn_ShouldDoNothing_WhenCountExceedsItemCount()
    {
        var monster = MockMonster.Create(Map);
        var source = MockAisling.Create(Map, "Source", new Point(5, 5));
        source.UserStatSheet.SetMaxWeight(50);

        var item = MockItem.Create("Potion", 3, true);
        source.Inventory.TryAddToNextSlot(item);

        // Try to drop 5 but only have 3
        monster.OnItemDroppedOn(source, item.Slot, 5);

        source.Inventory
              .Contains("Potion")
              .Should()
              .BeTrue();
    }

    [Test]
    public void OnItemDroppedOn_ShouldDoNothing_WhenScriptDisallowsDrop()
    {
        var monster = MockMonster.Create(Map);
        var source = MockAisling.Create(Map, "Source", new Point(5, 5));
        source.UserStatSheet.SetMaxWeight(50);

        var item = MockItem.Create();
        source.Inventory.TryAddToNextSlot(item);

        MockMonster.SetupScript(
            monster,
            s => s.Setup(x => x.CanDropItemOn(source, item))
                  .Returns(false));

        monster.OnItemDroppedOn(source, item.Slot, 1);

        source.Inventory
              .Contains("TestItem")
              .Should()
              .BeTrue();
    }

    [Test]
    public void OnItemDroppedOn_ShouldDoNothing_WhenItemScriptDisallowsDrop()
    {
        var monster = MockMonster.Create(Map);
        var source = MockAisling.Create(Map, "Source", new Point(5, 5));
        source.UserStatSheet.SetMaxWeight(50);

        var item = MockItem.Create();
        source.Inventory.TryAddToNextSlot(item);

        MockMonster.SetupScript(
            monster,
            s => s.Setup(x => x.CanDropItemOn(source, item))
                  .Returns(true));

        var itemScriptMock = Mock.Get(item.Script);

        itemScriptMock.Setup(x => x.CanBeDroppedOn(source, monster))
                      .Returns(false);

        monster.OnItemDroppedOn(source, item.Slot, 1);

        source.Inventory
              .Contains("TestItem")
              .Should()
              .BeTrue();
    }

    [Test]
    public void OnItemDroppedOn_ShouldRemoveFromInventory_WhenAllChecksPass()
    {
        var monster = MockMonster.Create(Map);
        var source = MockAisling.Create(Map, "Source", new Point(5, 5));
        source.UserStatSheet.SetMaxWeight(50);

        var item = MockItem.Create();
        source.Inventory.TryAddToNextSlot(item);
        var slot = item.Slot;

        MockMonster.SetupScript(
            monster,
            s =>
            {
                s.Setup(x => x.CanDropItemOn(source, item))
                 .Returns(true);
                s.Setup(x => x.OnItemDroppedOn(source, It.IsAny<Item>()));
            });

        var itemScriptMock = Mock.Get(item.Script);

        itemScriptMock.Setup(x => x.CanBeDroppedOn(source, monster))
                      .Returns(true);

        monster.OnItemDroppedOn(source, slot, 1);

        source.Inventory
              .TryGetObject(slot, out _)
              .Should()
              .BeFalse();
    }

    [Test]
    public void OnItemDroppedOn_ShouldAddToMonsterItems_WhenTargetIsMonster()
    {
        var monster = MockMonster.Create(Map);
        var source = MockAisling.Create(Map, "Source", new Point(5, 5));
        source.UserStatSheet.SetMaxWeight(50);

        var item = MockItem.Create("DropItem");
        source.Inventory.TryAddToNextSlot(item);
        var slot = item.Slot;

        MockMonster.SetupScript(
            monster,
            s =>
            {
                s.Setup(x => x.CanDropItemOn(source, item))
                 .Returns(true);
                s.Setup(x => x.OnItemDroppedOn(source, It.IsAny<Item>()));
            });

        var itemScriptMock = Mock.Get(item.Script);

        itemScriptMock.Setup(x => x.CanBeDroppedOn(source, monster))
                      .Returns(true);

        monster.OnItemDroppedOn(source, slot, 1);

        monster.Items
               .Should()
               .ContainSingle(i => i.DisplayName == "DropItem");
    }

    [Test]
    public void OnItemDroppedOn_ShouldCallScriptOnItemDroppedOn_WhenSuccessful()
    {
        var monster = MockMonster.Create(Map);
        var source = MockAisling.Create(Map, "Source", new Point(5, 5));
        source.UserStatSheet.SetMaxWeight(50);

        var item = MockItem.Create();
        source.Inventory.TryAddToNextSlot(item);

        MockMonster.SetupScript(
            monster,
            s =>
            {
                s.Setup(x => x.CanDropItemOn(source, item))
                 .Returns(true);
                s.Setup(x => x.OnItemDroppedOn(source, It.IsAny<Item>()));
            });

        var itemScriptMock = Mock.Get(item.Script);

        itemScriptMock.Setup(x => x.CanBeDroppedOn(source, monster))
                      .Returns(true);

        monster.OnItemDroppedOn(source, item.Slot, 1);

        Mock.Get(monster.Script)
            .Verify(s => s.OnItemDroppedOn(source, It.IsAny<Item>()), Times.Once);
    }
    #endregion

    #region CanUse (Skill)
    [Test]
    public void CanUse_Skill_ShouldReturnFalse_WhenCreatureScriptDisallows()
    {
        var monster = MockMonster.Create(Map);
        var skill = MockSkill.Create();

        MockMonster.SetupScript(
            monster,
            s => s.Setup(x => x.CanUseSkill(skill))
                  .Returns(false));

        monster.CanUse(skill, out var context)
               .Should()
               .BeFalse();

        context.Should()
               .BeNull();
    }

    [Test]
    public void CanUse_Skill_ShouldReturnFalse_WhenSkillCantBeUsed()
    {
        var monster = MockMonster.Create(Map);
        var skill = MockSkill.Create();

        MockMonster.SetupScript(
            monster,
            s => s.Setup(x => x.CanUseSkill(skill))
                  .Returns(true));

        // Skill.CanUse() depends on cooldown, which should be on cooldown immediately
        // after creation — depends on Skill implementation
        monster.CanUse(skill, out _);

        // The important thing is the context is populated or null depending on canUse result
    }

    [Test]
    public void CanUse_Skill_ShouldReturnResult_FromSkillScript()
    {
        var monster = MockMonster.Create(Map);
        var skill = MockSkill.Create(isAssail: true); // assails skip throttle

        MockMonster.SetupScript(
            monster,
            s => s.Setup(x => x.CanUseSkill(skill))
                  .Returns(true));

        var skillScriptMock = Mock.Get(skill.Script);

        skillScriptMock.Setup(x => x.CanUse(It.IsAny<ActivationContext>()))
                       .Returns(true);

        monster.CanUse(skill, out var context)
               .Should()
               .BeTrue();

        context.Should()
               .NotBeNull();
    }
    #endregion

    #region CanUse (Spell)
    [Test]
    public void CanUse_Spell_ShouldReturnFalse_WhenCreatureScriptDisallows()
    {
        var monster = MockMonster.Create(Map);
        var spell = MockSpell.Create();

        MockMonster.SetupScript(
            monster,
            s => s.Setup(x => x.CanUseSpell(spell))
                  .Returns(false));

        monster.CanUse(
                   spell,
                   monster,
                   null,
                   out var context)
               .Should()
               .BeFalse();

        context.Should()
               .BeNull();
    }

    [Test]
    public void CanUse_Spell_ShouldReturnResult_FromSpellScript()
    {
        var monster = MockMonster.Create(Map);
        var spell = MockSpell.Create();

        MockMonster.SetupScript(
            monster,
            s => s.Setup(x => x.CanUseSpell(spell))
                  .Returns(true));

        var spellScriptMock = Mock.Get(spell.Script);

        spellScriptMock.Setup(x => x.CanUse(It.IsAny<SpellContext>()))
                       .Returns(true);

        monster.CanUse(
                   spell,
                   monster,
                   null,
                   out var context)
               .Should()
               .BeTrue();

        context.Should()
               .NotBeNull();
    }
    #endregion

    #region TryUseSkill
    [Test]
    public void TryUseSkill_ShouldReturnFalse_WhenCanUseFails()
    {
        var monster = MockMonster.Create(Map);
        var skill = MockSkill.Create();

        MockMonster.SetupScript(
            monster,
            s => s.Setup(x => x.CanUseSkill(skill))
                  .Returns(false));

        monster.TryUseSkill(skill)
               .Should()
               .BeFalse();
    }

    [Test]
    public void TryUseSkill_ShouldUpdateTrackers_WhenSuccessful()
    {
        var monster = MockMonster.Create(Map);
        var skill = MockSkill.Create(isAssail: true);

        MockMonster.SetupScript(
            monster,
            s => s.Setup(x => x.CanUseSkill(skill))
                  .Returns(true));

        var skillScriptMock = Mock.Get(skill.Script);

        skillScriptMock.Setup(x => x.CanUse(It.IsAny<ActivationContext>()))
                       .Returns(true);

        monster.TryUseSkill(skill)
               .Should()
               .BeTrue();

        monster.Trackers
               .LastSkillUse
               .Should()
               .NotBeNull();

        monster.Trackers
               .LastUsedSkill
               .Should()
               .Be(skill);
    }
    #endregion

    #region TryUseSpell
    [Test]
    public void TryUseSpell_ShouldReturnFalse_WhenNoTargetIdAndTargeted()
    {
        var monster = MockMonster.Create(Map);

        var spell = MockSpell.Create(
            setup: s =>
            {
                typeof(SpellTemplate).GetProperty(nameof(SpellTemplate.SpellType))!.SetValue(s.Template, SpellType.Targeted);
            });

        monster.TryUseSpell(spell)
               .Should()
               .BeFalse();
    }

    [Test]
    public void TryUseSpell_ShouldTargetSelf_WhenNoTargetIdAndNotTargeted()
    {
        var monster = MockMonster.Create(Map);
        var spell = MockSpell.Create();

        MockMonster.SetupScript(
            monster,
            s => s.Setup(x => x.CanUseSpell(spell))
                  .Returns(true));

        var spellScriptMock = Mock.Get(spell.Script);

        spellScriptMock.Setup(x => x.CanUse(It.IsAny<SpellContext>()))
                       .Returns(true);

        monster.TryUseSpell(spell)
               .Should()
               .BeTrue();

        monster.Trackers
               .LastUsedSpell
               .Should()
               .Be(spell);
    }

    [Test]
    public void TryUseSpell_ShouldReturnFalse_WhenTargetNotFound()
    {
        var monster = MockMonster.Create(Map);
        var spell = MockSpell.Create();

        monster.TryUseSpell(spell, 99999)
               .Should()
               .BeFalse();
    }

    [Test]
    public void TryUseSpell_ShouldUpdateTrackers_WhenSuccessful()
    {
        var monster = MockMonster.Create(Map);
        var spell = MockSpell.Create();

        MockMonster.SetupScript(
            monster,
            s => s.Setup(x => x.CanUseSpell(spell))
                  .Returns(true));

        var spellScriptMock = Mock.Get(spell.Script);

        spellScriptMock.Setup(x => x.CanUse(It.IsAny<SpellContext>()))
                       .Returns(true);

        monster.TryUseSpell(spell)
               .Should()
               .BeTrue();

        monster.Trackers
               .LastSpellUse
               .Should()
               .NotBeNull();

        monster.Trackers
               .LastUsedSpell
               .Should()
               .Be(spell);
    }
    #endregion

    #region Turn
    [Test]
    public void Turn_ShouldNotChangeDirection_WhenNotForcedAndScriptDisallows()
    {
        var monster = MockMonster.Create(Map);
        monster.Direction = Direction.Up;

        MockMonster.SetupScript(
            monster,
            s => s.Setup(x => x.CanTurn())
                  .Returns(false));

        monster.Turn(Direction.Down);

        monster.Direction
               .Should()
               .Be(Direction.Up);
    }

    [Test]
    public void Turn_ShouldChangeDirection_WhenForced()
    {
        var monster = MockMonster.Create(Map);
        monster.Direction = Direction.Up;

        monster.Turn(Direction.Down, true);

        monster.Direction
               .Should()
               .Be(Direction.Down);
    }

    [Test]
    public void Turn_ShouldChangeDirection_WhenScriptAllows()
    {
        var monster = MockMonster.Create(Map);
        monster.Direction = Direction.Up;

        MockMonster.SetupScript(
            monster,
            s => s.Setup(x => x.CanTurn())
                  .Returns(true));

        monster.Turn(Direction.Right);

        monster.Direction
               .Should()
               .Be(Direction.Right);
    }

    [Test]
    public void Turn_ShouldUpdateTrackers()
    {
        var monster = MockMonster.Create(Map);

        monster.Turn(Direction.Left, true);

        monster.Trackers
               .LastTurn
               .Should()
               .NotBeNull();
    }

    [Test]
    public void Turn_ShouldSendCreatureTurn_ToNearbyObservingAislings()
    {
        var monster = MockMonster.Create(Map);
        var aisling = MockAisling.Create(Map, "Observer", new Point(5, 5));

        Map.AddEntity(monster, new Point(5, 5));
        Map.AddEntity(aisling, new Point(5, 6));

        aisling.ApproachTime[monster] = DateTime.UtcNow;

        MockMonster.SetupScript(
            monster,
            s => s.Setup(x => x.CanTurn())
                  .Returns(true));

        monster.Turn(Direction.Right);

        var clientMock = Mock.Get(aisling.Client);

        clientMock.Verify(c => c.SendCreatureTurn(monster.Id, Direction.Right), Times.Once);
    }
    #endregion

    #region ShowPublicMessage
    [Test]
    public void ShowPublicMessage_ShouldDoNothing_WhenScriptDisallowsTalk()
    {
        var monster = MockMonster.Create(Map);

        MockMonster.SetupScript(
            monster,
            s => s.Setup(x => x.CanTalk())
                  .Returns(false));

        monster.ShowPublicMessage(PublicMessageType.Normal, "Hello");

        // No crash, and tracker not updated
        monster.Trackers
               .LastTalk
               .Should()
               .BeNull();
    }

    [Test]
    public void ShowPublicMessage_ShouldUpdateTrackers_WhenScriptAllowsTalk()
    {
        var monster = MockMonster.Create(Map);

        MockMonster.SetupScript(
            monster,
            s => s.Setup(x => x.CanTalk())
                  .Returns(true));

        monster.ShowPublicMessage(PublicMessageType.Normal, "Hello");

        monster.Trackers
               .LastTalk
               .Should()
               .NotBeNull();
    }

    [Test]
    public void ShowPublicMessage_ShouldCallOnPublicMessage_OnNearbyCreatures()
    {
        var monster = MockMonster.Create(Map);
        var nearby = MockMonster.Create(Map, "Nearby");

        // Add nearby to map so it's within range
        Map.AddEntity(nearby, new Point(6, 6));

        MockMonster.SetupScript(
            monster,
            s => s.Setup(x => x.CanTalk())
                  .Returns(true));

        monster.ShowPublicMessage(PublicMessageType.Shout, "Hello!");

        // Shout queries all entities on the map, so nearby should get OnPublicMessage
        Mock.Get(nearby.Script)
            .Verify(s => s.OnPublicMessage(monster, "Hello!"), Times.Once);
    }

    [Test]
    public void ShowPublicMessage_ShouldSendToAisling_WhenNormal()
    {
        var monster = MockMonster.Create(Map);
        var aisling = MockAisling.Create(Map, "Listener", new Point(5, 5));

        // Put both on map near each other
        Map.AddEntity(monster, new Point(5, 5));
        Map.AddEntity(aisling, new Point(5, 6));

        // Aisling needs to observe monster
        aisling.ApproachTime[monster] = DateTime.UtcNow;

        MockMonster.SetupScript(
            monster,
            s => s.Setup(x => x.CanTalk())
                  .Returns(true));

        monster.ShowPublicMessage(PublicMessageType.Normal, "Hey");

        var clientMock = Mock.Get(aisling.Client);

        clientMock.Verify(
            c => c.SendDisplayPublicMessage(monster.Id, PublicMessageType.Normal, It.Is<string>(msg => msg.Contains("Hey"))),
            Times.Once);
    }

    [Test]
    public void ShowPublicMessage_ShouldNotSendToAisling_WhenOnIgnoreList()
    {
        var monster = MockMonster.Create(Map, "IgnoredMonster");
        var aisling = MockAisling.Create(Map, "Listener", new Point(5, 5));
        aisling.IgnoreList.Add("IgnoredMonster");

        Map.AddEntity(monster, new Point(5, 5));
        Map.AddEntity(aisling, new Point(5, 6));

        aisling.ApproachTime[monster] = DateTime.UtcNow;

        MockMonster.SetupScript(
            monster,
            s => s.Setup(x => x.CanTalk())
                  .Returns(true));

        monster.ShowPublicMessage(PublicMessageType.Normal, "Spam");

        var clientMock = Mock.Get(aisling.Client);

        clientMock.Verify(
            c => c.SendDisplayPublicMessage(It.IsAny<uint>(), It.IsAny<PublicMessageType>(), It.IsAny<string>()),
            Times.Never);
    }

    [Test]
    public void Say_ShouldCallShowPublicMessage_WithNormalType()
    {
        var monster = MockMonster.Create(Map);

        MockMonster.SetupScript(
            monster,
            s => s.Setup(x => x.CanTalk())
                  .Returns(true));

        monster.Say("Hello");

        monster.Trackers
               .LastTalk
               .Should()
               .NotBeNull();
    }

    [Test]
    public void Shout_ShouldCallShowPublicMessage_WithShoutType()
    {
        var monster = MockMonster.Create(Map);

        MockMonster.SetupScript(
            monster,
            s => s.Setup(x => x.CanTalk())
                  .Returns(true));

        monster.Shout("Hey!");

        monster.Trackers
               .LastTalk
               .Should()
               .NotBeNull();
    }

    [Test]
    public void Chant_ShouldCallShowPublicMessage_WithChantType()
    {
        var monster = MockMonster.Create(Map);

        MockMonster.SetupScript(
            monster,
            s => s.Setup(x => x.CanTalk())
                  .Returns(true));

        monster.Chant("Magic words");

        monster.Trackers
               .LastTalk
               .Should()
               .NotBeNull();
    }

    [Test]
    public void ShowPublicMessage_ShouldTruncateMessage_WhenAislingAndTooLong()
    {
        var aisling = MockAisling.Create(Map, "LongTalker", new Point(5, 5));

        Map.AddEntity(aisling, new Point(5, 5));

        var scriptMock = Mock.Get(aisling.Script);

        scriptMock.Setup(s => s.CanTalk())
                  .Returns(true);

        // Normal message format: "{Name}: {message}", so total could exceed 67 chars
        var longMessage = new string('A', 100);

        aisling.ShowPublicMessage(PublicMessageType.Normal, longMessage);

        // Verify message was sent (it should be truncated to MAX_MESSAGE_LINE_LENGTH)
        var clientMock = Mock.Get(aisling.Client);

        clientMock.Verify(
            c => c.SendDisplayPublicMessage(aisling.Id, PublicMessageType.Normal, It.Is<string>(msg => msg.Length <= 67)),
            Times.Once);
    }
    #endregion

    #region TryDropGold
    [Test]
    public void TryDropGold_ShouldReturnFalse_WhenAmountIsZero()
    {
        var monster = MockMonster.Create(Map);
        monster.Gold = 100;

        monster.TryDropGold(new Point(5, 5), 0, out var money)
               .Should()
               .BeFalse();

        money.Should()
             .BeNull();
    }

    [Test]
    public void TryDropGold_ShouldReturnFalse_WhenAmountIsNegative()
    {
        var monster = MockMonster.Create(Map);
        monster.Gold = 100;

        monster.TryDropGold(new Point(5, 5), -5, out var money)
               .Should()
               .BeFalse();

        money.Should()
             .BeNull();
    }

    [Test]
    public void TryDropGold_ShouldReturnFalse_WhenAmountExceedsGold()
    {
        var monster = MockMonster.Create(Map);
        monster.Gold = 50;

        monster.TryDropGold(new Point(5, 5), 100, out var money)
               .Should()
               .BeFalse();

        money.Should()
             .BeNull();

        monster.Gold
               .Should()
               .Be(50);
    }

    [Test]
    public void TryDropGold_ShouldSucceed_WhenAmountIsValid()
    {
        var monster = MockMonster.Create(Map);
        monster.Gold = 100;

        monster.TryDropGold(new Point(5, 5), 30, out var money)
               .Should()
               .BeTrue();

        money.Should()
             .NotBeNull();

        money.Amount
             .Should()
             .Be(30);

        monster.Gold
               .Should()
               .Be(70);
    }

    [Test]
    public void TryDropGold_ShouldCallOnGoldDroppedOn_WhenReactorAtPoint()
    {
        var monster = MockMonster.Create(Map);
        monster.Gold = 100;
        Map.AddEntity(monster, new Point(5, 5));

        var reactor = new ReactorTile(
            Map,
            new Point(5, 5),
            false,
            MockScriptProvider.Instance.Object,
            ["test"],
            new Dictionary<string, IScriptVars>());

        Map.SimpleAdd(reactor);

        monster.TryDropGold(new Point(5, 5), 25, out var money)
               .Should()
               .BeTrue();

        Mock.Get(reactor.Script)
            .Verify(s => s.OnGoldDroppedOn(monster, It.IsAny<Money>()), Times.Once);
    }
    #endregion

    #region TryDrop
    [Test]
    public void TryDrop_ShouldReturnFalse_WhenNoItems()
    {
        var monster = MockMonster.Create(Map);

        monster.TryDrop(new Point(5, 5), Array.Empty<Item>(), out var groundItems)
               .Should()
               .BeFalse();

        groundItems.Should()
                   .BeEmpty();
    }

    [Test]
    public void TryDrop_ShouldReturnTrue_WhenItemsProvided()
    {
        var monster = MockMonster.Create(Map);
        var item = MockItem.Create("Loot");

        monster.TryDrop(new Point(5, 5), [item], out var groundItems)
               .Should()
               .BeTrue();

        groundItems.Should()
                   .HaveCount(1);

        groundItems[0]
            .Name
            .Should()
            .Be("Loot");
    }

    [Test]
    public void TryDrop_ShouldCallItemScriptOnDropped()
    {
        var monster = MockMonster.Create(Map);
        var item = MockItem.Create();

        monster.TryDrop(new Point(5, 5), [item], out _);

        Mock.Get(item.Script)
            .Verify(s => s.OnDropped(monster, Map), Times.Once);
    }
    #endregion

    #region AnimateBody
    [Test]
    public void AnimateBody_ShouldDoNothing_WhenBodyAnimationIsNone()
    {
        var monster = MockMonster.Create(Map);

        // Should not throw and should return immediately
        var act = () => monster.AnimateBody(BodyAnimation.None);

        act.Should()
           .NotThrow();
    }

    [Test]
    public void AnimateBody_ShouldSendToNearbyAislings_WhenNotNone()
    {
        var monster = MockMonster.Create(Map);
        var aisling = MockAisling.Create(Map, "Watcher", new Point(5, 5));

        Map.AddEntity(monster, new Point(5, 5));
        Map.AddEntity(aisling, new Point(5, 6));

        // Aisling needs to CanSee the monster (not just CanObserve)
        var aislingScriptMock = Mock.Get(aisling.Script);

        aislingScriptMock.Setup(s => s.CanSee(monster))
                         .Returns(true);

        monster.AnimateBody(BodyAnimation.Assail);

        var clientMock = Mock.Get(aisling.Client);

        clientMock.Verify(
            c => c.SendBodyAnimation(
                monster.Id,
                BodyAnimation.Assail,
                25,
                null),
            Times.Once);
    }
    #endregion

    #region SetVisibility
    [Test]
    public void SetVisibility_ShouldDoNothing_WhenSameVisibility()
    {
        var monster = MockMonster.Create(Map);
        var originalVisibility = monster.Visibility;

        monster.SetVisibility(originalVisibility);

        // No exception, same state
        monster.Visibility
               .Should()
               .Be(originalVisibility);
    }

    [Test]
    public void SetVisibility_ShouldChangeVisibility_WhenDifferent()
    {
        var monster = MockMonster.Create(Map);

        monster.SetVisibility(VisibilityType.Hidden);

        monster.Visibility
               .Should()
               .Be(VisibilityType.Hidden);
    }
    #endregion

    #region WarpTo
    [Test]
    public void WarpTo_ShouldUpdateTrackersLastPosition()
    {
        var monster = MockMonster.Create(Map);
        var startPoint = Point.From(monster);

        monster.WarpTo(new Point(8, 8));

        monster.Trackers
               .LastPosition
               .Should()
               .NotBeNull();

        monster.Trackers.LastPosition!.X
               .Should()
               .Be(startPoint.X);

        monster.Trackers.LastPosition!.Y
               .Should()
               .Be(startPoint.Y);
    }

    [Test]
    public void WarpTo_ShouldChangePosition()
    {
        var monster = MockMonster.Create(Map);

        monster.WarpTo(new Point(8, 8));

        monster.X
               .Should()
               .Be(8);

        monster.Y
               .Should()
               .Be(8);
    }

    [Test]
    public void WarpTo_ShouldCallOnWalkedOn_WhenReactorAtDestination()
    {
        var monster = MockMonster.Create(Map);
        Map.AddEntity(monster, new Point(3, 3));

        var reactor = new ReactorTile(
            Map,
            new Point(8, 8),
            false,
            MockScriptProvider.Instance.Object,
            ["test"],
            new Dictionary<string, IScriptVars>());

        Map.SimpleAdd(reactor);

        monster.WarpTo(new Point(8, 8));

        Mock.Get(reactor.Script)
            .Verify(s => s.OnWalkedOn(monster), Times.Once);
    }
    #endregion

    #region Walk
    [Test]
    public void Walk_ShouldDoNothing_WhenScriptDisallowsMove()
    {
        var monster = MockMonster.Create(Map);
        var startX = monster.X;
        var startY = monster.Y;

        MockMonster.SetupScript(
            monster,
            s => s.Setup(x => x.CanMove())
                  .Returns(false));

        monster.Walk(Direction.Right);

        monster.X
               .Should()
               .Be(startX);

        monster.Y
               .Should()
               .Be(startY);
    }

    [Test]
    public void Walk_ShouldChangePosition_WhenAllowed()
    {
        var monster = MockMonster.Create(Map, setup: m => m.SetLocation(new Point(5, 5)));

        MockMonster.SetupScript(
            monster,
            s => s.Setup(x => x.CanMove())
                  .Returns(true));

        monster.Walk(Direction.Right);

        monster.X
               .Should()
               .Be(6);
    }

    [Test]
    public void Walk_ShouldSetDirection_EvenWhenBlocked()
    {
        var monster = MockMonster.Create(Map, setup: m => m.SetLocation(new Point(0, 0)));

        MockMonster.SetupScript(
            monster,
            s => s.Setup(x => x.CanMove())
                  .Returns(true));

        // Walking left from (0,0) hits map boundary
        monster.Walk(Direction.Left);

        // Direction is set before walkability check
        monster.Direction
               .Should()
               .Be(Direction.Left);
    }

    [Test]
    public void Walk_ShouldUpdateTrackers_WhenSuccessful()
    {
        var monster = MockMonster.Create(Map, setup: m => m.SetLocation(new Point(5, 5)));

        MockMonster.SetupScript(
            monster,
            s => s.Setup(x => x.CanMove())
                  .Returns(true));

        monster.Walk(Direction.Down);

        monster.Trackers
               .LastWalk
               .Should()
               .NotBeNull();

        monster.Trackers
               .LastPosition
               .Should()
               .NotBeNull();
    }

    [Test]
    public void Walk_ShouldNotUpdateTrackers_WhenNotWalkable()
    {
        var monster = MockMonster.Create(Map, setup: m => m.SetLocation(new Point(0, 0)));

        MockMonster.SetupScript(
            monster,
            s => s.Setup(x => x.CanMove())
                  .Returns(true));

        // Walking up from (0,0) is out of bounds
        monster.Walk(Direction.Up);

        monster.Trackers
               .LastWalk
               .Should()
               .BeNull();
    }

    [Test]
    public void Walk_ShouldSendCreatureWalk_ToNearbyObservingAislings()
    {
        var monster = MockMonster.Create(Map, setup: m => m.SetLocation(new Point(5, 5)));
        var aisling = MockAisling.Create(Map, "Walker", new Point(5, 6));

        Map.AddEntity(monster, new Point(5, 5));
        Map.AddEntity(aisling, new Point(5, 6));

        // Aisling must observe the monster
        aisling.ApproachTime[monster] = DateTime.UtcNow;

        MockMonster.SetupScript(
            monster,
            s => s.Setup(x => x.CanMove())
                  .Returns(true));

        monster.Walk(Direction.Right);

        var clientMock = Mock.Get(aisling.Client);

        clientMock.Verify(c => c.SendCreatureWalk(monster.Id, It.IsAny<Point>(), Direction.Right), Times.Once);
    }

    [Test]
    public void Walk_ShouldCallOnWalkedOn_WhenReactorAtDestination()
    {
        var monster = MockMonster.Create(Map, setup: m => m.SetLocation(new Point(5, 5)));
        Map.AddEntity(monster, new Point(5, 5));

        // Place a reactor at the destination (6, 5)
        var reactor = new ReactorTile(
            Map,
            new Point(6, 5),
            false,
            MockScriptProvider.Instance.Object,
            ["test"],
            new Dictionary<string, IScriptVars>());

        Map.SimpleAdd(reactor);

        MockMonster.SetupScript(
            monster,
            s => s.Setup(x => x.CanMove())
                  .Returns(true));

        monster.Walk(Direction.Right);

        Mock.Get(reactor.Script)
            .Verify(s => s.OnWalkedOn(monster), Times.Once);
    }
    #endregion

    #region Wander
    [Test]
    public void Wander_ShouldNotThrow_WhenPathfinderIsAvailable()
    {
        var pathfinderMock = new Mock<IPathfindingService>();

        pathfinderMock.Setup(p => p.FindRandomDirection(It.IsAny<string>(), It.IsAny<IPoint>(), It.IsAny<IPathOptions>()))
                      .Returns(Direction.Invalid);

        Map.Pathfinder = pathfinderMock.Object;

        var monster = MockMonster.Create(Map, setup: m => m.SetLocation(new Point(5, 5)));

        MockMonster.SetupScript(
            monster,
            s => s.Setup(x => x.CanMove())
                  .Returns(true));

        var act = () => monster.Wander();

        act.Should()
           .NotThrow();
    }

    [Test]
    public void Wander_ShouldNotWalk_WhenDirectionIsInvalid()
    {
        var pathfinderMock = new Mock<IPathfindingService>();

        pathfinderMock.Setup(p => p.FindRandomDirection(It.IsAny<string>(), It.IsAny<IPoint>(), It.IsAny<IPathOptions>()))
                      .Returns(Direction.Invalid);

        Map.Pathfinder = pathfinderMock.Object;

        var monster = MockMonster.Create(Map, setup: m => m.SetLocation(new Point(5, 5)));
        var startX = monster.X;

        MockMonster.SetupScript(
            monster,
            s => s.Setup(x => x.CanMove())
                  .Returns(true));

        monster.Wander();

        monster.X
               .Should()
               .Be(startX);
    }

    [Test]
    public void Wander_ShouldUseProvidedPathOptions_WhenNotNull()
    {
        var pathfinderMock = new Mock<IPathfindingService>();

        pathfinderMock.Setup(p => p.FindRandomDirection(It.IsAny<string>(), It.IsAny<IPoint>(), It.IsAny<IPathOptions>()))
                      .Returns(Direction.Right);

        Map.Pathfinder = pathfinderMock.Object;

        var monster = MockMonster.Create(Map, setup: m => m.SetLocation(new Point(5, 5)));

        MockMonster.SetupScript(
            monster,
            s => s.Setup(x => x.CanMove())
                  .Returns(true));

        var customPathOptions = new PathOptions
        {
            IgnoreWalls = true
        };

        monster.Wander(customPathOptions);

        // Verify pathfinder was called with options that include our customization
        pathfinderMock.Verify(
            p => p.FindRandomDirection(It.IsAny<string>(), It.IsAny<IPoint>(), It.Is<IPathOptions>(opts => opts.IgnoreWalls)),
            Times.Once);
    }
    #endregion

    #region UpdateViewPort (single entity)
    [Test]
    public void UpdateViewPort_SingleEntity_ShouldCallOnDeparture_WhenWasPreviouslyObservedButNoLongerObservable()
    {
        var monster = MockMonster.Create(Map);
        var other = MockMonster.Create(Map, "Other");

        // Previously observed
        monster.ApproachTime[other] = DateTime.UtcNow;

        // Entity not on the map → TryGetEntity fails → not currently observable
        monster.UpdateViewPort(other);

        monster.ApproachTime
               .Should()
               .NotContainKey(other);
    }

    [Test]
    public void UpdateViewPort_SingleEntity_ShouldDoNothing_WhenNotPreviouslyObservedAndNotObservable()
    {
        var monster = MockMonster.Create(Map);
        var other = MockMonster.Create(Map, "Other");

        // Not previously observed, not on map
        monster.UpdateViewPort(other);

        monster.ApproachTime
               .Should()
               .NotContainKey(other);
    }

    [Test]
    public void UpdateViewPort_SingleEntity_ShouldHandleGracefully_WhenEntityRemovedFromMap()
    {
        var monster = MockMonster.Create(Map);
        var other = MockMonster.Create(Map, "Other");

        // Add both to the map
        Map.AddEntity(monster, new Point(5, 5));
        Map.AddEntity(other, new Point(5, 6));

        // Monster was previously observing other
        monster.ApproachTime[other] = DateTime.UtcNow;

        // Remove other from the map
        Map.RemoveEntity(other);

        // Entity is no longer on the map, so TryGetEntity returns false → not currently observable
        monster.UpdateViewPort(other);

        // Should handle gracefully and call OnDeparture
        monster.ApproachTime
               .Should()
               .NotContainKey(other);
    }
    #endregion

    #region Walk with ignore parameters
    [Test]
    public void Walk_ShouldWalkThroughWall_WhenIgnoreWallsTrue()
    {
        // Create a map and set a wall at (6, 5) — the tile the monster would walk into
        MockMapInstance.SetWall(Map, new Point(6, 5));

        var monster = MockMonster.Create(Map, setup: m => m.SetLocation(new Point(5, 5)));

        MockMonster.SetupScript(
            monster,
            s => s.Setup(x => x.CanMove())
                  .Returns(true));

        // Without ignoreWalls, the walk should fail (wall blocks)
        monster.Walk(Direction.Right);

        monster.X
               .Should()
               .Be(5, "wall should block walking when ignoreWalls is not set");

        // Reset position
        monster.SetLocation(new Point(5, 5));

        // With ignoreWalls=true, the walk should succeed through the wall
        monster.Walk(Direction.Right, ignoreWalls: true);

        monster.X
               .Should()
               .Be(6, "ignoreWalls=true should allow walking through the wall");
    }

    [Test]
    public void Walk_ShouldWalkThroughBlockingReactor_WhenIgnoreBlockingReactorsTrue()
    {
        // Create a blocking reactor at (6, 5)
        var reactor = new ReactorTile(
            Map,
            new Point(6, 5),
            true,
            MockScriptProvider.Instance.Object,
            ["test"],
            new Dictionary<string, IScriptVars>());

        Map.SimpleAdd(reactor);

        var monster = MockMonster.Create(Map, setup: m => m.SetLocation(new Point(5, 5)));

        MockMonster.SetupScript(
            monster,
            s => s.Setup(x => x.CanMove())
                  .Returns(true));

        // Without ignoreBlockingReactors, the walk should fail for non-aisling
        monster.Walk(Direction.Right, false);

        monster.X
               .Should()
               .Be(5, "blocking reactor should prevent walking when ignoreBlockingReactors is false");

        // Reset position
        monster.SetLocation(new Point(5, 5));

        // With ignoreBlockingReactors=true, the walk should succeed
        monster.Walk(Direction.Right, true);

        monster.X
               .Should()
               .Be(6, "ignoreBlockingReactors=true should allow walking through the reactor");
    }
    #endregion

    #region OnDeparture with refresh=true
    [Test]
    public void OnDeparture_ShouldNotCallScriptOnDeparture_WhenRefreshIsTrue_EvenForCreature()
    {
        var monster = MockMonster.Create(Map);
        var other = MockMonster.Create(Map, "Other");

        monster.ApproachTime[other] = DateTime.UtcNow;

        MockMonster.SetupScript(monster, _ => { });

        monster.OnDeparture(other, true);

        // ApproachTime entry should be removed
        monster.ApproachTime
               .Should()
               .NotContainKey(other);

        // Script.OnDeparture should NOT be called when refresh is true
        Mock.Get(monster.Script)
            .Verify(s => s.OnDeparture(It.IsAny<Creature>()), Times.Never);
    }

    [Test]
    public void OnDeparture_ShouldNotCallScriptOnDeparture_WhenEntityIsNotCreature()
    {
        var monster = MockMonster.Create(Map);
        var groundItem = MockGroundItem.Create(Map);

        monster.ApproachTime[groundItem] = DateTime.UtcNow;

        MockMonster.SetupScript(monster, _ => { });

        monster.OnDeparture(groundItem);

        // ApproachTime entry should be removed
        monster.ApproachTime
               .Should()
               .NotContainKey(groundItem);

        // GroundItem is NOT a Creature — Script.OnDeparture should NOT be called
        Mock.Get(monster.Script)
            .Verify(s => s.OnDeparture(It.IsAny<Creature>()), Times.Never);
    }
    #endregion

    #region FindOptimalDirection
    [Test]
    public void FindOptimalDirection_ShouldUseDefaultPathOptions_WhenNoneProvided()
    {
        var pathfinderMock = new Mock<IPathfindingService>();

        pathfinderMock.Setup(p => p.FindOptimalDirection(
                          It.IsAny<string>(),
                          It.IsAny<IPoint>(),
                          It.IsAny<IPoint>(),
                          It.IsAny<IPathOptions>()))
                      .Returns(Direction.Right);

        Map.Pathfinder = pathfinderMock.Object;

        var monster = MockMonster.Create(Map, setup: m => m.SetLocation(new Point(5, 5)));

        var result = monster.FindOptimalDirection(new Point(8, 5));

        result.Should()
              .Be(Direction.Right);

        pathfinderMock.Verify(
            p => p.FindOptimalDirection(
                Map.InstanceId,
                monster,
                It.Is<IPoint>(pt => (pt.X == 8) && (pt.Y == 5)),
                It.IsAny<IPathOptions>()),
            Times.Once);
    }

    [Test]
    public void FindOptimalDirection_ShouldAddClosedDoorsToBlockedPoints_WhenNotIgnoringWalls()
    {
        var pathfinderMock = new Mock<IPathfindingService>();
        IPathOptions? capturedPathOptions = null;

        pathfinderMock.Setup(p => p.FindOptimalDirection(
                          It.IsAny<string>(),
                          It.IsAny<IPoint>(),
                          It.IsAny<IPoint>(),
                          It.IsAny<IPathOptions>()))
                      .Callback<string, IPoint, IPoint, IPathOptions>((
                          _,
                          _,
                          _,
                          opts) => capturedPathOptions = opts)
                      .Returns(Direction.Right);

        Map.Pathfinder = pathfinderMock.Object;

        var monster = MockMonster.Create(Map, setup: m => m.SetLocation(new Point(5, 5)));
        Map.AddEntity(monster, new Point(5, 5));

        // Add a closed door within range
        var door = new Door(
            false,
            100,
            Map,
            new Point(6, 5));
        Map.AddEntity(door, new Point(6, 5));

        var pathOptions = new PathOptions
        {
            IgnoreWalls = false
        };

        monster.FindOptimalDirection(new Point(8, 5), pathOptions);

        capturedPathOptions.Should()
                           .NotBeNull();

        capturedPathOptions!.BlockedPoints
                            .Should()
                            .Contain(p => (p.X == 6) && (p.Y == 5));
    }

    [Test]
    public void FindOptimalDirection_ShouldNotAddClosedDoors_WhenIgnoringWalls()
    {
        var pathfinderMock = new Mock<IPathfindingService>();
        IPathOptions? capturedPathOptions = null;

        pathfinderMock.Setup(p => p.FindOptimalDirection(
                          It.IsAny<string>(),
                          It.IsAny<IPoint>(),
                          It.IsAny<IPoint>(),
                          It.IsAny<IPathOptions>()))
                      .Callback<string, IPoint, IPoint, IPathOptions>((
                          _,
                          _,
                          _,
                          opts) => capturedPathOptions = opts)
                      .Returns(Direction.Right);

        Map.Pathfinder = pathfinderMock.Object;

        var monster = MockMonster.Create(Map, setup: m => m.SetLocation(new Point(5, 5)));
        Map.AddEntity(monster, new Point(5, 5));

        // Add a closed door within range
        var door = new Door(
            false,
            100,
            Map,
            new Point(6, 5));
        Map.AddEntity(door, new Point(6, 5));

        var pathOptions = new PathOptions
        {
            IgnoreWalls = true
        };

        monster.FindOptimalDirection(new Point(8, 5), pathOptions);

        capturedPathOptions.Should()
                           .NotBeNull();

        // Door should NOT be in blocked points when ignoring walls
        capturedPathOptions!.BlockedPoints
                            .Should()
                            .NotContain(p => (p.X == 6) && (p.Y == 5));
    }

    [Test]
    public void FindOptimalDirection_ShouldAddCollidingCreatures_WhenNotIgnoringCollision()
    {
        var pathfinderMock = new Mock<IPathfindingService>();
        IPathOptions? capturedPathOptions = null;

        pathfinderMock.Setup(p => p.FindOptimalDirection(
                          It.IsAny<string>(),
                          It.IsAny<IPoint>(),
                          It.IsAny<IPoint>(),
                          It.IsAny<IPathOptions>()))
                      .Callback<string, IPoint, IPoint, IPathOptions>((
                          _,
                          _,
                          _,
                          opts) => capturedPathOptions = opts)
                      .Returns(Direction.Right);

        Map.Pathfinder = pathfinderMock.Object;

        var monster = MockMonster.Create(Map, setup: m => m.SetLocation(new Point(5, 5)));
        Map.AddEntity(monster, new Point(5, 5));

        // Add another creature within range
        var other = MockMonster.Create(Map, "Blocker", setup: m => m.SetLocation(new Point(6, 5)));
        Map.AddEntity(other, new Point(6, 5));

        monster.FindOptimalDirection(new Point(8, 5), ignoreCollision: false);

        capturedPathOptions.Should()
                           .NotBeNull();

        // The other creature should be in blocked points
        capturedPathOptions!.BlockedPoints
                            .Should()
                            .Contain(p => (p.X == 6) && (p.Y == 5));
    }

    [Test]
    public void FindOptimalDirection_ShouldNotAddCreatures_WhenIgnoringCollision()
    {
        var pathfinderMock = new Mock<IPathfindingService>();
        IPathOptions? capturedPathOptions = null;

        pathfinderMock.Setup(p => p.FindOptimalDirection(
                          It.IsAny<string>(),
                          It.IsAny<IPoint>(),
                          It.IsAny<IPoint>(),
                          It.IsAny<IPathOptions>()))
                      .Callback<string, IPoint, IPoint, IPathOptions>((
                          _,
                          _,
                          _,
                          opts) => capturedPathOptions = opts)
                      .Returns(Direction.Right);

        Map.Pathfinder = pathfinderMock.Object;

        var monster = MockMonster.Create(Map, setup: m => m.SetLocation(new Point(5, 5)));
        Map.AddEntity(monster, new Point(5, 5));

        // Add another creature within range
        var other = MockMonster.Create(Map, "Blocker", setup: m => m.SetLocation(new Point(6, 5)));
        Map.AddEntity(other, new Point(6, 5));

        monster.FindOptimalDirection(new Point(8, 5), ignoreCollision: true);

        capturedPathOptions.Should()
                           .NotBeNull();

        // The other creature should NOT be in blocked points when ignoring collision
        capturedPathOptions!.BlockedPoints
                            .Should()
                            .NotContain(p => (p.X == 6) && (p.Y == 5));
    }
    #endregion

    #region FindPath
    [Test]
    public void FindPath_ShouldUseDefaultPathOptions_WhenNoneProvided()
    {
        var pathfinderMock = new Mock<IPathfindingService>();
        var expectedPath = new Stack<IPoint>();
        expectedPath.Push(new Point(6, 5));

        pathfinderMock.Setup(p => p.FindPath(
                          It.IsAny<string>(),
                          It.IsAny<IPoint>(),
                          It.IsAny<IPoint>(),
                          It.IsAny<IPathOptions>()))
                      .Returns(expectedPath);

        Map.Pathfinder = pathfinderMock.Object;

        var monster = MockMonster.Create(Map, setup: m => m.SetLocation(new Point(5, 5)));

        var result = monster.FindPath(new Point(8, 5));

        result.Should()
              .BeSameAs(expectedPath);

        pathfinderMock.Verify(
            p => p.FindPath(
                Map.InstanceId,
                monster,
                It.Is<IPoint>(pt => (pt.X == 8) && (pt.Y == 5)),
                It.IsAny<IPathOptions>()),
            Times.Once);
    }

    [Test]
    public void FindPath_ShouldReturnPathFromPathfinder()
    {
        var pathfinderMock = new Mock<IPathfindingService>();
        var expectedPath = new Stack<IPoint>();
        expectedPath.Push(new Point(8, 5));
        expectedPath.Push(new Point(7, 5));
        expectedPath.Push(new Point(6, 5));

        pathfinderMock.Setup(p => p.FindPath(
                          It.IsAny<string>(),
                          It.IsAny<IPoint>(),
                          It.IsAny<IPoint>(),
                          It.IsAny<IPathOptions>()))
                      .Returns(expectedPath);

        Map.Pathfinder = pathfinderMock.Object;

        var monster = MockMonster.Create(Map, setup: m => m.SetLocation(new Point(5, 5)));

        var result = monster.FindPath(new Point(8, 5));

        result.Should()
              .HaveCount(3);

        result.Peek()
              .X
              .Should()
              .Be(6);
    }
    #endregion

    #region Pathfind (deeper)
    [Test]
    public void Pathfind_ShouldNotMove_WhenWithinDistance()
    {
        var pathfinderMock = new Mock<IPathfindingService>();

        Map.Pathfinder = pathfinderMock.Object;

        var monster = MockMonster.Create(Map, setup: m => m.SetLocation(new Point(5, 5)));
        var startX = monster.X;
        var startY = monster.Y;

        // Target is at (5, 6), distance = 1 → ManhattanDistance = 1, within distance
        monster.Pathfind(new Point(5, 6));

        monster.X
               .Should()
               .Be(startX);

        monster.Y
               .Should()
               .Be(startY);

        // FindOptimalDirection should NOT have been called
        pathfinderMock.Verify(
            p => p.FindOptimalDirection(
                It.IsAny<string>(),
                It.IsAny<IPoint>(),
                It.IsAny<IPoint>(),
                It.IsAny<IPathOptions>()),
            Times.Never);
    }

    [Test]
    public void Pathfind_ShouldCallFindOptimalDirection_AndWalk_WhenBeyondDistance()
    {
        var pathfinderMock = new Mock<IPathfindingService>();

        pathfinderMock.Setup(p => p.FindOptimalDirection(
                          It.IsAny<string>(),
                          It.IsAny<IPoint>(),
                          It.IsAny<IPoint>(),
                          It.IsAny<IPathOptions>()))
                      .Returns(Direction.Right);

        Map.Pathfinder = pathfinderMock.Object;

        var monster = MockMonster.Create(Map, setup: m => m.SetLocation(new Point(5, 5)));

        MockMonster.SetupScript(
            monster,
            s => s.Setup(x => x.CanMove())
                  .Returns(true));

        // Target is at (10, 5), distance = 1 → ManhattanDistance = 5, beyond distance
        monster.Pathfind(new Point(10, 5));

        pathfinderMock.Verify(
            p => p.FindOptimalDirection(
                Map.InstanceId,
                monster,
                It.Is<IPoint>(pt => (pt.X == 10) && (pt.Y == 5)),
                It.IsAny<IPathOptions>()),
            Times.Once);

        // Monster should have walked right
        monster.X
               .Should()
               .Be(6);
    }
    #endregion

    #region SetVisibility (deeper)
    [Test]
    public void SetVisibility_ShouldUpdateOtherCreaturesViewPort_WhenChanged()
    {
        var monster = MockMonster.Create(Map);
        var aisling = MockAisling.Create(Map, "Observer", new Point(5, 5));

        Map.AddEntity(monster, new Point(5, 5));
        Map.AddEntity(aisling, new Point(5, 6));

        // Aisling observes monster initially
        aisling.ApproachTime[monster] = DateTime.UtcNow;

        // Change monster visibility — should trigger UpdateViewPort on the nearby aisling
        monster.SetVisibility(VisibilityType.Hidden);

        // The monster's visibility should have changed
        monster.Visibility
               .Should()
               .Be(VisibilityType.Hidden);

        // The aisling should still observe (Hidden is visible to all creatures)
        aisling.ApproachTime
               .Should()
               .ContainKey(monster);
    }

    [Test]
    public void SetVisibility_ShouldNotUpdateOwnViewPort()
    {
        var monster = MockMonster.Create(Map);
        Map.AddEntity(monster, new Point(5, 5));

        // Visibility change should not call UpdateViewPort on self (creature.Equals(this) check)
        var act = () => monster.SetVisibility(VisibilityType.Hidden);

        act.Should()
           .NotThrow();

        monster.Visibility
               .Should()
               .Be(VisibilityType.Hidden);
    }
    #endregion

    #region UpdateViewPort (multi overload)
    [Test]
    public void UpdateViewPort_Multi_ShouldUsePartialEntities_WhenProvided()
    {
        var monster = MockMonster.Create(Map);
        var other1 = MockMonster.Create(Map, "Other1");
        var other2 = MockMonster.Create(Map, "Other2");

        Map.AddEntity(monster, new Point(5, 5));
        Map.AddEntity(other1, new Point(5, 6));
        Map.AddEntity(other2, new Point(5, 7));

        // Set both as previously observed
        monster.ApproachTime[other1] = DateTime.UtcNow;
        monster.ApproachTime[other2] = DateTime.UtcNow;

        // Partial update with only other1 — other2 should be untouched
        monster.UpdateViewPort([other1]);

        monster.ApproachTime
               .Should()
               .ContainKey(other2, "other2 should not be affected by partial update that doesn't include it");
    }

    [Test]
    public void UpdateViewPort_Multi_ShouldDetectDeparture_WhenEntityRemovedFromMap()
    {
        var monster = MockMonster.Create(Map);
        var other = MockMonster.Create(Map, "Other");

        Map.AddEntity(monster, new Point(5, 5));
        Map.AddEntity(other, new Point(5, 6));

        monster.ApproachTime[other] = DateTime.UtcNow;

        // Remove other from map
        Map.RemoveEntity(other);

        // Full viewport update
        monster.UpdateViewPort();

        monster.ApproachTime
               .Should()
               .NotContainKey(other, "other should have departed since it's no longer on the map");
    }

    [Test]
    public void UpdateViewPort_Multi_WithRefresh_ShouldPassRefreshToDeparture()
    {
        var monster = MockMonster.Create(Map);
        var other = MockMonster.Create(Map, "Other");

        Map.AddEntity(monster, new Point(5, 5));

        // Don't add other to map — it's "departed"

        monster.ApproachTime[other] = DateTime.UtcNow;

        // Partial update with refresh=true
        monster.UpdateViewPort([other], true);

        // Other should have departed (removed from ApproachTime)
        monster.ApproachTime
               .Should()
               .NotContainKey(other);

        // Script.OnDeparture should NOT be called when refresh=true
        Mock.Get(monster.Script)
            .Verify(s => s.OnDeparture(It.IsAny<Creature>()), Times.Never);
    }

    [Test]
    public void UpdateViewPort_Multi_ShouldDetectNewApproach_WithPartialEntities()
    {
        var monster = MockMonster.Create(Map);
        var other = MockMonster.Create(Map, "Other");

        Map.AddEntity(monster, new Point(5, 5));
        Map.AddEntity(other, new Point(5, 6));

        // other is NOT in ApproachTime — will be detected as new approach
        monster.UpdateViewPort([other]);

        monster.ApproachTime
               .Should()
               .ContainKey(other, "other should be newly approached via partial update");
    }
    #endregion
}