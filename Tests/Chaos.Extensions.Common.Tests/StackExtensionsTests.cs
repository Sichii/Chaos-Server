using FluentAssertions;
using Xunit;

namespace Chaos.Extensions.Common.Tests;

public sealed class StackExtensionsTests
{
    [Fact]
    public void PopUntil_Should_Return_Default_If_No_Item_Satisfies_Predicate()
    {
        // Arrange
        var stack = new Stack<int>();
        stack.Push(1);
        stack.Push(3);
        stack.Push(5);

        // Act
        var result = stack.PopUntil(item => item % 2 == 0); // Predicate never satisfied

        // Assert
        result.Should().Be(default);
        stack.Should().BeEmpty(); // The stack should be empty
    }

    [Fact]
    public void PopUntil_Should_Return_Default_If_Stack_Is_Empty()
    {
        // Arrange
        var stack = new Stack<int>();

        // Act
        var result = stack.PopUntil(_ => true); // Predicate always returns true

        // Assert
        result.Should().Be(default);
        stack.Should().BeEmpty(); // The stack should still be empty
    }

    [Fact]
    public void PopUntil_Should_Return_First_Item_Satisfying_Predicate()
    {
        // Arrange
        var stack = new Stack<int>();
        stack.Push(1);
        stack.Push(2);
        stack.Push(3);
        stack.Push(4);

        // Act
        var result = stack.PopUntil(item => item % 2 == 0); // Find the first even number

        // Assert
        result.Should().Be(4);
        stack.Should().Equal(3, 2, 1); // The remaining items in the stack
    }
}