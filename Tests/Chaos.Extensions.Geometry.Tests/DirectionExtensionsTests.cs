using Chaos.Geometry.Abstractions.Definitions;
using FluentAssertions;
using Xunit;

// ReSharper disable ArrangeAttributes

namespace Chaos.Extensions.Geometry.Tests;

public sealed class DirectionExtensionsTests
{
    [Theory]
    [InlineData(
        Direction.Up,
        new[]
        {
            Direction.Up,
            Direction.Right,
            Direction.Down,
            Direction.Left
        })]
    [InlineData(
        Direction.Right,
        new[]
        {
            Direction.Right,
            Direction.Down,
            Direction.Left,
            Direction.Up
        })]
    [InlineData(
        Direction.Down,
        new[]
        {
            Direction.Down,
            Direction.Left,
            Direction.Up,
            Direction.Right
        })]
    [InlineData(
        Direction.Left,
        new[]
        {
            Direction.Left,
            Direction.Up,
            Direction.Right,
            Direction.Down
        })]
    public void AsEnumerable_ShouldReturnAllDirectionsInClockwiseOrder(Direction startDirection, Direction[] expectedDirections)
    {
        // Act
        var directions = startDirection.AsEnumerable();

        // Assert
        directions.Should()
                  .ContainInOrder(expectedDirections);
    }

    [Theory]
    [InlineData(Direction.Up, Direction.Left, Direction.Right)]
    [InlineData(Direction.Right, Direction.Up, Direction.Down)]
    [InlineData(Direction.Down, Direction.Right, Direction.Left)]
    [InlineData(Direction.Left, Direction.Down, Direction.Up)]
    [InlineData(Direction.Invalid, Direction.Invalid, Direction.Invalid)]
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

    [Theory]
    [InlineData(Direction.Up, Direction.Down)]
    [InlineData(Direction.Right, Direction.Left)]
    [InlineData(Direction.Down, Direction.Up)]
    [InlineData(Direction.Left, Direction.Right)]
    [InlineData(Direction.Invalid, Direction.Invalid)]
    public void Reverse_ShouldReturnCorrectReverseDirection(Direction direction, Direction expectedReverse)
    {
        // Act
        var reverse = direction.Reverse();

        // Assert
        reverse.Should()
               .Be(expectedReverse);
    }
}