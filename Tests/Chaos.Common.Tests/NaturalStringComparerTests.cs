#region
using Chaos.Common.Comparers;
using FluentAssertions;
#endregion

namespace Chaos.Common.Tests;

public sealed class NaturalStringComparerTests
{
    private readonly NaturalStringComparer _comparer = new();

    [Test]
    public void Compare_AllZerosInY_ShouldExhaustLeadingZeroLoop()
    {
        // Arrange & Act - y has all-zero numeric portion at end of string
        // This tests the iy >= y.Length exit condition in the leading zeros loop (line 73)
        var result = _comparer.Compare("file1", "file0");

        // Assert
        result.Should()
              .BePositive(); // 1 > 0
    }

    [Test]
    public void Compare_AllZerosNumericPortion_ShouldExhaustLeadingZeroLoop()
    {
        // Arrange & Act - "file0" has all-zero numeric portion at end of string
        // This tests the ix >= x.Length exit condition in the leading zeros loop (line 69)
        var result = _comparer.Compare("file0", "file1");

        // Assert
        result.Should()
              .BeNegative(); // 0 < 1
    }

    [Test]
    public void Compare_AllZerosVsAllZeros_ShouldReturnZero()
    {
        // Arrange & Act - both strings have all-zeros numeric portions
        var result = _comparer.Compare("item000", "item00");

        // Assert
        result.Should()
              .Be(0); // both are numerically 0
    }

    [Test]
    public void Compare_AlphabeticStrings_ShouldCompareAlphabetically()
    {
        // Arrange & Act
        var result1 = _comparer.Compare("apple", "banana");
        var result2 = _comparer.Compare("zebra", "apple");

        // Assert
        result1.Should()
               .BeNegative();

        result2.Should()
               .BePositive();
    }

    [Test]
    public void Compare_BothStringsNull_ShouldReturnZero()
    {
        // Arrange & Act
        var result = _comparer.Compare(null, null);

        // Assert
        result.Should()
              .Be(0);
    }

    [Test]
    public void Compare_ComplexVersionNumbers_ShouldOrderCorrectly()
    {
        // Arrange
        var versions = new[]
        {
            "v1.10.5",
            "v1.2.3",
            "v1.9.10",
            "v2.1.0"
        };

        // Act
        var sorted = versions.OrderBy(v => v, _comparer)
                             .ToArray();

        // Assert
        sorted.Should()
              .Equal(
                  "v1.2.3",
                  "v1.9.10",
                  "v1.10.5",
                  "v2.1.0");
    }

    [Test]
    public void Compare_ConsecutiveDigits_ShouldTreatAsOneNumber()
    {
        // Arrange & Act
        var result = _comparer.Compare("test123abc", "test456def");

        // Assert
        result.Should()
              .BeNegative(); // 123 < 456
    }

    [Test]
    public void Compare_DifferentLeadingZeros_SameNumericValue_ShouldReturnZero()
    {
        // Arrange & Act - "007" and "7" should be equal since leading zeros are skipped
        var result = _comparer.Compare("file007", "file7");

        // Assert
        result.Should()
              .Be(0);
    }

    [Test]
    public void Compare_EmptyStrings_ShouldReturnZero()
    {
        // Arrange & Act
        var result = _comparer.Compare("", "");

        // Assert
        result.Should()
              .Be(0);
    }

    [Test]
    public void Compare_EqualNumericParts_ShouldContinueComparison()
    {
        // Arrange & Act
        var result1 = _comparer.Compare("file10a", "file10b");
        var result2 = _comparer.Compare("test5x", "test5y");

        // Assert
        result1.Should()
               .BeNegative();

        result2.Should()
               .BeNegative();
    }

    [Test]
    public void Compare_EqualNumericStrings_ShouldReturnZero()
    {
        // Arrange & Act
        var result = _comparer.Compare("100", "100");

        // Assert
        result.Should()
              .Be(0);
    }

    [Test]
    public void Compare_FirstStringNull_ShouldReturnNegativeOne()
    {
        // Arrange & Act
        var result = _comparer.Compare(null, "test");

        // Assert
        result.Should()
              .Be(-1);
    }

    [Test]
    public void Compare_IdenticalStrings_ShouldReturnZero()
    {
        // Arrange & Act
        var result = _comparer.Compare("test", "test");

        // Assert
        result.Should()
              .Be(0);
    }

    [Test]
    public void Compare_LargeNumbers_ShouldHandleCorrectly()
    {
        // Arrange & Act
        var result1 = _comparer.Compare("file999999999", "file1000000000");
        var result2 = _comparer.Compare("item1000000000", "item999999999");

        // Assert
        result1.Should()
               .BeNegative();

        result2.Should()
               .BePositive();
    }

    [Test]
    public void Compare_LeadingZeros_ShouldIgnoreLeadingZeros()
    {
        // Arrange & Act
        var result1 = _comparer.Compare("file005", "file5");
        var result2 = _comparer.Compare("item010", "item10");

        // Assert
        result1.Should()
               .Be(0);

        result2.Should()
               .Be(0);
    }

    [Test]
    public void Compare_MixedAlphaNumericStrings_ShouldCompareNumericPartsNumerically()
    {
        // Arrange & Act
        var result1 = _comparer.Compare("file5.txt", "file10.txt");
        var result2 = _comparer.Compare("item100", "item20");

        // Assert
        result1.Should()
               .BeNegative();

        result2.Should()
               .BePositive();
    }

    [Test]
    public void Compare_MixedCaseAlpha_ShouldRespectCaseSensitivity()
    {
        // Arrange & Act
        var result1 = _comparer.Compare("File", "file");
        var result2 = _comparer.Compare("TEST", "test");

        // Assert
        result1.Should()
               .BeNegative(); // 'F' < 'f' in ASCII

        result2.Should()
               .BeNegative(); // 'T' < 't' in ASCII
    }

    [Test]
    public void Compare_MultipleNumericParts_ShouldCompareEachNumerically()
    {
        // Arrange & Act
        var result1 = _comparer.Compare("v1.2.5", "v1.2.10");
        var result2 = _comparer.Compare("v2.1.1", "v1.9.9");

        // Assert
        result1.Should()
               .BeNegative();

        result2.Should()
               .BePositive();
    }

    [Test]
    public void Compare_NumberAtEnd_ShouldCompareNumerically()
    {
        // Arrange & Act
        var result1 = _comparer.Compare("abc5", "abc10");
        var result2 = _comparer.Compare("xyz100", "xyz20");

        // Assert
        result1.Should()
               .BeNegative();

        result2.Should()
               .BePositive();
    }

    [Test]
    public void Compare_NumberAtStart_ShouldCompareNumerically()
    {
        // Arrange & Act
        var result1 = _comparer.Compare("5abc", "10def");
        var result2 = _comparer.Compare("100xyz", "20uvw");

        // Assert
        result1.Should()
               .BeNegative();

        result2.Should()
               .BePositive();
    }

    [Test]
    public void Compare_NumberVersusLetter_ShouldCompareLexicographically()
    {
        // Arrange & Act
        var result1 = _comparer.Compare("5", "a");
        var result2 = _comparer.Compare("a", "5");

        // Assert
        result1.Should()
               .BeNegative(); // '5' (ASCII 53) < 'a' (ASCII 97)

        result2.Should()
               .BePositive();
    }

    [Test]
    public void Compare_NumericPartAtEndOfShorterString_ShouldCompareCorrectly()
    {
        // Arrange & Act
        var result = _comparer.Compare("abc5", "abc5def");

        // Assert
        result.Should()
              .BeNegative(); // shorter string comes first when prefix matches
    }

    [Test]
    public void Compare_NumericStrings_ShouldCompareNumerically()
    {
        // Arrange & Act
        var result1 = _comparer.Compare("5", "10");
        var result2 = _comparer.Compare("100", "20");

        // Assert
        result1.Should()
               .BeNegative();

        result2.Should()
               .BePositive();
    }

    [Test]
    public void Compare_OnlyNumbers_ShouldCompareNumerically()
    {
        // Arrange & Act
        var result1 = _comparer.Compare("5", "10");
        var result2 = _comparer.Compare("100", "99");

        // Assert
        result1.Should()
               .BeNegative();

        result2.Should()
               .BePositive();
    }

    [Test]
    public void Compare_PrefixMatch_ShouldCompareByLength()
    {
        // Arrange & Act
        var result1 = _comparer.Compare("test", "testing");
        var result2 = _comparer.Compare("testing", "test");

        // Assert
        result1.Should()
               .BeNegative();

        result2.Should()
               .BePositive();
    }

    [Test]
    public void Compare_SameReferenceString_ShouldReturnZero()
    {
        // Arrange
        var str = "same";

        // Act - same reference object
        var result = _comparer.Compare(str, str);

        // Assert
        result.Should()
              .Be(0);
    }

    [Test]
    public void Compare_SecondStringNull_ShouldReturnPositiveOne()
    {
        // Arrange & Act
        var result = _comparer.Compare("test", null);

        // Assert
        result.Should()
              .Be(1);
    }

    [Test]
    public void Compare_ShorterStringIsPrefix_NumericDifference_ShouldCompareByLength()
    {
        // Arrange & Act - "test" is exhausted before "test1"
        var result = _comparer.Compare("test", "test1");

        // Assert
        result.Should()
              .BeNegative();
    }

    [Test]
    public void Compare_SingleCharacterDifference_ShouldCompareCorrectly()
    {
        // Arrange & Act
        var result1 = _comparer.Compare("a", "b");
        var result2 = _comparer.Compare("z", "a");

        // Assert
        result1.Should()
               .BeNegative();

        result2.Should()
               .BePositive();
    }

    [Test]
    public void Compare_WindowsFileNamingPattern_ShouldOrderNaturally()
    {
        // Arrange
        var files = new[]
        {
            "file1.txt",
            "file10.txt",
            "file2.txt",
            "file20.txt"
        };

        // Act
        var sorted = files.OrderBy(f => f, _comparer)
                          .ToArray();

        // Assert
        sorted.Should()
              .Equal(
                  "file1.txt",
                  "file2.txt",
                  "file10.txt",
                  "file20.txt");
    }
}