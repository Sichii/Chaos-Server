#region
using Chaos.Collections.Common;
using FluentAssertions;
#endregion

namespace Chaos.Common.Tests;

public sealed class ArgumentCollectionTests
{
    [Test]
    public void Add_WithArguments_ShouldAddArgumentsToCollection()
    {
        // Arrange
        var argumentCollection = new ArgumentCollection();

        // Act
        argumentCollection.Add(
            [
                "arg1",
                "arg2",
                "arg3"
            ]);

        // Assert
        argumentCollection.Should()
                          .Equal("arg1", "arg2", "arg3");
    }

    [Test]
    public void Add_WithString_ShouldParseArgumentsFromSpaceDelimitedStringAndAddToCollection()
    {
        // Arrange
        var argumentCollection = new ArgumentCollection
        {
            // Act
            "arg1 arg2 arg3"
        };

        // Assert
        argumentCollection.Should()
                          .Equal("arg1", "arg2", "arg3");
    }

    [Test]
    public void Add_WithStringAndDelimiter_ShouldSplitStringIntoArgumentsAndAddToCollection()
    {
        // Arrange
        // Act
        var argumentCollection = new ArgumentCollection("arg1,arg2,arg3", ",");

        // Assert
        argumentCollection.Should()
                          .Equal("arg1", "arg2", "arg3");
    }

    // Argument at index is empty
    [Test]
    public void Argument_At_Index_Is_Empty()
    {
        // Arrange
        var arguments = new ArgumentCollection(
            [
                "",
                "2",
                "3"
            ]);

        // Act
        var result = arguments.TryGet<string>(0, out var value);

        // Assert
        result.Should()
              .BeTrue();

        value.Should()
             .BeEmpty();
    }

    // Arguments list is empty
    [Test]
    public void Arguments_List_Is_Empty()
    {
        // Arrange
        var arguments = new ArgumentCollection();

        // Act
        var result = arguments.TryGet<int>(0, out var value);

        // Assert
        result.Should()
              .BeFalse();

        value.Should()
             .Be(default);
    }

    [Test]
    public void Constructor_WithArguments_ShouldInitializeCollectionWithArguments()
    {
        // Arrange
        var arguments = new List<string>
        {
            "arg1",
            "arg2",
            "arg3"
        };

        // Act
        var argumentCollection = new ArgumentCollection(arguments);

        // Assert
        argumentCollection.Should()
                          .Equal(arguments);
    }

    [Test]
    public void Constructor_WithDelimiter_ShouldSplitStringsIntoArguments()
    {
        // Arrange
        const string ARGUMENT_STR = "arg1,arg2,arg3";
        const string DELIMITER = ",";

        // Act
        var argumentCollection = new ArgumentCollection(ARGUMENT_STR, DELIMITER);

        // Assert
        argumentCollection.Should()
                          .Equal("arg1", "arg2", "arg3");
    }

    [Test]
    public void Constructor_WithString_ShouldParseArgumentsFromSpaceDelimitedString()
    {
        // Arrange
        const string ARGUMENT_STR = "arg1 arg2 arg3";

        // Act
        var argumentCollection = new ArgumentCollection(ARGUMENT_STR);

        // Assert
        argumentCollection.Should()
                          .Equal("arg1", "arg2", "arg3");
    }

    [Test]
    public void Count_ShouldReturnNumberOfArguments()
    {
        // Arrange
        var argumentCollection = new ArgumentCollection("arg1 arg2 arg3");

        // Act
        var count = argumentCollection.Count;

        // Assert
        count.Should()
             .Be(3);
    }

    [Test]
    public void Handles_Invalid_ValueType_Conversion_Gracefully()
    {
        // Arrange
        var argumentCollection = new ArgumentCollection(
            new List<string>
            {
                "abc"
            });

        // Act
        var result = argumentCollection.TryGet<int>(0, out var value);

        // Assert
        result.Should()
              .BeFalse();

        value.Should()
             .Be(default);
    }

    [Test]
    public void Handles_Null_Conversion_Type_Gracefully()
    {
        // Arrange
        var argumentCollection = new ArgumentCollection(
            new List<string>
            {
                "abc"
            });

        // Act
        var result = argumentCollection.TryGet<int?>(0, out _);

        // Assert
        result.Should()
              .BeFalse();
    }

    [Test]
    public void Handles_Nullable_Primitive_Conversion_Gracefully()
    {
        // Arrange
        var argumentCollection = new ArgumentCollection(
            new List<string>
            {
                "abc",
                "5"
            });

        // Act
        var result = argumentCollection.TryGet<int?>(0, out var value1);
        var result2 = argumentCollection.TryGet<int?>(1, out var value2);

        // Assert
        result.Should()
              .BeFalse();

        result2.Should()
               .BeTrue();

        value1.Should()
              .BeNull();

        value2.Should()
              .Be(5);
    }

    // Index is negative
    [Test]
    public void Index_Is_Negative()
    {
        // Arrange
        var arguments = new ArgumentCollection(
            [
                "1",
                "2",
                "3"
            ]);

        // Act
        var result = arguments.TryGet<int>(-1, out var value);

        // Assert
        result.Should()
              .BeFalse();

        value.Should()
             .Be(default);
    }

    // properly handles arguments in double quotes
    [Test]
    public void Properly_Handles_Arguments_In_Double_Quotes()
    {
        // Arrange
        var arguments = new ArgumentCollection("arg1 \"arg2\" arg3");

        // Act
        var result = arguments.TryGet<string>(1, out var value);

        // Assert
        result.Should()
              .BeTrue();

        value.Should()
             .Be("arg2");
    }

    // properly handles arguments with spaces in double quotes
    [Test]
    public void Properly_Handles_Arguments_With_Spaces_In_Double_Quotes()
    {
        // Arrange
        var arguments = new ArgumentCollection("arg1 \"arg2 arg3\" arg4");

        // Act
        var result = arguments.TryGet<string>(1, out var value);

        // Assert
        result.Should()
              .BeTrue();

        value.Should()
             .Be("arg2 arg3");
    }

    // Returns false if argument cannot be converted to specified type
    [Test]
    public void Returns_False_If_Argument_Cannot_Be_Converted_To_Specified_Type()
    {
        // Arrange
        var arguments = new ArgumentCollection(
            [
                "1",
                "2",
                "3"
            ]);

        // Act
        var result = arguments.TryGet<bool>(1, out var value);

        // Assert
        result.Should()
              .BeFalse();

        value.Should()
             .Be(default);
    }

    // Returns false if index is out of range
    [Test]
    public void Returns_False_If_Index_Is_Out_Of_Range()
    {
        // Arrange
        var arguments = new ArgumentCollection(
            [
                "1",
                "2",
                "3"
            ]);

        // Act
        var result = arguments.TryGet<int>(3, out var value);

        // Assert
        result.Should()
              .BeFalse();

        value.Should()
             .Be(default);
    }

    // Returns true if argument exists at given index and is convertible to specified type
    [Test]
    public void Returns_True_If_Argument_Exists_At_Given_Index_And_Is_Convertible_To_Specified_Type()
    {
        // Arrange
        var arguments = new ArgumentCollection(
            [
                "1",
                "2",
                "3"
            ]);

        // Act
        var result = arguments.TryGet<int>(1, out var value);

        // Assert
        result.Should()
              .BeTrue();

        value.Should()
             .Be(2);
    }

    // Successfully retrieves argument at given index and converts to specified type
    [Test]
    public void Successfully_Retrieves_Argument_At_Given_Index_And_Converts_To_Specified_Type()
    {
        // Arrange
        var arguments = new ArgumentCollection(
            [
                "1",
                "2",
                "3"
            ]);

        // Act
        var result = arguments.TryGet<int>(1, out var value);

        // Assert
        result.Should()
              .BeTrue();

        value.Should()
             .Be(2);
    }

    // Supports conversion to ArgumentCollection type
    [Test]
    public void Supports_Conversion_To_ArgumentCollection_Type()
    {
        // Arrange
        var arguments = new ArgumentCollection(
            [
                "1",
                "2",
                "3"
            ]);

        // Act
        var result = arguments.TryGet<ArgumentCollection>(0, out var value);

        // Assert
        result.Should()
              .BeTrue();

        value.Should()
             .BeEquivalentTo(arguments);
    }

    [Test]
    public void ToString_ShouldReturnStringRepresentationOfArguments()
    {
        // Arrange
        var argumentCollection = new ArgumentCollection("arg1 arg2 arg3");

        // Act
        var result = argumentCollection.ToString();

        // Assert
        result.Should()
              .Be("\"arg1\" \"arg2\" \"arg3\" ");
    }
}