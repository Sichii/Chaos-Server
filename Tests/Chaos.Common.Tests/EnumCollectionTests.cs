using Chaos.Collections.Common;
using Chaos.Testing.Infrastructure.Definitions;

// ReSharper disable UnusedMember.Local

namespace Chaos.Common.Tests;

public sealed class EnumCollectionTests
{
    [Fact]
    public void HasValue_ShouldReturnFalse_WhenEnumTypeNotInCollection()
    {
        var result = new EnumCollection().HasValue(typeof(SampleEnum1), SampleEnum1.Value2);

        result.Should()
              .BeFalse();
    }

    [Fact]
    public void HasValue_ShouldReturnFalse_WhenEnumValueDoesNotMatch()
    {
        // Arrange
        var collection = new EnumCollection();
        const SampleEnum1 ENUM_VALUE = SampleEnum1.Value1;
        collection.Set(ENUM_VALUE);

        var result = collection.HasValue(typeof(SampleEnum1), SampleEnum1.Value2);

        result.Should()
              .BeFalse();
    }

    [Fact]
    public void HasValue_ShouldReturnTrue_WhenEnumValueMatches()
    {
        // Arrange
        var collection = new EnumCollection();
        const SampleEnum1 ENUM_VALUE = SampleEnum1.Value1;
        collection.Set(ENUM_VALUE);

        var result = collection.HasValue(typeof(SampleEnum1), SampleEnum1.Value1);

        result.Should()
              .BeTrue();
    }

    [Fact]
    public void HasValue_ShouldThrowInvalidOperationException_ForFlagEnums()
    {
        Action action = () => new EnumCollection().HasValue(typeof(SampleFlag1), SampleFlag1.Value1);

        action.Should()
              .Throw<InvalidOperationException>()
              .WithMessage("*flag enum*");
    }

    [Fact]
    public void Remove_ShouldNotThrowException_WhenTypeDoesNotExist()
    {
        // Arrange
        var collection = new EnumCollection();

        // Act
        var act = () => collection.Remove<SampleEnum1>();

        // Assert
        act.Should()
           .NotThrow();
    }

    [Fact]
    public void Remove_ShouldRemoveEnum_WhenTypeExists()
    {
        // Arrange
        var collection = new EnumCollection();
        collection.Set(SampleEnum1.Value1);

        // Act
        collection.Remove<SampleEnum1>();

        // Assert
        collection.TryGetValue<SampleEnum1>(out _)
                  .Should()
                  .BeFalse();
    }

    [Fact]
    public void Set_ShouldSetEnumValue_WhenTypeIsNotFlagEnum()
    {
        // Arrange
        var collection = new EnumCollection();
        const SampleEnum1 ENUM_VALUE = SampleEnum1.Value1;

        // Act
        collection.Set(ENUM_VALUE);

        // Assert
        collection.TryGetValue<SampleEnum1>(out var value)
                  .Should()
                  .BeTrue();

        value.Should()
             .Be(ENUM_VALUE);
    }

    [Fact]
    public void Set_ShouldThrowException_WhenTypeIsFlagEnum()
    {
        // Arrange
        var collection = new EnumCollection();
        const SampleFlag1 FLAG_ENUM_VALUE = SampleFlag1.Value1 | SampleFlag1.Value2;

        // Act
        var act = () => collection.Set(FLAG_ENUM_VALUE);

        // Assert
        act.Should()
           .Throw<InvalidOperationException>()
           .WithMessage($"Enum of type {typeof(SampleFlag1).FullName} is a flag enum. Use the flag collection.");
    }

    [Fact]
    public void TryGetValue_ShouldReturnFalse_WhenTypeDoesNotExist()
    {
        // Arrange
        var collection = new EnumCollection();

        // Act
        var result = collection.TryGetValue<SampleEnum1>(out _);

        // Assert
        result.Should()
              .BeFalse();
    }

    [Fact]
    public void TryGetValue_ShouldReturnTrueAndSetValue_WhenTypeExists()
    {
        // Arrange
        var collection = new EnumCollection();
        const SampleEnum1 ENUM_VALUE = SampleEnum1.Value1;
        collection.Set(ENUM_VALUE);

        // Act
        var result = collection.TryGetValue<SampleEnum1>(out var value);

        // Assert
        result.Should()
              .BeTrue();

        value.Should()
             .Be(ENUM_VALUE);
    }
}