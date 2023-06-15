using Chaos.Geometry.Definitions;
using FluentAssertions;
using Xunit;

// ReSharper disable ArrangeAttributes

namespace Chaos.Geometry.Tests;

public sealed class RegexCacheTests
{
    //@formatter:off
    [Theory]
    [InlineData("Invalid input")]
    [InlineData("NoNumbers: ()")]
    [InlineData("NoNumbers: (, )")]
    [InlineData("NoNumbers: (, 123)")]
    [InlineData("NoNumbers: (123, )")]
    [InlineData("NoSpaceParenthesisColon345,678")]
    [InlineData("NoSpaceColon(345,678)")]
    [InlineData("NoSpaceColonComma(345 678)")]
    //@formatter:on
    public void Location_Regex_InvalidInput_ReturnsNoMatch(string input)
    {
        // Arrange
        var regex = RegexCache.LOCATION_REGEX;

        // Act
        var match = regex.Match(input);

        // Assert
        match.Success.Should().BeFalse();
    }

    //@formatter:off
    [Theory]
    [InlineData("Example: (123, 456)", "Example", "123", "456")]
    [InlineData("Multi-word: (901, 234)", "Multi-word", "901", "234")]
    [InlineData("NoSpace:(345,678)", "NoSpace", "345", "678")]
    [InlineData("NoParentheses: 567, 890", "NoParentheses", "567", "890")]
    [InlineData("NoColon (123, 456)", "NoColon", "123", "456")]
    [InlineData("NoComma: (901 234)", "NoComma", "901", "234")]
    //nospace+
    [InlineData("NoSpaceParentheses:345,678", "NoSpaceParentheses", "345", "678")]
    //NoSpaceParenthesisColon = invalid
    [InlineData("NoSpaceParenthesisComma:345 678", "NoSpaceParenthesisComma", "345", "678")]
    //NoSpaceColon = invalid
    //NoSpaceColonComma = invalid
    [InlineData("NoSpaceComma:(345 678)", "NoSpaceComma", "345", "678")]

    //noparenthesis+
    [InlineData("NoParenthesisColon 789, 012", "NoParenthesisColon", "789", "012")]
    [InlineData("NoParenthesisColonComma 789 012", "NoParenthesisColonComma", "789", "012")]
    [InlineData("NoParenthesisComma: 789 012", "NoParenthesisComma", "789", "012")]
    
    //nocolon+
    [InlineData("NoColonComma (123 456)", "NoColonComma", "123", "456")]
    //@formatter:on
    public void Location_Regex_ValidInput_ReturnsMatch(
        string input,
        string expectedGroup1,
        string expectedGroup2,
        string expectedGroup3
    )
    {
        // Arrange
        var regex = RegexCache.LOCATION_REGEX;

        // Act
        var match = regex.Match(input);

        // Assert
        match.Success.Should().BeTrue();
        match.Groups[1].Value.Should().Be(expectedGroup1);
        match.Groups[2].Value.Should().Be(expectedGroup2);
        match.Groups[3].Value.Should().Be(expectedGroup3);
    }

    [Theory]
    [InlineData("Invalid input")]
    [InlineData("()")]
    [InlineData("(, )")]
    [InlineData("(, 123)")]
    [InlineData("(123, )")]
    [InlineData("125,")]
    [InlineData(",125")]
    public void Point_Regex_InvalidInput_ReturnsNoMatch(string input)
    {
        // Arrange
        var regex = RegexCache.POINT_REGEX;

        // Act
        var match = regex.Match(input);

        // Assert
        match.Success.Should().BeFalse();
    }

    [Theory]
    [InlineData("(123, 456)", "123", "456")]
    [InlineData("(345,678)", "345", "678")]
    [InlineData("(901 234)", "901", "234")]
    [InlineData("123, 456", "123", "456")]
    [InlineData("345,678", "345", "678")]
    [InlineData("901 234", "901", "234")]
    public void Point_Regex_ValidInput_ReturnsMatch(string input, string expectedGroup1, string expectedGroup2)
    {
        // Arrange
        var regex = RegexCache.POINT_REGEX;

        // Act
        var match = regex.Match(input);

        // Assert
        match.Success.Should().BeTrue();
        match.Groups[1].Value.Should().Be(expectedGroup1);
        match.Groups[2].Value.Should().Be(expectedGroup2);
    }
}