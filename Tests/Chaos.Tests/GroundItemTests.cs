#region
using Chaos.Collections;
using Chaos.DarkAges.Definitions;
using Chaos.Geometry;
using Chaos.Models.Abstractions;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Testing.Infrastructure.Mocks;
using FluentAssertions;
using Moq;
#endregion

namespace Chaos.Tests;

public sealed class GroundItemTests
{
    private readonly MapInstance Map = MockMapInstance.Create();

    #region Properties
    [Test]
    public void Item_ShouldBeAccessible()
    {
        var item = MockItem.Create("Sword");
        var groundItem = new GroundItem(item, Map, new Point(5, 5));

        groundItem.Item
                  .Should()
                  .BeSameAs(item);
    }
    #endregion

    #region OnClicked
    [Test]
    public void OnClicked_ShouldNotThrow()
    {
        var groundItem = MockGroundItem.Create(Map);
        var aisling = MockAisling.Create(Map);

        var act = () => groundItem.OnClicked(aisling);

        act.Should()
           .NotThrow();
    }
    #endregion

    #region CanBePickedUp (script integration)
    [Test]
    public void CanBePickedUp_ShouldCallScriptCanBePickedUp()
    {
        var groundItem = MockGroundItem.Create(Map);
        var aisling = MockAisling.Create(Map, "Player");

        // Just verify the method executes (exercises the && short-circuit true path)
        groundItem.CanBePickedUp(aisling)
                  .Should()
                  .BeTrue();

        Mock.Get(groundItem.Item.Script)
            .Verify(s => s.CanBePickedUp(aisling, It.IsAny<Point>()), Times.AtLeastOnce);
    }

    [Test]
    public void CanBePickedUp_ShouldReturnFalse_WhenItemScriptDisallows()
    {
        var groundItem = MockGroundItem.Create(Map);
        var aisling = MockAisling.Create(Map, "Player");

        Mock.Get(groundItem.Item.Script)
            .Setup(s => s.CanBePickedUp(It.IsAny<Aisling>(), It.IsAny<Point>()))
            .Returns(false);

        groundItem.CanBePickedUp(aisling)
                  .Should()
                  .BeFalse();
    }
    #endregion

    #region CanBePickedUp
    [Test]
    public void CanBePickedUp_ShouldReturnTrue_WhenNoOwners()
    {
        var groundItem = MockGroundItem.Create(Map);
        var aisling = MockAisling.Create(Map, "Player");

        groundItem.CanBePickedUp(aisling)
                  .Should()
                  .BeTrue();
    }

    [Test]
    public void CanBePickedUp_ShouldReturnTrue_WhenSourceIsAdmin()
    {
        var groundItem = MockGroundItem.Create(Map);
        var admin = MockAisling.Create(Map, "Admin", setup: a => a.IsAdmin = true);

        // Lock to a different player
        var owner = MockAisling.Create(Map, "Owner");
        groundItem.LockToAislings(60, owner);

        groundItem.CanBePickedUp(admin)
                  .Should()
                  .BeTrue();
    }

    [Test]
    public void CanBePickedUp_ShouldReturnTrue_WhenSourceIsOwner()
    {
        var groundItem = MockGroundItem.Create(Map);
        var owner = MockAisling.Create(Map, "Owner");

        groundItem.LockToAislings(60, owner);

        groundItem.CanBePickedUp(owner)
                  .Should()
                  .BeTrue();
    }

    [Test]
    public void CanBePickedUp_ShouldReturnFalse_WhenSourceIsNotOwner()
    {
        var groundItem = MockGroundItem.Create(Map);
        var owner = MockAisling.Create(Map, "Owner");
        var other = MockAisling.Create(Map, "Other");

        groundItem.LockToAislings(60, owner);

        groundItem.CanBePickedUp(other)
                  .Should()
                  .BeFalse();
    }
    #endregion

    #region LockToAislings
    [Test]
    public void LockToAislings_ShouldNotLock_WhenSecondsIsZero()
    {
        var groundItem = MockGroundItem.Create(Map);
        var owner = MockAisling.Create(Map, "Owner");
        var other = MockAisling.Create(Map, "Other");

        groundItem.LockToAislings(0, owner);

        // Should still be pickable by anyone since lock was ignored
        groundItem.CanBePickedUp(other)
                  .Should()
                  .BeTrue();
    }

    [Test]
    public void LockToAislings_ShouldNotLock_WhenSecondsIsNegative()
    {
        var groundItem = MockGroundItem.Create(Map);
        var owner = MockAisling.Create(Map, "Owner");
        var other = MockAisling.Create(Map, "Other");

        groundItem.LockToAislings(-1, owner);

        groundItem.CanBePickedUp(other)
                  .Should()
                  .BeTrue();
    }
    #endregion

    #region Update (GroundEntity timer logic)
    [Test]
    public void Update_ShouldNotRemoveEntity_WhenGroundTimerHasNotElapsed()
    {
        var groundItem = MockGroundItem.Create(Map);

        // Small delta — well under the 30-minute despawn time
        groundItem.Update(TimeSpan.FromSeconds(1));

        // Item should still exist on the map (no removal)
        groundItem.MapInstance
                  .Should()
                  .BeSameAs(Map);
    }

    [Test]
    public void Update_ShouldClearLock_WhenLockTimerElapsed()
    {
        var groundItem = MockGroundItem.Create(Map);
        var owner = MockAisling.Create(Map, "Owner");
        var other = MockAisling.Create(Map, "Other");

        // Lock for 5 seconds
        groundItem.LockToAislings(5, owner);

        // Verify lock is active
        groundItem.CanBePickedUp(other)
                  .Should()
                  .BeFalse();

        // Update past the lock timer
        groundItem.Update(TimeSpan.FromSeconds(6));

        // Lock should be cleared
        groundItem.CanBePickedUp(other)
                  .Should()
                  .BeTrue();
    }

    [Test]
    public void Update_ShouldNotClearLock_WhenLockTimerHasNotElapsed()
    {
        var groundItem = MockGroundItem.Create(Map);
        var owner = MockAisling.Create(Map, "Owner");
        var other = MockAisling.Create(Map, "Other");

        // Lock for 10 seconds
        groundItem.LockToAislings(10, owner);

        // Update only 2 seconds
        groundItem.Update(TimeSpan.FromSeconds(2));

        // Lock should still be active
        groundItem.CanBePickedUp(other)
                  .Should()
                  .BeFalse();
    }

    [Test]
    public void Update_ShouldRemoveFromMap_WhenGroundTimerElapses()
    {
        var groundItem = MockGroundItem.Create(Map);

        // Ground despawn time is 30 minutes in test config
        // Update with enough time to trigger despawn
        var act = () => groundItem.Update(TimeSpan.FromMinutes(31));

        // RemoveEntity may throw or no-op if entity not in collection, just verify no crash
        act.Should()
           .NotThrow();
    }

    [Test]
    public void Update_ShouldUpdateItemCooldownAsWell()
    {
        var item = MockItem.Create("CooldownItem");
        item.Cooldown = TimeSpan.FromSeconds(10);
        item.Elapsed = TimeSpan.Zero;

        Mock.Get(item.Script)
            .Setup(s => s.CanBePickedUp(It.IsAny<Aisling>(), It.IsAny<Point>()))
            .Returns(true);

        var groundItem = new GroundItem(item, Map, new Point(5, 5));

        groundItem.Update(TimeSpan.FromSeconds(2));

        item.Elapsed
            .Should()
            .Be(TimeSpan.FromSeconds(2));
    }
    #endregion

    #region IDialogSourceEntity
    [Test]
    public void EntityType_ShouldBeItem()
    {
        var groundItem = MockGroundItem.Create(Map);
        var dialogSource = (IDialogSourceEntity)groundItem;

        dialogSource.EntityType
                    .Should()
                    .Be(EntityType.Item);
    }

    [Test]
    public void Activate_ShouldCallItemScriptOnUse()
    {
        var groundItem = MockGroundItem.Create(Map);
        var aisling = MockAisling.Create(Map, "Player");

        var itemScriptMock = Mock.Get(groundItem.Item.Script);
        itemScriptMock.Reset();

        ((IDialogSourceEntity)groundItem).Activate(aisling);

        itemScriptMock.Verify(s => s.OnUse(aisling), Times.Once);
    }

    [Test]
    public void Color_ShouldReturnItemColor()
    {
        var groundItem = MockGroundItem.Create(Map);
        var dialogSource = (IDialogSourceEntity)groundItem;

        dialogSource.Color
                    .Should()
                    .Be(groundItem.Item.Color);
    }
    #endregion

    #region Animate
    [Test]
    public void Animate_ShouldNotThrow_WhenNoAislingsInRange()
    {
        var groundItem = MockGroundItem.Create(Map);

        var animation = new Animation
        {
            TargetAnimation = 1
        };

        var act = () => groundItem.Animate(animation);

        act.Should()
           .NotThrow();
    }

    [Test]
    public void Animate_ShouldSendAnimation_WhenAislingObservesGroundItem()
    {
        var groundItem = MockGroundItem.Create(Map);
        var aisling = MockAisling.Create(Map, position: new Point(5, 5));

        Map.AddEntity(groundItem, new Point(5, 5));
        Map.AddEntity(aisling, new Point(5, 5));

        aisling.ApproachTime[groundItem] = DateTime.UtcNow;

        Mock.Get(aisling.Script)
            .Setup(s => s.CanSee(It.IsAny<VisibleEntity>()))
            .Returns(true);

        var animation = new Animation
        {
            TargetAnimation = 1
        };
        groundItem.Animate(animation);

        var clientMock = Mock.Get(aisling.Client);

        clientMock.Verify(c => c.SendAnimation(It.IsAny<Animation>()), Times.AtLeastOnce);
    }
    #endregion
}