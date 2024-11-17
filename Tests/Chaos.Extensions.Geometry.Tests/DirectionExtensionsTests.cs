#region
using Chaos.Geometry.Abstractions.Definitions;
using FluentAssertions;
#endregion

// ReSharper disable ArrangeAttributes

namespace Chaos.Extensions.Geometry.Tests;

public sealed class DirectionExtensionsTests
{
    [Test]
    [Arguments(
        Direction.Up,
        new[]
        {
            Direction.Up,
            Direction.Right,
            Direction.Down,
            Direction.Left
        })]
    [Arguments(
        Direction.Right,
        new[]
        {
            Direction.Right,
            Direction.Down,
            Direction.Left,
            Direction.Up
        })]
    [Arguments(
        Direction.Down,
        new[]
        {
            Direction.Down,
            Direction.Left,
            Direction.Up,
            Direction.Right
        })]
    [Arguments(
        Direction.Left,
        new[]
        {
            Direction.Left,
            Direction.Up,
            Direction.Right,
            Direction.Down
        })]
    [Arguments(
        Direction.All,
        new[]
        {
            Direction.Up,
            Direction.Right,
            Direction.Down,
            Direction.Left
        })]
    public void AsEnumerable_ShouldReturnAllDirectionsInClockwiseOrder(Direction startDirection, Direction[] expectedDirections)
    {
        // Act
        var directions = startDirection.AsEnumerable();

        // Assert
        directions.Should()
                  .ContainInOrder(expectedDirections);
    }

    [Test]
    [Arguments(Direction.Up, Direction.Left, Direction.Right)]
    [Arguments(Direction.Right, Direction.Up, Direction.Down)]
    [Arguments(Direction.Down, Direction.Right, Direction.Left)]
    [Arguments(Direction.Left, Direction.Down, Direction.Up)]
    [Arguments(Direction.Invalid, Direction.Invalid, Direction.Invalid)]
    public void GetSideDirections_ShouldReturnCorrectSideDirections(Direction direction, Direction expectedSide1, Direction expectedSide2)
    {
        // Act
        (var side1, var side2) = direction.GetSideDirections();

        // Assert
        side1.Should()
             .Be(expectedSide1);

        side2.Should()
             .Be(expectedSide2);
    }

    [Test]
    [Arguments(Direction.Up, Direction.Down)]
    [Arguments(Direction.Right, Direction.Left)]
    [Arguments(Direction.Down, Direction.Up)]
    [Arguments(Direction.Left, Direction.Right)]
    [Arguments(Direction.Invalid, Direction.Invalid)]
    public void Reverse_ShouldReturnCorrectReverseDirection(Direction direction, Direction expectedReverse)
    {
        // Act
        var reverse = direction.Reverse();

        // Assert
        reverse.Should()
               .Be(expectedReverse);
    }
}