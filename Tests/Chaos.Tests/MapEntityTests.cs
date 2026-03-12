#region
using Chaos.Collections;
using Chaos.Geometry;
using Chaos.Testing.Infrastructure.Mocks;
using FluentAssertions;
using Moq;
#endregion

namespace Chaos.Tests;

/// <summary>
///     Tests for <see cref="Chaos.Models.World.Abstractions.MapEntity" /> through concrete subclasses
/// </summary>
public sealed class MapEntityTests
{
    private readonly MapInstance Map = MockMapInstance.Create();

    #region SetLocation(MapInstance, IPoint)
    [Test]
    public void SetLocation_WithMapInstance_ShouldUpdateCoordinatesAndMap()
    {
        var monster = MockMonster.Create(Map);
        var newMap = MockMapInstance.Create("new_map");
        var newPoint = new Point(7, 8);

        monster.SetLocation(newMap, newPoint);

        monster.X
               .Should()
               .Be(7);

        monster.Y
               .Should()
               .Be(8);

        monster.MapInstance
               .Should()
               .BeSameAs(newMap);
    }
    #endregion

    #region SetLocation(IPoint)
    [Test]
    public void SetLocation_ShouldSucceed_WhenPointIsWithinMap()
    {
        var monster = MockMonster.Create(Map);
        var destination = new Point(3, 3);

        var act = () => monster.SetLocation(destination);

        act.Should()
           .NotThrow();
    }

    [Test]
    public void SetLocation_ShouldThrow_WhenPointIsOutsideMap_AndEntityIsNotAisling()
    {
        var monster = MockMonster.Create(Map);
        var outOfBounds = new Point(100, 100);

        var act = () => monster.SetLocation(outOfBounds);

        act.Should()
           .Throw<InvalidOperationException>();
    }

    [Test]
    public void SetLocation_ShouldRefreshAndThrow_WhenPointIsOutsideMap_AndEntityIsAisling()
    {
        var aisling = MockAisling.Create(Map);
        var clientMock = Mock.Get(aisling.Client);
        var outOfBounds = new Point(100, 100);

        var act = () => aisling.SetLocation(outOfBounds);

        act.Should()
           .Throw<InvalidOperationException>();

        // Refresh(true) calls SendMapInfo among other things
        clientMock.Verify(c => c.SendMapInfo(), Times.AtLeastOnce);
    }
    #endregion
}