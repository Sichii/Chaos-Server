#region
using Chaos.Geometry.Definitions;
using FluentAssertions;
#endregion

// ReSharper disable ArrangeAttributes

namespace Chaos.Geometry.Tests;

public sealed class RegexCacheTests
{
    //@formatter:off
    [Test]
    [Arguments("Invalid input")]
    [Arguments("NoNumbers: ()")]
    [Arguments("NoNumbers: (, )")]
    [Arguments("NoNumbers: (, 123)")]
    [Arguments("NoNumbers: (123, )")]
    [Arguments("NoSpaceParenthesisColon345,678")]
    [Arguments("NoSpaceColon(345,678)")]
    [Arguments("NoSpaceColonComma(345 678)")]
    //@formatter:on
    public void Location_Regex_InvalidInput_ReturnsNoMatch(string input)
    {
        // Arrange
         var regex = RegexCache.LocationRegex;

        // Act
        var match = regex.Match(input);

        // Assert
        match.Success
             .Should()
             .BeFalse();
    }

    //@formatter:off
    [Test]
    [Arguments("Example: (123, 456)", "Example", "123", "456")]
    [Arguments("Multi-word: (901, 234)", "Multi-word", "901", "234")]
    [Arguments("NoSpace:(345,678)", "NoSpace", "345", "678")]
    [Arguments("NoParentheses: 567, 890", "NoParentheses", "567", "890")]
    [Arguments("NoColon (123, 456)", "NoColon", "123", "456")]
    [Arguments("NoComma: (901 234)", "NoComma", "901", "234")]
    //nospace+
    [Arguments("NoSpaceParentheses:345,678", "NoSpaceParentheses", "345", "678")]
    //NoSpaceParenthesisColon = invalid
    [Arguments("NoSpaceParenthesisComma:345 678", "NoSpaceParenthesisComma", "345", "678")]
    //NoSpaceColon = invalid
    //NoSpaceColonComma = invalid
    [Arguments("NoSpaceComma:(345 678)", "NoSpaceComma", "345", "678")]

    //noparenthesis+
    [Arguments("NoParenthesisColon 789, 012", "NoParenthesisColon", "789", "012")]
    [Arguments("NoParenthesisColonComma 789 012", "NoParenthesisColonComma", "789", "012")]
    [Arguments("NoParenthesisComma: 789 012", "NoParenthesisComma", "789", "012")]
    
    //nocolon+
    [Arguments("NoColonComma (123 456)", "NoColonComma", "123", "456")]
    //@formatter:on
    public void Location_Regex_ValidInput_ReturnsMatch(
        string input,
        string expectedGroup1,
        string expectedGroup2,
        string expectedGroup3)
    {
        // Arrange
         var regex = RegexCache.LocationRegex;

        // Act
        var match = regex.Match(input);

        // Assert
        match.Success
             .Should()
             .BeTrue();

        match.Groups[1]
             .Value
             .Should()
             .Be(expectedGroup1);

        match.Groups[2]
             .Value
             .Should()
             .Be(expectedGroup2);

        match.Groups[3]
             .Value
             .Should()
             .Be(expectedGroup3);
    }

     [Test]
     [Arguments("Invalid input")]
     [Arguments("()")]
     [Arguments("(, )")]
     [Arguments("(, 123)")]
     [Arguments("(123, )")]
     [Arguments("125,")]
     [Arguments(",125")]
    public void Point_Regex_InvalidInput_ReturnsNoMatch(string input)
    {
        // Arrange
         var regex = RegexCache.PointRegex;

        // Act
        var match = regex.Match(input);

        // Assert
        match.Success
             .Should()
             .BeFalse();
    }

     [Test]
     [Arguments("(123, 456)", "123", "456")]
     [Arguments("(345,678)", "345", "678")]
     [Arguments("(901 234)", "901", "234")]
     [Arguments("123, 456", "123", "456")]
     [Arguments("345,678", "345", "678")]
     [Arguments("901 234", "901", "234")]
    public void Point_Regex_ValidInput_ReturnsMatch(string input, string expectedGroup1, string expectedGroup2)
    {
        // Arrange
         var regex = RegexCache.PointRegex;

        // Act
        var match = regex.Match(input);

        // Assert
        match.Success
             .Should()
             .BeTrue();

        match.Groups[1]
             .Value
             .Should()
             .Be(expectedGroup1);

        match.Groups[2]
             .Value
             .Should()
             .Be(expectedGroup2);
    }
}