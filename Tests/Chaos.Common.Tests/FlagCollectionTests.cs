using Chaos.Collections.Common;
using Chaos.Testing.Infrastructure.Definitions;
using FluentAssertions;
using Xunit;

// ReSharper disable UnusedMember.Local

namespace Chaos.Common.Tests;

public sealed class FlagCollectionTests
{
    [Fact]
    public void AddFlag_ShouldAddFlagToExistingFlagOfTheSameType_WhenTypeIsFlagEnum()
    {
        // Arrange
        var collection = new FlagCollection();
        const SampleFlag1 INITIAL_FLAG = SampleFlag1.Value1;
        const SampleFlag1 FLAG_TO_ADD = SampleFlag1.Value2;
        collection.AddFlag(INITIAL_FLAG);

        // Act
        collection.AddFlag(FLAG_TO_ADD);

        // Assert
        collection.HasFlag(INITIAL_FLAG).Should().BeTrue();
        collection.HasFlag(FLAG_TO_ADD).Should().BeTrue();
    }

    [Fact]
    public void AddFlag_ShouldSetFlag_WhenTypeIsFlagEnumAndFlagDoesNotExist()
    {
        // Arrange
        var collection = new FlagCollection();
        var flag = SampleFlag1.Value1;

        // Act
        collection.AddFlag(flag);

        // Assert
        collection.HasFlag(flag).Should().BeTrue();
    }

    [Fact]
    public void AddFlag_ShouldThrowException_WhenTypeIsNotFlagEnum()
    {
        // Arrange
        var collection = new FlagCollection();
        var enumValue = SampleEnum1.Value1;

        // Act
        var act = () => collection.AddFlag(enumValue);

        // Assert
        act.Should()
           .Throw<InvalidOperationException>()
           .WithMessage($"Enum of type {typeof(SampleEnum1).FullName} is not a flag enum. Use the enum collection.");
    }

    [Fact]
    public void GetFlag_ShouldReturnFlagValue_WhenTypeIsFlagEnumAndFlagExists()
    {
        // Arrange
        var collection = new FlagCollection();
        var flag = SampleFlag1.Value1;
        collection.AddFlag(flag);

        // Act
        var result = collection.GetFlag<SampleFlag1>();

        // Assert
        result.Should().Be(flag);
    }

    [Fact]
    public void GetFlag_ShouldThrowException_WhenFlagDoesNotExist()
    {
        // Arrange
        var collection = new FlagCollection();

        // Act
        Action act = () => collection.GetFlag<SampleFlag1>();

        // Assert
        act.Should()
           .Throw<KeyNotFoundException>()
           .WithMessage($"Enum of type {typeof(SampleFlag1).FullName} was not found in the collection");
    }

    [Fact]
    public void GetFlag_ShouldThrowException_WhenTypeIsNotFlagEnum()
    {
        // Arrange
        var collection = new FlagCollection();

        // Act
        Action act = () => collection.GetFlag<SampleEnum1>();

        // Assert
        act.Should()
           .Throw<InvalidOperationException>()
           .WithMessage($"Enum of type {typeof(SampleEnum1).FullName} is not a flag enum. Use the enum collection.");
    }

    [Fact]
    public void HasFlag_ShouldReturnFalse_WhenTypeIsFlagEnumAndFlagDoesNotExist()
    {
        // Arrange
        var collection = new FlagCollection();
        var flag = SampleFlag1.Value1;

        // Act
        var result = collection.HasFlag(flag);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void HasFlag_ShouldReturnTrue_WhenTypeIsFlagEnumAndFlagExists()
    {
        // Arrange
        var collection = new FlagCollection();
        var flag = SampleFlag1.Value1;
        collection.AddFlag(flag);

        // Act
        var result = collection.HasFlag(flag);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void HasFlag_ShouldThrowException_WhenTypeIsNotFlagEnum()
    {
        // Arrange
        var collection = new FlagCollection();
        var enumValue = SampleEnum1.Value1;

        // Act
        Action act = () => collection.HasFlag(enumValue);

        // Assert
        act.Should()
           .Throw<InvalidOperationException>()
           .WithMessage($"Enum of type {typeof(SampleEnum1).FullName} is not a flag enum. Use the enum collection.");
    }

    [Fact]
    public void RemoveFlag_ShouldNotThrowException_WhenTypeIsNotFlagEnum()
    {
        // Arrange
        var collection = new FlagCollection();
        var enumValue = SampleEnum1.Value1;

        // Act
        var act = () => collection.RemoveFlag(enumValue);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void RemoveFlag_ShouldRemoveFlagFromExistingFlagOfTheSameType_WhenTypeIsFlagEnum()
    {
        // Arrange
        var collection = new FlagCollection();
        var initialFlag = SampleFlag1.Value1 | SampleFlag1.Value2;
        var flagToRemove = SampleFlag1.Value2;
        collection.AddFlag(initialFlag);

        // Act
        collection.RemoveFlag(flagToRemove);

        // Assert
        collection.HasFlag(SampleFlag1.Value1).Should().BeTrue();
        collection.HasFlag(initialFlag).Should().BeFalse();
        collection.HasFlag(flagToRemove).Should().BeFalse();
    }

    [Fact]
    public void TryGetFlag_ShouldReturnFalse_WhenTypeIsFlagEnumAndFlagDoesNotExist()
    {
        // Arrange
        var collection = new FlagCollection();

        // Act
        var result = collection.TryGetFlag<SampleFlag1>(out _);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void TryGetFlag_ShouldReturnTrueAndSetFlagValue_WhenTypeIsFlagEnumAndFlagExists()
    {
        // Arrange
        var collection = new FlagCollection();
        var flag = SampleFlag1.Value1;
        collection.AddFlag(flag);

        // Act
        var result = collection.TryGetFlag<SampleFlag1>(out var value);

        // Assert
        result.Should().BeTrue();
        value.Should().Be(flag);
    }

    [Fact]
    public void TryGetFlag_ShouldThrowException_WhenTypeIsNotFlagEnum()
    {
        // Arrange
        var collection = new FlagCollection();

        // Act
        Action act = () => collection.TryGetFlag<SampleEnum1>(out _);

        // Assert
        act.Should()
           .Throw<InvalidOperationException>()
           .WithMessage($"Enum of type {typeof(SampleEnum1).FullName} is not a flag enum. Use the enum collection.");
    }
}