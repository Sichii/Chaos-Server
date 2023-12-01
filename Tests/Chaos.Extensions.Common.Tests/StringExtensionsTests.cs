

// ReSharper disable ArrangeAttributes

namespace Chaos.Extensions.Common.Tests;

public sealed class StringExtensionsTests
{
    public static IEnumerable<object?[]> FuzzySearchTestData
        => new List<object?[]>
        {
            new object?[]
            {
                new[]
                {
                    "kitten",
                    "sitting",
                    "mittens"
                },
                "sitten",
                0.6m,
                0.33m,
                default(int?),
                true,
                "kitten"
            },
            new object?[]
            {
                new[]
                {
                    "kitten",
                    "sitting",
                    "mittens"
                },
                "sitten",
                0.6m,
                0.33m,
                default(int?),
                false,
                "kitten"
            },
            new object?[]
            {
                new[]
                {
                    "kitten",
                    "Sitting",
                    "mittens"
                },
                "sitteng",
                0.6m,
                0.33m,
                default(int?),
                true,
                "kitten"
            },
            new object?[]
            {
                new[]
                {
                    "kitten",
                    "Sitting",
                    "mittens"
                },
                "sitteng",
                0.6m,
                0.33m,
                default(int?),
                false,
                "Sitting"
            },
            new object?[]
            {
                new[]
                {
                    "Written",
                    "writing",
                    "kitten"
                },
                "ritten",
                0.6m,
                0.33m,
                default(int?),
                true,
                "Written"
            },
            new object?[]
            {
                new[]
                {
                    "Written",
                    "writing",
                    "kitten"
                },
                "ritten",
                0.6m,
                0.33m,
                default(int?),
                false,
                "Written"
            },
            new object?[]
            {
                new[]
                {
                    "apple",
                    "banana",
                    "cherry"
                },
                "peach",
                0,
                1,
                default(int?),
                true,
                "cherry"
            },
            new object?[]
            {
                new[]
                {
                    "apple",
                    "banana",
                    "cherry"
                },
                "peach",
                0,
                1,
                default(int?),
                true,
                "cherry"
            },
            new object?[]
            {
                Array.Empty<string>(),
                "peach",
                0,
                1,
                default(int?),
                true,
                default(string)
            }
        };

    //@formatter:off
    [Theory]
    [InlineData("night", "nacht", true, 0.25)]
    [InlineData("night", "nacht", false, 0.25)]
    [InlineData("context", "contact", true, 0.5)]
    [InlineData("context", "contact", false, 0.5)]
    [InlineData("Context", "contact", true, 0.3333)]
    [InlineData("Context", "contact", false, 0.5)]
    [InlineData("Stick", "sticks", true, 0.6667)]
    [InlineData("Stick", "sticks", false, 0.8889)]
    [InlineData("sticks", "Stick", true, 0.6667)]
    [InlineData("sticks", "Stick", false, 0.8889)]
    [InlineData("", "", true, 0)]
    [InlineData("", "", false, 0)]
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
    [Theory]
    [InlineData("kitten", "sitting", true, 3)]
    [InlineData("kitten", "sitting", false, 3)]
    [InlineData("Kitten", "sitting", true, 3)]
    [InlineData("Kitten", "sitting", false, 3)]
    [InlineData("Saturday", "Sunday", true, 3)]
    [InlineData("Saturday", "Sunday", false, 3)]
    [InlineData("Saturday", "SUNDAY", true, 7)]
    [InlineData("Saturday", "SUNDAY", false, 3)]
    [InlineData("", "", true, 0)]
    [InlineData("", "", false, 0)]
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

    [Fact]
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

    [Fact]
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

    [Fact]
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

    [Fact]
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

    [Fact]
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

    [Fact]
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

    [Fact]
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

    [Fact]
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

    [Fact]
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

    [Fact]
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

    [Fact]
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

    [Fact]
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

    [Theory]
    [InlineData("Hello\r\nWorld", "Hello\nWorld")]
    [InlineData("Hello\nWorld", "Hello\nWorld")]
    [InlineData("Hello\rWorld", "Hello\nWorld")]
    [InlineData("Hello\r\n\r\nWorld", "Hello\n\nWorld")]
    [InlineData("Hello\r\nWorld\n", "Hello\nWorld")]
    [InlineData("Hello\r\nWorld\r\n", "Hello\nWorld")]
    public void FixLineEndings_ShouldReplaceLineEndingsCorrectly(string input, string expectedOutput)
    {
        // Act
        var result = input.FixLineEndings();

        // Assert
        result.Should()
              .Be(expectedOutput);
    }

    [Theory]
    [InlineData(
        new[]
        {
            "Hello World",
            "Hi world",
            "Greetings, World!"
        },
        "Helo World",
        false,
        true)]
    [InlineData(
        new[]
        {
            "apple",
            "banana",
            "cherry"
        },
        "Hello World",
        false,
        false)]
    [InlineData(
        new[]
        {
            "Hello World",
            "HELLO WORLD",
            "hello world"
        },
        "HELLO World",
        true,
        false)]
    [InlineData(
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

    [Theory]
    [MemberData(nameof(FuzzySearchTestData))]
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

    [Theory]
    [MemberData(nameof(FuzzySearchTestData))]
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

    [Fact]
    public void Inject_MissingParameters_ThrowsArgumentException()
    {
        // Arrange
        const string INPUT = "Hello, {One} and {Two}!";

        // Act and Assert
        INPUT.Invoking(x => x.Inject("World"))
             .Should()
             .Throw<ArgumentException>();
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
        result.Should()
              .Be(EXPECTED);
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
        result.Should()
              .Be(EXPECTED);
    }

    [Fact]
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

    [Fact]
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
        result.Should()
              .Be("Hello World"); // Expected result: "Hello World"
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
        result.Should()
              .Be("Hi Hi World"); // Expected result: "Hi Hi World"
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
        result.Should()
              .BeFalse(); // Expected result: false
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
        result.Should()
              .BeTrue(); // Expected result: true
    }
}