#region
using Chaos.Collections;
using Chaos.Geometry;
using Chaos.Models.Templates;
using Chaos.Models.World;
using Chaos.Testing.Infrastructure.Mocks;
using FluentAssertions;
using Moq;
#endregion

namespace Chaos.Tests;

public sealed class DoorTests
{
    private readonly MapInstance Map = MockMapInstance.Create();

    #region Closed
    [Test]
    public void Closed_ShouldBeSettable()
    {
        var door = CreateDoor();

        door.Closed = false;

        door.Closed
            .Should()
            .BeFalse();

        door.Closed = true;

        door.Closed
            .Should()
            .BeTrue();
    }
    #endregion

    private Door CreateDoor(
        bool openRight = false,
        ushort sprite = 100,
        int x = 5,
        int y = 5)
        => new(
            openRight,
            sprite,
            Map,
            new Point(x, y));

    private Door CreateDoorFromTemplate(
        bool openRight = false,
        ushort sprite = 100,
        int x = 5,
        int y = 5)
    {
        var template = new DoorTemplate
        {
            Closed = true,
            OpenRight = openRight,
            Sprite = sprite,
            Point = new Point(x, y)
        };

        return new Door(template, Map);
    }

    #region HideFrom
    [Test]
    public void HideFrom_ShouldNotThrow()
    {
        var door = CreateDoor();
        var aisling = MockAisling.Create(Map, "Observer");

        var act = () => door.HideFrom(aisling);

        act.Should()
           .NotThrow();
    }
    #endregion

    #region ShowTo
    [Test]
    public void ShowTo_ShouldSendDoorsToAisling()
    {
        var door = CreateDoor();
        Map.AddEntity(door, new Point(5, 5));

        var aisling = MockAisling.Create(Map, "Observer");

        door.ShowTo(aisling);

        var clientMock = Mock.Get(aisling.Client);

        clientMock.Verify(c => c.SendDoors(It.IsAny<IEnumerable<Door>>()), Times.AtLeastOnce);
    }
    #endregion

    #region Constructor
    [Test]
    public void Constructor_ShouldDefaultClosed_ToTrue()
    {
        var door = CreateDoor();

        door.Closed
            .Should()
            .BeTrue();
    }

    [Test]
    public void Constructor_ShouldSetOpenRight()
    {
        var door = CreateDoor(true);

        door.OpenRight
            .Should()
            .BeTrue();
    }

    [Test]
    public void TemplateConstructor_ShouldSetProperties()
    {
        var door = CreateDoorFromTemplate(
            true,
            200,
            3,
            4);

        door.OpenRight
            .Should()
            .BeTrue();

        door.Sprite
            .Should()
            .Be(200);

        door.X
            .Should()
            .Be(3);

        door.Y
            .Should()
            .Be(4);

        door.Closed
            .Should()
            .BeTrue();
    }
    #endregion

    #region ShouldRegisterClick
    [Test]
    public void ShouldRegisterClick_ShouldReturnTrue_WhenLastClickedIsEmpty()
    {
        var door = CreateDoor();

        door.ShouldRegisterClick(1)
            .Should()
            .BeTrue();
    }

    [Test]
    public void ShouldRegisterClick_ShouldReturnFalse_WhenRecentlyClicked()
    {
        var door = CreateDoor();
        Map.AddEntity(door, new Point(5, 5));

        var aisling = MockAisling.Create(Map, "Clicker");
        Map.AddEntity(aisling, new Point(5, 5));

        // Click to populate LastClicked
        door.OnClicked(aisling);

        // Immediately check — should be throttled
        door.ShouldRegisterClick(aisling.Id)
            .Should()
            .BeFalse();
    }
    #endregion

    #region OnClicked
    [Test]
    public void OnClicked_ShouldToggleClosed_WhenNotThrottled()
    {
        var door = CreateDoor();
        Map.AddEntity(door, new Point(5, 5));

        var aisling = MockAisling.Create(Map, "Clicker");
        Map.AddEntity(aisling, new Point(5, 5));

        door.Closed
            .Should()
            .BeTrue();

        door.OnClicked(aisling);

        door.Closed
            .Should()
            .BeFalse();
    }

    [Test]
    public void OnClicked_ShouldNotToggle_WhenThrottled()
    {
        var door = CreateDoor();
        Map.AddEntity(door, new Point(5, 5));

        var aisling = MockAisling.Create(Map, "Clicker");
        Map.AddEntity(aisling, new Point(5, 5));

        // Click once
        door.OnClicked(aisling);

        var afterFirstClick = door.Closed; // should be false

        // Immediately click again — should be throttled
        door.OnClicked(aisling);

        door.Closed
            .Should()
            .Be(afterFirstClick);
    }

    [Test]
    public void OnClicked_ShouldSendDoorsToNearbyAislings()
    {
        var door = CreateDoor();
        Map.AddEntity(door, new Point(5, 5));

        var aisling = MockAisling.Create(Map, "Clicker");
        Map.AddEntity(aisling, new Point(5, 5));

        door.OnClicked(aisling);

        var clientMock = Mock.Get(aisling.Client);

        clientMock.Verify(c => c.SendDoors(It.IsAny<IEnumerable<Door>>()), Times.AtLeastOnce);
    }

    [Test]
    public void OnClicked_ShouldBeThrottled_AfterFirstClick()
    {
        var door = CreateDoor();
        Map.AddEntity(door, new Point(5, 5));

        var aisling = MockAisling.Create(Map, "Clicker");
        Map.AddEntity(aisling, new Point(5, 5));

        door.OnClicked(aisling);

        // After clicking, ShouldRegisterClick should return false (throttled)
        door.ShouldRegisterClick(aisling.Id)
            .Should()
            .BeFalse();
    }
    #endregion
}