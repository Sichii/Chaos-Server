#region
using Chaos.Extensions.Common;
using Chaos.Testing.Infrastructure.Definitions;
using FluentAssertions;
#endregion

// ReSharper disable ArrangeAttributes

namespace Chaos.Common.Tests;

public sealed class TypeExtensionsTests
{
    [Test]
    public void IsFlagEnum_ShouldReturnFalse_WhenTypeIsEnumWithoutFlagsAttribute()
    {
        // Arrange
        var enumType = typeof(SampleEnum1);

        // Act
        var result = enumType.IsFlagEnum();

        // Assert
        result.Should()
              .BeFalse();
    }

    [Test]
    public void IsFlagEnum_ShouldReturnFalse_WhenTypeIsNotEnum()
    {
        // Arrange
        var nonEnumType = typeof(string);

        // Act
        var result = nonEnumType.IsFlagEnum();

        // Assert
        result.Should()
              .BeFalse();
    }

    [Test]
    public void IsFlagEnum_ShouldReturnTrue_WhenTypeIsFlagEnum()
    {
        // Arrange
        var flagEnumType = typeof(SampleFlag1);

        // Act
        var result = flagEnumType.IsFlagEnum();

        // Assert
        result.Should()
              .BeTrue();
    }

    [Test]
    [Arguments(typeof(object))]
    [Arguments(typeof(DateTime))]
    [Arguments(typeof(Enum))]
    public void IsPrimitive_ShouldReturnFalse_WhenTypeIsNotPrimitiveValueType(Type type)
    {
        // Act
        var result = type.IsPrimitive();

        // Assert
        result.Should()
              .BeFalse();
    }

    [Test]
    [Arguments(typeof(int))]
    [Arguments(typeof(double))]
    [Arguments(typeof(decimal))]
    [Arguments(typeof(bool))]
    public void IsPrimitive_ShouldReturnTrue_WhenTypeIsPrimitiveValueType(Type type)
    {
        // Act
        var result = type.IsPrimitive();

        // Assert
        result.Should()
              .BeTrue();
    }

    [Test]
    public void IsPrimitive_ShouldReturnTrue_WhenTypeIsString()
    {
        // Arrange
        var stringType = typeof(string);

        // Act
        var result = stringType.IsPrimitive();

        // Assert
        result.Should()
              .BeTrue();
    }
}