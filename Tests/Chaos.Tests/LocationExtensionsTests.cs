#region
using Chaos.Extensions;
using Chaos.Geometry.Abstractions;
using FluentAssertions;
using Moq;
#endregion

namespace Chaos.Tests;

public sealed class LocationExtensionsTests
{
    private static ILocation CreateLocation(string map, int x, int y)
    {
        var mock = new Mock<ILocation>();

        mock.Setup(l => l.Map)
            .Returns(map);

        mock.Setup(l => l.X)
            .Returns(x);

        mock.Setup(l => l.Y)
            .Returns(y);

        return mock.Object;
    }

    #region WithinRange
    [Test]
    public void WithinRange_SameMap_ShouldReturnTrue_WhenInRange()
    {
        var location1 = CreateLocation("map1", 5, 5);
        var location2 = CreateLocation("map1", 6, 6);

        location1.WithinRange(location2, 5)
                 .Should()
                 .BeTrue();
    }

    [Test]
    public void WithinRange_DifferentMaps_ShouldReturnFalse()
    {
        var location1 = CreateLocation("map1", 5, 5);
        var location2 = CreateLocation("map2", 5, 5);

        location1.WithinRange(location2, 5)
                 .Should()
                 .BeFalse();
    }

    [Test]
    public void WithinRange_SameMap_ShouldReturnFalse_WhenOutOfRange()
    {
        var location1 = CreateLocation("map1", 0, 0);
        var location2 = CreateLocation("map1", 20, 20);

        location1.WithinRange(location2, 5)
                 .Should()
                 .BeFalse();
    }

    [Test]
    public void WithinRange_SamePoint_ShouldReturnTrue()
    {
        var location1 = CreateLocation("map1", 5, 5);
        var location2 = CreateLocation("map1", 5, 5);

        location1.WithinRange(location2, 0)
                 .Should()
                 .BeTrue();
    }
    #endregion
}