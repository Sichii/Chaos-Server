#region
using Chaos.Geometry.Abstractions;
using FluentAssertions;
#endregion

namespace Chaos.Geometry.Tests;

public sealed class ValueLocationTests
{
    [Test]
    public void Constructor_MapAndIPoint_SetsProperties()
    {
        IPoint point = new Point(4, 8);
        var vl = new ValueLocation("map2", point);

        vl.Map
          .Should()
          .Be("map2");

        vl.X
          .Should()
          .Be(4);

        vl.Y
          .Should()
          .Be(8);
    }

    [Test]
    public void Constructor_MapAndPoint_SetsProperties()
    {
        var vl = new ValueLocation("map", new Point(3, 7));

        vl.Map
          .Should()
          .Be("map");

        vl.X
          .Should()
          .Be(3);

        vl.Y
          .Should()
          .Be(7);
    }

    [Test]
    public void Constructor_MapXY_SetsProperties()
    {
        var vl = new ValueLocation("testMap", 5, 10);

        vl.Map
          .Should()
          .Be("testMap");

        vl.X
          .Should()
          .Be(5);

        vl.Y
          .Should()
          .Be(10);
    }

    [Test]
    public void Deconstruct_ReturnsMapXY()
    {
        var vl = new ValueLocation("testMap", 5, 10);

        (var map, var x, var y) = vl;

        map.Should()
           .Be("testMap");

        x.Should()
         .Be(5);

        y.Should()
         .Be(10);
    }

    [Test]
    public void Equals_ILocation_DifferentCoords_ReturnsFalse()
    {
        var vl = new ValueLocation("map", 1, 2);
        ILocation other = new Location("map", 3, 4);

        vl.Equals(other)
          .Should()
          .BeFalse();
    }

    [Test]
    public void Equals_ILocation_DifferentMap_ReturnsFalse()
    {
        var vl = new ValueLocation("map1", 1, 2);
        ILocation other = new Location("map2", 1, 2);

        vl.Equals(other)
          .Should()
          .BeFalse();
    }

    [Test]
    public void Equals_ILocation_Null_ReturnsFalse()
    {
        var vl = new ValueLocation("map", 1, 2);

        vl.Equals(null)
          .Should()
          .BeFalse();
    }

    [Test]
    public void Equals_ILocation_SameValues_ReturnsTrue()
    {
        var vl = new ValueLocation("map", 1, 2);
        ILocation other = new Location("map", 1, 2);

        vl.Equals(other)
          .Should()
          .BeTrue();
    }

    [Test]
    public void Equals_Object_ILocation_ReturnsTrue()
    {
        var vl = new ValueLocation("map", 1, 2);
        object other = new Location("map", 1, 2);

        vl.Equals(other)
          .Should()
          .BeTrue();
    }

    [Test]
    public void Equals_Object_NonLocation_ReturnsFalse()
    {
        var vl = new ValueLocation("map", 1, 2);

        // ReSharper disable once SuspiciousTypeConversion.Global
        vl.Equals("not a location")
          .Should()
          .BeFalse();
    }

    [Test]
    public void ExplicitConversion_FromLocation()
    {
        var loc = new Location("myMap", 3, 7);
        var vl = (ValueLocation)loc;

        vl.Map
          .Should()
          .Be("myMap");

        vl.X
          .Should()
          .Be(3);

        vl.Y
          .Should()
          .Be(7);
    }

    [Test]
    public void From_Location_ReturnsSameInstance()
    {
        var loc = new Location("map", 1, 2);

        var result = ValueLocation.From(loc);

        result.Should()
              .BeSameAs(loc);
    }

    [Test]
    public void GetHashCode_SameForEqualLocations()
    {
        var vl1 = new ValueLocation("map", 1, 2);
        var vl2 = new ValueLocation("map", 1, 2);

        vl1.GetHashCode()
           .Should()
           .Be(vl2.GetHashCode());
    }

    [Test]
    public void Operator_Equality_ReturnsTrue_ForEqual()
    {
        var vl = new ValueLocation("map", 1, 2);
        ILocation other = new Location("map", 1, 2);

        (vl == other).Should()
                     .BeTrue();
    }

    [Test]
    public void Operator_Inequality_ReturnsTrue_ForDifferent()
    {
        var vl = new ValueLocation("map1", 1, 2);
        ILocation other = new Location("map2", 1, 2);

        (vl != other).Should()
                     .BeTrue();
    }

    [Test]
    public void ToString_ReturnsCorrectFormat()
    {
        var vl = new ValueLocation("testMap", 5, 10);

        vl.ToString()
          .Should()
          .Be("testMap:(5, 10)");
    }

    [Test]
    public void TryParse_InvalidString_ReturnsFalse()
    {
        var result = ValueLocation.TryParse("invalid", out var location);

        result.Should()
              .BeFalse();

        location.Should()
                .BeNull();
    }

    [Test]
    public void TryParse_ValidString_ReturnsTrue()
    {
        var result = ValueLocation.TryParse("testMap:(10, 20)", out var location);

        result.Should()
              .BeTrue();

        location!.Map
                 .Should()
                 .Be("testMap");

        location.X
                .Should()
                .Be(10);

        location.Y
                .Should()
                .Be(20);
    }
}