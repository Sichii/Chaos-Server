using FluentAssertions;
using Xunit;

namespace Chaos.Extensions.Common.Tests;

public sealed class EnumerableExtensionsTests
{
    [Fact]
    public void ContainsI_Should_Return_False_When_Sequence_Does_Not_Contain_String()
    {
        // Arrange
        var enumerable = new[] { "apple", "banana", "cherry" };
        const string STR = "grape";

        // Act
        var result = enumerable.ContainsI(STR);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void ContainsI_Should_Return_False_When_Sequence_Is_Empty()
    {
        // Arrange
        var enumerable = Enumerable.Empty<string>();
        const string STR = "apple";

        // Act
        var result = enumerable.ContainsI(STR);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void ContainsI_Should_Return_True_When_Sequence_Contains_String()
    {
        // Arrange
        var enumerable = new[] { "apple", "banana", "cherry" };
        const string STR = "BaNaNa";

        // Act
        var result = enumerable.ContainsI(STR);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsNullOrEmpty_Should_Return_False_When_Sequence_Is_Not_Null_Or_Empty()
    {
        // Arrange
        var enumerable = new[] { 1, 2, 3 };

        // Act
        var result = enumerable.IsNullOrEmpty();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void IsNullOrEmpty_Should_Return_True_When_Sequence_Is_Empty()
    {
        // Arrange
        var enumerable = Enumerable.Empty<int>();

        // Act
        var result = enumerable.IsNullOrEmpty();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void IsNullOrEmpty_Should_Return_True_When_Sequence_Is_Null()
    {
        // Arrange
        IEnumerable<int>? enumerable = null;

        // Act
        var result = enumerable.IsNullOrEmpty();

        // Assert
        result.Should().BeTrue();
    }
}