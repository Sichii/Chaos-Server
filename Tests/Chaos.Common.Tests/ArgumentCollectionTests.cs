using Chaos.Collections.Common;
using FluentAssertions;
using Xunit;

namespace Chaos.Common.Tests;

public sealed class ArgumentCollectionTests
{
    [Fact]
    public void Add_WithArguments_ShouldAddArgumentsToCollection()
    {
        // Arrange
        var argumentCollection = new ArgumentCollection();

        // Act
        argumentCollection.Add(new[] { "arg1", "arg2", "arg3" });

        // Assert
        argumentCollection.Should().Equal("arg1", "arg2", "arg3");
    }

    [Fact]
    public void Add_WithString_ShouldParseArgumentsFromSpaceDelimitedStringAndAddToCollection()
    {
        // Arrange
        var argumentCollection = new ArgumentCollection
        {
            // Act
            "arg1 arg2 arg3"
        };

        // Assert
        argumentCollection.Should().Equal("arg1", "arg2", "arg3");
    }

    [Fact]
    public void Add_WithStringAndDelimiter_ShouldSplitStringIntoArgumentsAndAddToCollection()
    {
        // Arrange
        // Act
        var argumentCollection = new ArgumentCollection("arg1,arg2,arg3", ",");

        // Assert
        argumentCollection.Should().Equal("arg1", "arg2", "arg3");
    }

    [Fact]
    public void Constructor_WithArguments_ShouldInitializeCollectionWithArguments()
    {
        // Arrange
        var arguments = new List<string> { "arg1", "arg2", "arg3" };

        // Act
        var argumentCollection = new ArgumentCollection(arguments);

        // Assert
        argumentCollection.Should().Equal(arguments);
    }

    [Fact]
    public void Constructor_WithDelimiter_ShouldSplitStringsIntoArguments()
    {
        // Arrange
        const string ARGUMENT_STR = "arg1,arg2,arg3";
        const string DELIMITER = ",";

        // Act
        var argumentCollection = new ArgumentCollection(ARGUMENT_STR, DELIMITER);

        // Assert
        argumentCollection.Should().Equal("arg1", "arg2", "arg3");
    }

    [Fact]
    public void Constructor_WithString_ShouldParseArgumentsFromSpaceDelimitedString()
    {
        // Arrange
        const string ARGUMENT_STR = "arg1 arg2 arg3";

        // Act
        var argumentCollection = new ArgumentCollection(ARGUMENT_STR);

        // Assert
        argumentCollection.Should().Equal("arg1", "arg2", "arg3");
    }

    [Fact]
    public void Count_ShouldReturnNumberOfArguments()
    {
        // Arrange
        var argumentCollection = new ArgumentCollection("arg1 arg2 arg3");

        // Act
        var count = argumentCollection.Count;

        // Assert
        count.Should().Be(3);
    }

    [Fact]
    public void ToString_ShouldReturnStringRepresentationOfArguments()
    {
        // Arrange
        var argumentCollection = new ArgumentCollection("arg1 arg2 arg3");

        // Act
        var result = argumentCollection.ToString();

        // Assert
        result.Should().Be("\"arg1\" \"arg2\" \"arg3\" ");
    }

    [Fact]
    public void TryGet_WithInvalidConvertibleType_ShouldReturnDefaultValueAndFalse()
    {
        // Arrange
        var argumentCollection = new ArgumentCollection("invalid");

        // Act
        var result = argumentCollection.TryGet<int>(0, out var value);

        // Assert
        result.Should().BeFalse();
        value.Should().Be(default);
    }

    [Fact]
    public void TryGet_WithInvalidIndex_ShouldReturnDefaultValueAndFalse()
    {
        // Arrange
        var argumentCollection = new ArgumentCollection("42");

        // Act
        var result = argumentCollection.TryGet<int>(1, out var value);

        // Assert
        result.Should().BeFalse();
        value.Should().Be(default);
    }

    [Fact]
    public void TryGet_WithValidIndexAndConvertibleType_ShouldReturnConvertedValueAndTrue()
    {
        // Arrange
        var argumentCollection = new ArgumentCollection("42");

        // Act
        var result = argumentCollection.TryGet<int>(0, out var value);

        // Assert
        result.Should().BeTrue();
        value.Should().Be(42);
    }

    [Fact]
    public void TryGetNext_WithInvalidConvertibleType_ShouldReturnDefaultValueAndFalse()
    {
        // Arrange
        var argumentCollection = new ArgumentCollection("invalid");

        // Act
        var result = argumentCollection.TryGetNext<int>(out var value);

        // Assert
        result.Should().BeFalse();
        value.Should().Be(default);
    }

    [Fact]
    public void TryGetNext_WithInvalidIndex_ShouldReturnDefaultValueAndFalse()
    {
        // Arrange
        var argumentCollection = new ArgumentCollection("42");

        // Act
        argumentCollection.TryGetNext<int>(out _);
        var result = argumentCollection.TryGetNext<int>(out var value);

        // Assert
        result.Should().BeFalse();
        value.Should().Be(default);
    }

    [Fact]
    public void TryGetNext_WithValidIndexAndConvertibleType_ShouldReturnNextConvertedValueAndTrue()
    {
        // Arrange
        var argumentCollection = new ArgumentCollection("42 3.14 true");

        // Act
        var result1 = argumentCollection.TryGetNext<int>(out var value1);
        var result2 = argumentCollection.TryGetNext<double>(out var value2);
        var result3 = argumentCollection.TryGetNext<bool>(out var value3);

        // Assert
        result1.Should().BeTrue();
        value1.Should().Be(42);
        result2.Should().BeTrue();
        value2.Should().Be(3.14);
        result3.Should().BeTrue();
        value3.Should().Be(true);
    }
}