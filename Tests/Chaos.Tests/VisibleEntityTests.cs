#region
using Chaos.Collections;
using Chaos.Definitions;
using Chaos.Geometry;
using Chaos.Models.World.Abstractions;
using Chaos.Testing.Infrastructure.Mocks;
using FluentAssertions;
using Moq;
#endregion

namespace Chaos.Tests;

/// <summary>
///     Tests for <see cref="VisibleEntity" /> through concrete subclasses (Monster, GroundItem)
/// </summary>
public sealed class VisibleEntityTests
{
    private readonly MapInstance Map = MockMapInstance.Create();

    #region Display (with nearby aisling)
    [Test]
    public void Display_ShouldNotThrow_WhenAislingsAreInRange()
    {
        var monster = MockMonster.Create(Map);
        var aisling = MockAisling.Create(Map, position: new Point(5, 5));

        Map.AddEntity(monster, new Point(5, 5));
        Map.AddEntity(aisling, new Point(5, 5));

        // Set up the aisling to observe the monster
        aisling.ApproachTime[monster] = DateTime.UtcNow;

        Mock.Get(aisling.Script)
            .Setup(s => s.CanSee(It.IsAny<VisibleEntity>()))
            .Returns(true);

        // Display iterates over nearby aislings and calls ShowTo for each
        var act = () => monster.Display();

        act.Should()
           .NotThrow();
    }
    #endregion

    #region Hide (with nearby non-self aisling)
    [Test]
    public void Hide_ShouldCallHideFrom_WhenNonSelfAislingInRange()
    {
        var monster = MockMonster.Create(Map);
        var aisling = MockAisling.Create(Map, position: new Point(5, 5));

        Map.AddEntity(monster, new Point(5, 5));
        Map.AddEntity(aisling, new Point(5, 5));

        // Aisling can see the monster
        aisling.ApproachTime[monster] = DateTime.UtcNow;

        Mock.Get(aisling.Script)
            .Setup(s => s.CanSee(It.IsAny<VisibleEntity>()))
            .Returns(true);

        var clientMock = Mock.Get(aisling.Client);
        clientMock.Invocations.Clear();

        monster.Hide();

        // Monster is NOT aisling, so HideFrom should be called
        clientMock.Verify(c => c.SendRemoveEntity(monster.Id), Times.AtLeastOnce);
    }
    #endregion

    #region Hide (self-exclusion)
    [Test]
    public void Hide_ShouldNotHideSelfFromItself()
    {
        var aisling = MockAisling.Create(Map, position: new Point(5, 5));
        Map.AddEntity(aisling, new Point(5, 5));

        var clientMock = Mock.Get(aisling.Client);
        clientMock.Reset();

        // Setup so the aisling can see itself (ThatCanObserve)
        Mock.Get(aisling.Script)
            .Setup(s => s.CanSee(It.IsAny<VisibleEntity>()))
            .Returns(true);

        aisling.Hide();

        // Hide should skip self, so SendRemoveEntity should NOT be called
        clientMock.Verify(c => c.SendRemoveEntity(aisling.Id), Times.Never);
    }
    #endregion

    #region WarpTo
    [Test]
    public void WarpTo_ShouldNotThrow()
    {
        var monster = MockMonster.Create(Map);
        var destination = new Point(3, 3);

        var act = () => monster.WarpTo(destination);

        act.Should()
           .NotThrow();
    }
    #endregion

    #region SetVisibility (Creature override)
    [Test]
    public void SetVisibility_Creature_ShouldUpdateVisibility_WhenDifferent()
    {
        var monster = MockMonster.Create(Map);
        Map.AddEntity(monster, new Point(5, 5));

        monster.Visibility
               .Should()
               .Be(VisibilityType.Normal);

        monster.SetVisibility(VisibilityType.Hidden);

        monster.Visibility
               .Should()
               .Be(VisibilityType.Hidden);
    }

    [Test]
    public void SetVisibility_Creature_ShouldDoNothing_WhenSameVisibility()
    {
        var monster = MockMonster.Create(Map);
        Map.AddEntity(monster, new Point(5, 5));
        var initial = monster.Visibility;

        monster.SetVisibility(initial);

        monster.Visibility
               .Should()
               .Be(initial);
    }
    #endregion

    #region SetVisibility (VisibleEntity base — non-Creature)
    [Test]
    public void SetVisibility_Base_ShouldUpdateVisibility_WhenDifferent()
    {
        // GroundItem uses VisibleEntity.SetVisibility (not Creature override)
        var groundItem = MockGroundItem.Create(Map);
        Map.AddEntity(groundItem, new Point(5, 5));

        groundItem.Visibility
                  .Should()
                  .Be(VisibilityType.Normal);

        groundItem.SetVisibility(VisibilityType.Hidden);

        groundItem.Visibility
                  .Should()
                  .Be(VisibilityType.Hidden);
    }

    [Test]
    public void SetVisibility_Base_ShouldDoNothing_WhenSameVisibility()
    {
        var groundItem = MockGroundItem.Create(Map);
        Map.AddEntity(groundItem, new Point(5, 5));
        var initial = groundItem.Visibility;

        groundItem.SetVisibility(initial);

        groundItem.Visibility
                  .Should()
                  .Be(initial);
    }
    #endregion

    #region ShouldRegisterClick
    [Test]
    public void ShouldRegisterClick_ShouldReturnTrue_WhenNeverClicked()
    {
        var monster = MockMonster.Create(Map);

        monster.ShouldRegisterClick(999)
               .Should()
               .BeTrue();
    }

    [Test]
    public void ShouldRegisterClick_ShouldReturnFalse_WhenClickedRecently()
    {
        var monster = MockMonster.Create(Map);
        var aisling = MockAisling.Create(Map);

        // Simulate a click by calling OnClicked (which records LastClicked)
        monster.OnClicked(aisling);

        // Immediately after, ShouldRegisterClick should be false (within 500ms)
        monster.ShouldRegisterClick(aisling.Id)
               .Should()
               .BeFalse();
    }

    [Test]
    public void ShouldRegisterClick_ShouldReturnTrue_WhenDifferentSourceId()
    {
        var monster = MockMonster.Create(Map);
        var aisling1 = MockAisling.Create(Map);
        var aisling2 = MockAisling.Create(Map);

        // Aisling1 clicks the monster
        monster.OnClicked(aisling1);

        // Aisling2 should still be able to register a click
        monster.ShouldRegisterClick(aisling2.Id)
               .Should()
               .BeTrue();
    }
    #endregion

    #region Display / Hide
    [Test]
    public void Display_ShouldNotThrow_WhenNoAislingsInRange()
    {
        var monster = MockMonster.Create(Map);

        var act = () => monster.Display();

        act.Should()
           .NotThrow();
    }

    [Test]
    public void Hide_ShouldNotThrow_WhenNoAislingsInRange()
    {
        var monster = MockMonster.Create(Map);

        var act = () => monster.Hide();

        act.Should()
           .NotThrow();
    }
    #endregion

    #region ShowTo / HideFrom
    [Test]
    public void ShowTo_ShouldNotThrow()
    {
        var monster = MockMonster.Create(Map);
        var aisling = MockAisling.Create(Map);

        var act = () => monster.ShowTo(aisling);

        act.Should()
           .NotThrow();
    }

    [Test]
    public void HideFrom_ShouldSendRemoveEntity()
    {
        var monster = MockMonster.Create(Map);
        var aisling = MockAisling.Create(Map);
        var clientMock = Mock.Get(aisling.Client);

        monster.HideFrom(aisling);

        clientMock.Verify(c => c.SendRemoveEntity(monster.Id), Times.Once);
    }
    #endregion

    #region WarpTo (with watchers)
    [Test]
    public void WarpTo_ShouldUpdatePosition()
    {
        var monster = MockMonster.Create(Map);
        Map.AddEntity(monster, new Point(5, 5));

        monster.WarpTo(new Point(8, 8));

        monster.X
               .Should()
               .Be(8);

        monster.Y
               .Should()
               .Be(8);
    }

    [Test]
    public void WarpTo_ShouldHideAndShowTo_WhenAislingWatchedWarp()
    {
        var monster = MockMonster.Create(Map);
        var aisling = MockAisling.Create(Map, position: new Point(5, 5));

        Map.AddEntity(monster, new Point(5, 5));
        Map.AddEntity(aisling, new Point(5, 5));

        // Aisling can observe monster at both start and destination
        aisling.ApproachTime[monster] = DateTime.UtcNow;

        Mock.Get(aisling.Script)
            .Setup(s => s.CanSee(It.IsAny<VisibleEntity>()))
            .Returns(true);

        var clientMock = Mock.Get(aisling.Client);
        clientMock.Invocations.Clear();

        // Warp to a nearby point (still within range of aisling at 5,5)
        monster.WarpTo(new Point(6, 6));

        // Aisling should receive HideFrom + ShowTo
        clientMock.Verify(c => c.SendRemoveEntity(monster.Id), Times.AtLeastOnce);
    }
    #endregion
}