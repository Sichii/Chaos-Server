using Chaos.Extensions.Common;
using FluentAssertions;
using Xunit;

// ReSharper disable ConvertToConstant.Local

namespace Chaos.Tests;

public sealed class StringInjectTests
{
    [Fact]
    public void Inject_MissingParameters_ThrowsArgumentException()
    {
        // Arrange
        var input = "Hello, {One} and {Two}!";

        // Act and Assert
        input.Invoking(x => x.Inject("World")).Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Inject_MultiplePlaceholders_ReturnsStringWithReplacedPlaceholders()
    {
        // Arrange
        var input = "Hello, {Wrld} and {Uvrs}!";
        var expected = "Hello, World and Universe!";

        // Act
        var result = input.Inject("World", "Universe");

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void Inject_NonPlaceholderBraces_ReturnsStringWithReplacedBraces()
    {
        // Arrange
        var input = "Hello, {{Wrld}}!";
        var expected = "Hello, {Wrld}!";

        // Act
        var result = input.Inject("World");

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void Inject_NoPlaceholders_ReturnsSameString()
    {
        // Arrange
        var input = "This is a test string.";

        // Act
        var result = input.Inject();

        // Assert
        result.Should().Be(input);
    }

    [Fact]
    public void Inject_OnePlaceholder_ReturnsStringWithReplacedPlaceholder()
    {
        // Arrange
        var input = "Hello, {Wrld}!";
        var expected = "Hello, World!";

        // Act
        var result = input.Inject("World");

        // Assert
        result.Should().Be(expected);
    }
}