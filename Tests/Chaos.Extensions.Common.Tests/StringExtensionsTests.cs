#region
using FluentAssertions;
#endregion

// ReSharper disable ArrangeAttributes

namespace Chaos.Extensions.Common.Tests;

public sealed class StringExtensionsTests
{
//@formatter:off
    [Test]
    [Arguments("night", "nacht", true, 0.25)]
    [Arguments("night", "nacht", false, 0.25)]
    [Arguments("context", "contact", true, 0.5)]
    [Arguments("context", "contact", false, 0.5)]
    [Arguments("Context", "contact", true, 0.3333)]
    [Arguments("Context", "contact", false, 0.5)]
    [Arguments("Stick", "sticks", true, 0.6667)]
    [Arguments("Stick", "sticks", false, 0.8889)]
    [Arguments("sticks", "Stick", true, 0.6667)]
    [Arguments("sticks", "Stick", false, 0.8889)]
    [Arguments("", "", true, 0)]
    [Arguments("", "", false, 0)]
    //@formatter:on
    public void CalculateDiceCoefficientTests(
        string string1,
        string string2,
        bool caseSensitive,
        decimal expected)
    {
        // Act
        var actual = string1.CalculateSorensenCoefficient(string2, caseSensitive);

        // Assert
        actual.Should()
              .BeApproximately(expected, 0.0001m);
    }

    //@formatter:off
    [Test]
    [Arguments("kitten", "sitting", true, 3)]
    [Arguments("kitten", "sitting", false, 3)]
    [Arguments("Kitten", "sitting", true, 3)]
    [Arguments("Kitten", "sitting", false, 3)]
    [Arguments("Saturday", "Sunday", true, 3)]
    [Arguments("Saturday", "Sunday", false, 3)]
    [Arguments("Saturday", "SUNDAY", true, 7)]
    [Arguments("Saturday", "SUNDAY", false, 3)]
    [Arguments("", "", true, 0)]
    [Arguments("", "", false, 0)]
    //@formatter:on
    public void CalculateLevenshteinDistanceTests(
        string str1,
        string str2,
        bool caseSensitive,
        int expected)
    {
        // Act
        var actual = str1.CalculateLevenshteinDistance(str2, caseSensitive);

        // Assert
        actual.Should()
              .Be(expected);
    }

    [Test]
    public void CenterAlign_Should_Return_Empty_String_If_Width_Is_Zero()
    {
        // Arrange
        const string STR = "Hello";
        const int WIDTH = 0;

        // Act
        var result = STR.CenterAlign(WIDTH);

        // Assert
        result.Should()
              .BeEmpty(); // Expected result: ""
    }

    [Test]
    public void CenterAlign_Should_Return_Input_String_If_Width_Is_Less_Than_String_Length()
    {
        // Arrange
        const string STR = "Hello";
        const int WIDTH = 3;

        // Act
        var result = STR.CenterAlign(WIDTH);

        // Assert
        result.Should()
              .Be("Hel"); // Expected result: "Hel"
    }

    [Test]
    public void CenterAlign_Should_Return_String_Centered_In_Field()
    {
        // Arrange
        const string STR = "Hello";
        const int WIDTH = 10;

        // Act
        var result = STR.CenterAlign(WIDTH);

        // Assert
        result.Should()
              .Be("  Hello   "); // Expected result: "  Hello   "
    }

    [Test]
    public void ContainsI_Should_Return_False_If_String_Does_Not_Contain_Substring_Case_Insensitive()
    {
        // Arrange
        const string STR = "Hello World";
        const string SUBSTRING = "foo";

        // Act
        var result = STR.ContainsI(SUBSTRING);

        // Assert
        result.Should()
              .BeFalse(); // Expected result: false
    }

    [Test]
    public void ContainsI_Should_Return_True_If_String_Contains_Substring_Case_Insensitive()
    {
        // Arrange
        const string STR = "Hello World";
        const string SUBSTRING = "WORLD";

        // Act
        var result = STR.ContainsI(SUBSTRING);

        // Assert
        result.Should()
              .BeTrue(); // Expected result: true
    }

    [Test]
    public void EndsWithI_Should_Return_False_If_String_Does_Not_End_With_Specified_Substring_Case_Insensitive()
    {
        // Arrange
        const string STR = "Hello World";
        const string SUBSTRING = "foo";

        // Act
        var result = STR.EndsWithI(SUBSTRING);

        // Assert
        result.Should()
              .BeFalse(); // Expected result: false
    }

    [Test]
    public void EndsWithI_Should_Return_True_If_String_Ends_With_Specified_Substring_Case_Insensitive()
    {
        // Arrange
        const string STR = "Hello World";
        const string SUBSTRING = "WORLD";

        // Act
        var result = STR.EndsWithI(SUBSTRING);

        // Assert
        result.Should()
              .BeTrue(); // Expected result: true
    }

    [Test]
    public void EqualsI_Should_Return_False_If_Strings_Are_Not_Equal_Case_Insensitive()
    {
        // Arrange
        const string STR1 = "Hello World";
        const string STR2 = "foo";

        // Act
        var result = STR1.EqualsI(STR2);

        // Assert
        result.Should()
              .BeFalse(); // Expected result: false
    }

    [Test]
    public void EqualsI_Should_Return_True_If_Strings_Are_Equal_Case_Insensitive()
    {
        // Arrange
        const string STR1 = "Hello World";
        const string STR2 = "hello world";

        // Act
        var result = STR1.EqualsI(STR2);

        // Assert
        result.Should()
              .BeTrue(); // Expected result: true
    }

    [Test]
    public void FirstUpper_Should_Capitalize_First_Letter_In_Non_Empty_String()
    {
        // Arrange
        const string INPUT = "hello world";

        // Act
        var result = INPUT.FirstUpper();

        // Assert
        result.Should()
              .Be("Hello world"); // Expected result: "Hello world"
    }

    [Test]
    public void FirstUpper_Should_Throw_ArgumentException_When_Input_Is_Empty()
    {
        // Arrange
        const string INPUT = "";

        // Act
        Action action = () => INPUT.FirstUpper();

        // Assert
        action.Should()
              .Throw<ArgumentException>(); // Expected result: ArgumentException
    }

    [Test]
    public void FirstUpper_Should_Throw_ArgumentNullException_When_Input_Is_Null()
    {
        // Arrange
        string? input = null;

        // Act
        Action action = () => input!.FirstUpper();

        // Assert
        action.Should()
              .Throw<ArgumentNullException>(); // Expected result: ArgumentNullException
    }

    [Test]
    [Arguments("Hello\r\nWorld", "Hello\nWorld")]
    [Arguments("Hello\nWorld", "Hello\nWorld")]
    [Arguments("Hello\rWorld", "Hello\nWorld")]
    [Arguments("Hello\r\n\r\nWorld", "Hello\n\nWorld")]
    [Arguments("Hello\r\nWorld\n", "Hello\nWorld")]
    [Arguments("Hello\r\nWorld\r\n", "Hello\nWorld")]
    public void FixLineEndings_ShouldReplaceLineEndingsCorrectly(string input, string expectedOutput)
    {
        // Act
        var result = input.FixLineEndings();

        // Assert
        result.Should()
              .Be(expectedOutput);
    }

    [Test]
    [Arguments(
        new[]
        {
            "Hello World",
            "Hi world",
            "Greetings, World!"
        },
        "Helo World",
        false,
        true)]
    [Arguments(
        new[]
        {
            "apple",
            "banana",
            "cherry"
        },
        "Hello World",
        false,
        false)]
    [Arguments(
        new[]
        {
            "Hello World",
            "HELLO WORLD",
            "hello world"
        },
        "HELLO World",
        true,
        false)]
    [Arguments(
        new[]
        {
            "Hello World",
            "HELLO WORLD",
            "hello world"
        },
        "HELLO World",
        false,
        true)]
    public void FuzzyContains_ShouldReturnExpectedResult_GivenVariousInputs(
        string[] samples,
        string searchTerm,
        bool caseSensitive,
        bool expected)
    {
        // Act
        var result = samples.FuzzyContains(searchTerm, caseSensitive: caseSensitive);

        // Assert
        result.Should()
              .Be(expected);
    }

    [Test]
    [MethodDataSource(nameof(FuzzySearchTestData))]
    public void FuzzySearchByTests(
        IEnumerable<string> strings,
        string str,
        decimal minCoefficient,
        decimal maxDistancePct,
        int? maxDistance,
        bool caseSensitive,
        string? expected)
    {
        // Act
        var actual = strings.FuzzySearchBy(
            s => s,
            str,
            minCoefficient,
            maxDistancePct,
            maxDistance,
            caseSensitive);

        // Assert
        actual.Should()
              .Be(expected);
    }

    public static IEnumerable<(IEnumerable<string>, string, decimal, decimal, int?, bool, string? expected)> FuzzySearchTestData()
        =>
        [
            ([
                 "kitten",
                 "sitting",
                 "mittens"
             ], "sitten", 0.6m, 0.33m, default, true, "kitten"),
            ([
                 "kitten",
                 "sitting",
                 "mittens"
             ], "sitten", 0.6m, 0.33m, default, false, "kitten"),
            ([
                 "kitten",
                 "Sitting",
                 "mittens"
             ], "sitteng", 0.6m, 0.33m, default, true, "kitten"),
            ([
                 "kitten",
                 "Sitting",
                 "mittens"
             ], "sitteng", 0.6m, 0.33m, default, false, "Sitting"),
            ([
                 "Written",
                 "writing",
                 "kitten"
             ], "ritten", 0.6m, 0.33m, default, true, "Written"),
            ([
                 "Written",
                 "writing",
                 "kitten"
             ], "ritten", 0.6m, 0.33m, default, false, "Written"),
            ([
                 "apple",
                 "banana",
                 "cherry"
             ], "peach", 0, 1, default, true, "cherry"),
            ([
                 "apple",
                 "banana",
                 "cherry"
             ], "peach", 0, 1, default, true, "cherry"),
            (Array.Empty<string>(), "peach", 0, 1, default, true, default)
        ];

    [Test]
    [MethodDataSource(nameof(FuzzySearchTestData))]
    public void FuzzySearchTests(
        IEnumerable<string> strings,
        string str,
        decimal minCoefficient,
        decimal maxDistancePct,
        int? maxDistance,
        bool caseSensitive,
        string? expected)
    {
        // Act
        var actual = strings.FuzzySearch(
            str,
            minCoefficient,
            maxDistancePct,
            maxDistance,
            caseSensitive);

        // Assert
        actual.Should()
              .Be(expected);
    }

    [Test]
    public void Inject_MissingParameters_ThrowsArgumentException()
    {
        // Arrange
        const string INPUT = "Hello, {One} and {Two}!";

        // Act and Assert
        INPUT.Invoking<string>(x => x.Inject("World"))
             .Should()
             .Throw<ArgumentException>();
    }

    [Test]
    public void Inject_MultiplePlaceholders_ReturnsStringWithReplacedPlaceholders()
    {
        // Arrange
        const string INPUT = "Hello, {Wrld} and {Uvrs}!";
        const string EXPECTED = "Hello, World and Universe!";

        // Act
        var result = INPUT.Inject(
            [
                "World",
                "Universe"
            ]);

        // Assert
        result.Should()
              .Be(EXPECTED);
    }

    [Test]
    public void Inject_NonPlaceholderBraces_ReturnsStringWithReplacedBraces()
    {
        // Arrange
        const string INPUT = "Hello, {{Wrld}}!";
        const string EXPECTED = "Hello, {Wrld}!";

        // Act
        var result = INPUT.Inject();

        // Assert
        result.Should()
              .Be(EXPECTED);
    }

    [Test]
    public void Inject_NoPlaceholders_ReturnsSameString()
    {
        // Arrange
        const string INPUT = "This is a test string.";

        // Act
        var result = INPUT.Inject();

        // Assert
        result.Should()
              .Be(INPUT);
    }

    [Test]
    public void Inject_OnePlaceholder_ReturnsStringWithReplacedPlaceholder()
    {
        // Arrange
        const string INPUT = "Hello, {Wrld}!";
        const string EXPECTED = "Hello, World!";

        // Act
        var result = INPUT.Inject("World");

        // Assert
        result.Should()
              .Be(EXPECTED);
    }

    [Test]
    public void ReplaceI_Should_Not_Modify_String_When_OldValue_Not_Found()
    {
        // Arrange
        const string INPUT = "Hello World";
        const string OLD_VALUE = "foo";
        const string NEW_VALUE = "bar";

        // Act
        var result = INPUT.ReplaceI(OLD_VALUE, NEW_VALUE);

        // Assert
        result.Should()
              .Be("Hello World"); // Expected result: "Hello World"
    }

    [Test]
    public void ReplaceI_Should_Replace_All_Occurrences_Case_Insensitive()
    {
        // Arrange
        const string INPUT = "Hello hello World";
        const string OLD_VALUE = "hello";
        const string NEW_VALUE = "Hi";

        // Act
        var result = INPUT.ReplaceI(OLD_VALUE, NEW_VALUE);

        // Assert
        result.Should()
              .Be("Hi Hi World"); // Expected result: "Hi Hi World"
    }

    [Test]
    public void StartsWithI_Should_Return_False_When_String_Does_Not_Start_With_Value_Case_Insensitive()
    {
        // Arrange
        const string INPUT = "Hello World";
        const string VALUE = "foo";

        // Act
        var result = INPUT.StartsWithI(VALUE);

        // Assert
        result.Should()
              .BeFalse(); // Expected result: false
    }

    [Test]
    public void StartsWithI_Should_Return_True_When_String_Starts_With_Value_Case_Insensitive()
    {
        // Arrange
        const string INPUT = "Hello World";
        const string VALUE = "hello";

        // Act
        var result = INPUT.StartsWithI(VALUE);

        // Assert
        result.Should()
              .BeTrue(); // Expected result: true
    }
}