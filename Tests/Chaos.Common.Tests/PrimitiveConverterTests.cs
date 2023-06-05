using Chaos.Common.Converters;
using Chaos.Testing.Infrastructure.Definitions;
using FluentAssertions;
using Xunit;

// ReSharper disable UnusedMember.Local

// ReSharper disable ArrangeAttributes

namespace Chaos.Common.Tests;

public sealed class PrimitiveConverterTests
{
    [Fact]
    public void Convert_ShouldConvertEnumStringValueToEnumType()
    {
        // Arrange
        const string VALUE = "Red";
        var targetType = typeof(ColorEnum);
        const ColorEnum EXPECTED = ColorEnum.Red;

        // Act
        var result = PrimitiveConverter.Convert(targetType, VALUE);

        // Assert
        result.Should().Be(EXPECTED);
    }

    [Fact]
    public void Convert_ShouldConvertStringToParsedType()
    {
        // Arrange
        const string VALUE = "10";
        const int EXPECTED = 10;

        // Act
        var result = PrimitiveConverter.Convert<int>(VALUE);

        // Assert
        result.Should().Be(EXPECTED);
    }

    //@formatter:off
    [Theory]
    [InlineData(10, typeof(int))]
    [InlineData("123", typeof(int))]
    [InlineData("true", typeof(bool))]
    [InlineData("3.14", typeof(double))]
    //@formatter:on
    public void Convert_ShouldConvertValueToSpecifiedType(object value, Type targetType)
    {
        // Act
        var result = PrimitiveConverter.Convert(targetType, value);

        // Assert
        result.Should().BeOfType(targetType);
    }
}