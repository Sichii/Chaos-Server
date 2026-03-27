#region
using System.Globalization;
using Chaos.Common.Converters;
using FluentAssertions;
#endregion

namespace Chaos.Common.Tests;

public sealed class PrimitiveConverterTests
{
    [Test]
    public void Convert_Generic_ShouldThrowForInvalidConversion()
    {
        // Arrange
        object value = "invalid_number";

        // Act & Assert
        var act = () => PrimitiveConverter.Convert<int>(value);

        act.Should()
           .Throw<FormatException>();
    }

    [Test]
    public void Convert_Generic_WithObject_ShouldConvertStringToInt()
    {
        // Arrange
        object value = "123";

        // Act
        var result = PrimitiveConverter.Convert<int>(value);

        // Assert
        result.Should()
              .Be(123);
    }

    [Test]
    public void Convert_Generic_WithObject_ShouldConvertToTargetType()
    {
        // Arrange
        object value = 42;

        // Act
        var result = PrimitiveConverter.Convert<int>(value);

        // Assert
        result.Should()
              .Be(42);
    }

    [Test]
    public void Convert_Generic_WithString_ShouldParseValue()
    {
        // Arrange
        var str = "42";

        // Act
        var result = PrimitiveConverter.Convert<int>(str);

        // Assert
        result.Should()
              .Be(42);
    }

    [Test]
    public void Convert_Generic_WithString_ShouldParseValueWithFormatProvider()
    {
        // Arrange
        var str = "3.14159";
        var formatProvider = CultureInfo.InvariantCulture;

        // Act
        var result = PrimitiveConverter.Convert<double>(str, formatProvider);

        // Assert
        result.Should()
              .Be(3.14159);
    }

    [Test]
    public void Convert_Generic_WithString_ShouldParseValueWithNullFormatProvider()
    {
        // Arrange
        var str = "42";

        // Act
        var result = PrimitiveConverter.Convert<int>(str);

        // Assert
        result.Should()
              .Be(42);
    }

    [Test]
    public void Convert_Generic_WithString_ShouldThrowForInvalidFormat()
    {
        // Arrange
        var str = "invalid_number";

        // Act & Assert
        var act = () => PrimitiveConverter.Convert<int>(str);

        act.Should()
           .Throw<FormatException>();
    }

    [Test]
    public void Convert_WithType_ShouldConvertBoolFromString()
    {
        // Arrange
        var type = typeof(bool);
        object value = "true";

        // Act
        var result = PrimitiveConverter.Convert(type, value);

        // Assert
        result.Should()
              .Be(true);

        result.Should()
              .BeOfType<bool>();
    }

    [Test]
    public void Convert_WithType_ShouldConvertDateTimeFromString()
    {
        // Arrange
        var type = typeof(DateTime);
        object value = "2025-01-01";

        // Act
        var result = PrimitiveConverter.Convert(type, value);

        // Assert
        result.Should()
              .Be(new DateTime(2025, 1, 1));

        result.Should()
              .BeOfType<DateTime>();
    }

    [Test]
    public void Convert_WithType_ShouldConvertPrimitiveTypes()
    {
        // Arrange
        var type = typeof(double);
        object value = 42;

        // Act
        var result = PrimitiveConverter.Convert(type, value);

        // Assert
        result.Should()
              .Be(42.0);

        result.Should()
              .BeOfType<double>();
    }

    [Test]
    public void Convert_WithType_ShouldConvertToNullableType()
    {
        // Arrange
        var type = typeof(int?);
        object value = "42";

        // Act
        var result = PrimitiveConverter.Convert(type, value);

        // Assert
        result.Should()
              .Be(42);

        result.Should()
              .BeOfType<int>();
    }

    [Test]
    public void Convert_WithType_ShouldHandleComplexConversions()
    {
        // Arrange & Act & Assert
        PrimitiveConverter.Convert(typeof(long), 42)
                          .Should()
                          .Be(42L);

        PrimitiveConverter.Convert(typeof(float), "3.14")
                          .Should()
                          .Be(3.14f);

        PrimitiveConverter.Convert(typeof(decimal), 100)
                          .Should()
                          .Be(100m);

        PrimitiveConverter.Convert(typeof(byte), "255")
                          .Should()
                          .Be((byte)255);

        PrimitiveConverter.Convert(typeof(short), 1000)
                          .Should()
                          .Be((short)1000);
    }

    [Test]
    public void Convert_WithType_ShouldHandleNullableEnumTypes()
    {
        // Arrange
        var type = typeof(TestEnum?);
        object value = "Value2";

        // Act
        var result = PrimitiveConverter.Convert(type, value);

        // Assert
        result.Should()
              .Be(TestEnum.Value2);
    }

    [Test]
    public void Convert_WithType_ShouldHandleNullablePrimitiveTypes()
    {
        // Arrange
        var type = typeof(double?);
        object value = "42.5";

        // Act
        var result = PrimitiveConverter.Convert(type, value);

        // Assert
        result.Should()
              .Be(42.5);
    }

    [Test]
    public void Convert_WithType_ShouldParseEnumCaseSensitive()
    {
        // Arrange
        var type = typeof(TestEnum);
        object value = "TestValue";

        // Act
        var result = PrimitiveConverter.Convert(type, value);

        // Assert
        result.Should()
              .Be(TestEnum.TestValue);
    }

    [Test]
    public void Convert_WithType_ShouldParseEnumIgnoreCase()
    {
        // Arrange
        var type = typeof(TestEnum);
        object value = "value1";

        // Act
        var result = PrimitiveConverter.Convert(type, value);

        // Assert
        result.Should()
              .Be(TestEnum.Value1);
    }

    [Test]
    public void Convert_WithType_ShouldReturnSameStringWhenTargetIsString()
    {
        // Arrange
        var type = typeof(string);
        object value = "test string";

        // Act
        var result = PrimitiveConverter.Convert(type, value);

        // Assert
        result.Should()
              .Be("test string");

        result.Should()
              .BeOfType<string>();
    }

    [Test]
    public void Convert_WithType_ShouldThrowForInvalidEnumValue()
    {
        // Arrange
        var type = typeof(TestEnum);
        object value = "InvalidEnumValue";

        // Act & Assert
        var act = () => PrimitiveConverter.Convert(type, value);

        act.Should()
           .Throw<ArgumentException>();
    }

    [Test]
    public void TryConvert_Generic_WithObject_Null_ReturnsDefault()
    {
        object? value = null;
        var res = PrimitiveConverter.TryConvert<int>(value);

        res.Should()
           .Be(0);
    }

    [Test]
    public void TryConvert_Generic_WithObject_ShouldConvertObjectToSameType()
    {
        // Arrange
        object value = 42;

        // Act
        var result = PrimitiveConverter.TryConvert<int>(value);

        // Assert
        result.Should()
              .Be(42);
    }

    [Test]
    public void TryConvert_Generic_WithObject_ShouldConvertValidValue()
    {
        // Arrange
        object value = "42";

        // Act
        var result = PrimitiveConverter.TryConvert<int>(value);

        // Assert
        result.Should()
              .Be(42);
    }

    [Test]
    public void TryConvert_Generic_WithObject_ShouldHandleNullableTypes()
    {
        // Arrange
        object value = 42;

        // Act
        var result = PrimitiveConverter.TryConvert<int?>(value);

        // Assert
        result.Should()
              .Be(42);
    }

    [Test]
    public void TryConvert_Generic_WithObject_ShouldReturnDefaultForNull()
    {
        // Arrange
        object? value = null;

        // Act
        var result = PrimitiveConverter.TryConvert<int>(value);

        // Assert
        result.Should()
              .Be(0);
    }

    [Test]
    public void TryConvert_Generic_WithObject_ShouldThrowForInvalidConversion()
    {
        // Arrange
        object value = "invalid_number";

        // Act & Assert
        var act = () => PrimitiveConverter.TryConvert<int>(value);

        act.Should()
           .Throw<FormatException>();
    }

    [Test]
    public void TryConvert_Generic_WithString_Empty_ReturnsDefault()
    {
        var res = PrimitiveConverter.TryConvert<int>("");

        res.Should()
           .Be(0);
    }

    [Test]
    public void TryConvert_Generic_WithString_ShouldHandleNullableTypes()
    {
        // Arrange
        var str = "42";

        // Act
        var result = PrimitiveConverter.TryConvert<int?>(str);

        // Assert
        result.Should()
              .Be(42);
    }

    [Test]
    public void TryConvert_Generic_WithString_ShouldParseValidValue()
    {
        // Arrange
        var str = "42";

        // Act
        var result = PrimitiveConverter.TryConvert<int>(str);

        // Assert
        result.Should()
              .Be(42);
    }

    [Test]
    public void TryConvert_Generic_WithString_ShouldParseValidValueWithFormatProvider()
    {
        // Arrange
        var str = "3.14159";
        var formatProvider = CultureInfo.InvariantCulture;

        // Act
        var result = PrimitiveConverter.TryConvert<double>(str, formatProvider);

        // Assert
        result.Should()
              .Be(3.14159);
    }

    [Test]
    public void TryConvert_Generic_WithString_ShouldReturnDefaultForNullOrEmptyString()
    {
        // Arrange & Act
        var resultNull = PrimitiveConverter.TryConvert<int>(null!);
        var resultEmpty = PrimitiveConverter.TryConvert<int>("");
        var exception = () => PrimitiveConverter.TryConvert<int>("   ");

        // Assert
        resultNull.Should()
                  .Be(0);

        resultEmpty.Should()
                   .Be(0);

        exception.Should()
                 .Throw<FormatException>();
    }

    [Test]
    public void TryConvert_Generic_WithString_ShouldThrowForInvalidFormat()
    {
        // Arrange
        var str = "invalid_number";

        // Act & Assert
        var act = () => PrimitiveConverter.TryConvert<int>(str);

        act.Should()
           .Throw<FormatException>();
    }

    private enum TestEnum
    {
        Value1,
        Value2,
        TestValue
    }
}