using FluentAssertions;
using Xunit;

namespace Chaos.Extensions.Common.Tests;

public sealed class StringExtensionsTests
{
    [Fact]
    public void CenterAlign_Should_Return_Empty_String_If_Width_Is_Zero()
    {
        // Arrange
        const string STR = "Hello";
        const int WIDTH = 0;

        // Act
        var result = STR.CenterAlign(WIDTH);

        // Assert
        result.Should().BeEmpty(); // Expected result: ""
    }

    [Fact]
    public void CenterAlign_Should_Return_Input_String_If_Width_Is_Less_Than_String_Length()
    {
        // Arrange
        const string STR = "Hello";
        const int WIDTH = 3;

        // Act
        var result = STR.CenterAlign(WIDTH);

        // Assert
        result.Should().Be("Hel"); // Expected result: "Hel"
    }

    [Fact]
    public void CenterAlign_Should_Return_String_Centered_In_Field()
    {
        // Arrange
        const string STR = "Hello";
        const int WIDTH = 10;

        // Act
        var result = STR.CenterAlign(WIDTH);

        // Assert
        result.Should().Be("  Hello   "); // Expected result: "  Hello   "
    }

    [Fact]
    public void ContainsI_Should_Return_False_If_String_Does_Not_Contain_Substring_Case_Insensitive()
    {
        // Arrange
        const string STR = "Hello World";
        const string SUBSTRING = "foo";

        // Act
        var result = STR.ContainsI(SUBSTRING);

        // Assert
        result.Should().BeFalse(); // Expected result: false
    }

    [Fact]
    public void ContainsI_Should_Return_True_If_String_Contains_Substring_Case_Insensitive()
    {
        // Arrange
        const string STR = "Hello World";
        const string SUBSTRING = "WORLD";

        // Act
        var result = STR.ContainsI(SUBSTRING);

        // Assert
        result.Should().BeTrue(); // Expected result: true
    }

    [Fact]
    public void EndsWithI_Should_Return_False_If_String_Does_Not_End_With_Specified_Substring_Case_Insensitive()
    {
        // Arrange
        const string STR = "Hello World";
        const string SUBSTRING = "foo";

        // Act
        var result = STR.EndsWithI(SUBSTRING);

        // Assert
        result.Should().BeFalse(); // Expected result: false
    }

    [Fact]
    public void EndsWithI_Should_Return_True_If_String_Ends_With_Specified_Substring_Case_Insensitive()
    {
        // Arrange
        const string STR = "Hello World";
        const string SUBSTRING = "WORLD";

        // Act
        var result = STR.EndsWithI(SUBSTRING);

        // Assert
        result.Should().BeTrue(); // Expected result: true
    }

    [Fact]
    public void EqualsI_Should_Return_False_If_Strings_Are_Not_Equal_Case_Insensitive()
    {
        // Arrange
        const string STR1 = "Hello World";
        const string STR2 = "foo";

        // Act
        var result = STR1.EqualsI(STR2);

        // Assert
        result.Should().BeFalse(); // Expected result: false
    }

    [Fact]
    public void EqualsI_Should_Return_True_If_Strings_Are_Equal_Case_Insensitive()
    {
        // Arrange
        const string STR1 = "Hello World";
        const string STR2 = "hello world";

        // Act
        var result = STR1.EqualsI(STR2);

        // Assert
        result.Should().BeTrue(); // Expected result: true
    }

    [Fact]
    public void FirstUpper_Should_Capitalize_First_Letter_In_Non_Empty_String()
    {
        // Arrange
        const string INPUT = "hello world";

        // Act
        var result = INPUT.FirstUpper();

        // Assert
        result.Should().Be("Hello world"); // Expected result: "Hello world"
    }

    [Fact]
    public void FirstUpper_Should_Throw_ArgumentException_When_Input_Is_Empty()
    {
        // Arrange
        const string INPUT = "";

        // Act
        Action action = () => INPUT.FirstUpper();

        // Assert
        action.Should().Throw<ArgumentException>(); // Expected result: ArgumentException
    }

    [Fact]
    public void FirstUpper_Should_Throw_ArgumentNullException_When_Input_Is_Null()
    {
        // Arrange
        string? input = null;

        // Act
        Action action = () => input!.FirstUpper();

        // Assert
        action.Should().Throw<ArgumentNullException>(); // Expected result: ArgumentNullException
    }

    [Fact]
    public void Inject_MissingParameters_ThrowsArgumentException()
    {
        // Arrange
        const string INPUT = "Hello, {One} and {Two}!";

        // Act and Assert
        INPUT.Invoking(x => x.Inject("World")).Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Inject_MultiplePlaceholders_ReturnsStringWithReplacedPlaceholders()
    {
        // Arrange
        const string INPUT = "Hello, {Wrld} and {Uvrs}!";
        const string EXPECTED = "Hello, World and Universe!";

        // Act
        var result = INPUT.Inject("World", "Universe");

        // Assert
        result.Should().Be(EXPECTED);
    }

    [Fact]
    public void Inject_NonPlaceholderBraces_ReturnsStringWithReplacedBraces()
    {
        // Arrange
        const string INPUT = "Hello, {{Wrld}}!";
        const string EXPECTED = "Hello, {Wrld}!";

        // Act
        var result = INPUT.Inject();

        // Assert
        result.Should().Be(EXPECTED);
    }

    [Fact]
    public void Inject_NoPlaceholders_ReturnsSameString()
    {
        // Arrange
        const string INPUT = "This is a test string.";

        // Act
        var result = INPUT.Inject();

        // Assert
        result.Should().Be(INPUT);
    }

    [Fact]
    public void Inject_OnePlaceholder_ReturnsStringWithReplacedPlaceholder()
    {
        // Arrange
        const string INPUT = "Hello, {Wrld}!";
        const string EXPECTED = "Hello, World!";

        // Act
        var result = INPUT.Inject("World");

        // Assert
        result.Should().Be(EXPECTED);
    }

    [Fact]
    public void ReplaceI_Should_Not_Modify_String_When_OldValue_Not_Found()
    {
        // Arrange
        const string INPUT = "Hello World";
        const string OLD_VALUE = "foo";
        const string NEW_VALUE = "bar";

        // Act
        var result = INPUT.ReplaceI(OLD_VALUE, NEW_VALUE);

        // Assert
        result.Should().Be("Hello World"); // Expected result: "Hello World"
    }

    [Fact]
    public void ReplaceI_Should_Replace_All_Occurrences_Case_Insensitive()
    {
        // Arrange
        const string INPUT = "Hello hello World";
        const string OLD_VALUE = "hello";
        const string NEW_VALUE = "Hi";

        // Act
        var result = INPUT.ReplaceI(OLD_VALUE, NEW_VALUE);

        // Assert
        result.Should().Be("Hi Hi World"); // Expected result: "Hi Hi World"
    }

    [Fact]
    public void StartsWithI_Should_Return_False_When_String_Does_Not_Start_With_Value_Case_Insensitive()
    {
        // Arrange
        const string INPUT = "Hello World";
        const string VALUE = "foo";

        // Act
        var result = INPUT.StartsWithI(VALUE);

        // Assert
        result.Should().BeFalse(); // Expected result: false
    }

    [Fact]
    public void StartsWithI_Should_Return_True_When_String_Starts_With_Value_Case_Insensitive()
    {
        // Arrange
        const string INPUT = "Hello World";
        const string VALUE = "hello";

        // Act
        var result = INPUT.StartsWithI(VALUE);

        // Assert
        result.Should().BeTrue(); // Expected result: true
    }
}