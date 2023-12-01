namespace Chaos.Extensions.Common.Tests;

public sealed class EnumerableExtensionsTests
{
    [Fact]
    public void ContainsI_Should_Return_False_When_Sequence_Does_Not_Contain_String()
    {
        // Arrange
        var enumerable = new[]
        {
            "apple",
            "banana",
            "cherry"
        };
        const string STR = "grape";

        // Act
        var result = enumerable.ContainsI(STR);

        // Assert
        result.Should()
              .BeFalse();
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
        result.Should()
              .BeFalse();
    }

    [Fact]
    public void ContainsI_Should_Return_True_When_Sequence_Contains_String()
    {
        // Arrange
        var enumerable = new[]
        {
            "apple",
            "banana",
            "cherry"
        };
        const string STR = "BaNaNa";

        // Act
        var result = enumerable.ContainsI(STR);

        // Assert
        result.Should()
              .BeTrue();
    }

    [Fact]
    public void IsNullOrEmpty_Should_Return_False_When_Sequence_Is_Not_Null_Or_Empty()
    {
        // Arrange
        var enumerable = new[]
        {
            1,
            2,
            3
        };

        // Act
        var result = enumerable.IsNullOrEmpty();

        // Assert
        result.Should()
              .BeFalse();
    }

    [Fact]
    public void IsNullOrEmpty_Should_Return_True_When_Sequence_Is_Empty()
    {
        // Arrange
        var enumerable = Enumerable.Empty<int>();

        // Act
        var result = enumerable.IsNullOrEmpty();

        // Assert
        result.Should()
              .BeTrue();
    }

    [Fact]
    public void IsNullOrEmpty_Should_Return_True_When_Sequence_Is_Null()
    {
        // Arrange
        IEnumerable<int>? enumerable = null;

        // Act
        var result = enumerable.IsNullOrEmpty();

        // Assert
        result.Should()
              .BeTrue();
    }

    // ReSharper disable once ArrangeAttributes
    [Theory]
    [InlineData(
        new[]
        {
            3,
            8,
            5,
            2,
            6,
            1,
            9
        },
        5,
        6)]
    [InlineData(
        new[]
        {
            3,
            2,
            1
        },
        3,
        3)]
    public void NextHighest_ShouldReturnExpectedResult(int[] numbers, int seed, int expected)
    {
        // Arrange
        var listNumbers = numbers.ToList();

        // Act
        var result = listNumbers.NextHighest(seed);

        // Assert
        result.Should()
              .Be(expected);
    }

    // ReSharper disable once ArrangeAttributes
    [Theory]
    [InlineData(
        new[]
        {
            3,
            8,
            5,
            2,
            6,
            1,
            9
        },
        5,
        3)]
    [InlineData(
        new[]
        {
            3,
            2,
            1
        },
        3,
        2)]
    public void NextLowest_ShouldReturnExpectedResult(int[] numbers, int seed, int expected)
    {
        // Arrange
        var listNumbers = numbers.ToList();

        // Act
        var result = listNumbers.NextLowest(seed);

        // Assert
        result.Should()
              .Be(expected);
    }
}