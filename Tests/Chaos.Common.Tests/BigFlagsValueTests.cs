#region
using System.Numerics;
using Chaos.Common.Abstractions;
using Chaos.Common.CustomTypes;
using Chaos.Testing.Infrastructure.Definitions;
using FluentAssertions;
#endregion

namespace Chaos.Common.Tests;

public sealed class BigFlagsValueTests
{
    [Test]
    public void AndOperator_ShouldIntersectFlags()
    {
        // Arrange
        var flag1 = TestFeatures.Feature1 | TestFeatures.Feature2;
        var flag2 = TestFeatures.Feature2 | TestFeatures.Feature3;

        // Act
        var result = flag1 & flag2;

        // Assert
        result.Should()
              .Be(TestFeatures.Feature2);
    }

    [Test]
    public void ClearFlag_WithBitIndex_ShouldClearCorrectBit()
    {
        // Arrange
        var flag = new BigFlagsValue<TestFeatures>((BigInteger)0b111); // Bits 0, 1, 2

        // Act
        var result = flag.ClearFlag(1);

        // Assert
        result.HasFlag(0)
              .Should()
              .BeTrue();

        result.HasFlag(1)
              .Should()
              .BeFalse();

        result.HasFlag(2)
              .Should()
              .BeTrue();
    }

    [Test]
    public void ClearFlag_WithFlagValue_ShouldRemoveSpecifiedFlags()
    {
        // Arrange
        var flag = TestFeatures.Feature1 | TestFeatures.Feature2 | TestFeatures.Feature3;

        // Act
        var result = flag.ClearFlag(TestFeatures.Feature2);

        // Assert
        result.HasFlag(TestFeatures.Feature1)
              .Should()
              .BeTrue();

        result.HasFlag(TestFeatures.Feature2)
              .Should()
              .BeFalse();

        result.HasFlag(TestFeatures.Feature3)
              .Should()
              .BeTrue();
    }

    [Test]
    public void Constructor_WithBigInteger_ShouldInitializeCorrectly()
    {
        // Arrange
        var value = new BigInteger(42);

        // Act
        var flag = new BigFlagsValue<TestFeatures>(value);

        // Assert
        flag.Value
            .Should()
            .Be(value);
    }

    [Test]
    public void Constructor_WithBitIndex_ShouldSetCorrectBit()
    {
        // Act
        var flag = new BigFlagsValue<TestFeatures>(5);

        // Assert
        flag.Value
            .Should()
            .Be(BigInteger.One << 5);

        flag.HasFlag(5)
            .Should()
            .BeTrue();
    }

    [Test]
    public void EqualityOperator_ShouldReturnFalse_WhenValuesDifferent()
    {
        // Arrange
        var flag1 = TestFeatures.Feature1;
        var flag2 = TestFeatures.Feature2;

        // Assert
        (flag1 == flag2).Should()
                        .BeFalse();
    }

    [Test]
    public void EqualityOperator_ShouldReturnTrue_WhenValuesEqual()
    {
        // Arrange
        var flag1 = TestFeatures.Feature1;
        var flag2 = TestFeatures.Feature1;

        // Assert
        (flag1 == flag2).Should()
                        .BeTrue();
    }

    [Test]
    public void Equals_ShouldReturnTrue_WhenValuesEqual()
    {
        // Arrange
        var flag1 = TestFeatures.Feature1;
        var flag2 = TestFeatures.Feature1;

        // Assert
        flag1.Equals(flag2)
             .Should()
             .BeTrue();
    }

    [Test]
    public void GetHashCode_ShouldBeConsistent()
    {
        // Arrange
        var flag1 = TestFeatures.Feature1;
        var flag2 = TestFeatures.Feature1;

        // Assert
        flag1.GetHashCode()
             .Should()
             .Be(flag2.GetHashCode());
    }

    [Test]
    public void GetSetBitIndices_ShouldReturnCorrectIndices()
    {
        // Arrange
        var flag = new BigFlagsValue<TestFeatures>((BigInteger.One << 0) | (BigInteger.One << 5) | (BigInteger.One << 100));

        // Act
        var indices = flag.GetSetBitIndices()
                          .ToList();

        // Assert
        indices.Should()
               .Equal(0, 5, 100);
    }

    [Test]
    public void GetSetBitIndices_ShouldReturnEmpty_WhenNoFlagsSet()
    {
        // Arrange
        var flag = BigFlagsValue<TestFeatures>.None;

        // Act
        var indices = flag.GetSetBitIndices()
                          .ToList();

        // Assert
        indices.Should()
               .BeEmpty();
    }

    [Test]
    public void HasAllFlags_ShouldReturnFalse_WhenSomeBitsNotSet()
    {
        // Arrange
        var flag = new BigFlagsValue<TestFeatures>((BigInteger)0b1010); // Bits 1, 3

        // Assert
        flag.HasAllFlags(
                0,
                1,
                2,
                3)
            .Should()
            .BeFalse();
    }

    [Test]
    public void HasAllFlags_ShouldReturnTrue_WhenAllBitsSet()
    {
        // Arrange
        var flag = new BigFlagsValue<TestFeatures>((BigInteger)0b1111); // Bits 0, 1, 2, 3

        // Assert
        flag.HasAllFlags(
                0,
                1,
                2,
                3)
            .Should()
            .BeTrue();
    }

    [Test]
    public void HasAnyFlag_ShouldReturnFalse_WhenNoBitsSet()
    {
        // Arrange
        var flag = new BigFlagsValue<TestFeatures>((BigInteger)0b1000); // Bit 3

        // Assert
        flag.HasAnyFlag(0, 1, 2)
            .Should()
            .BeFalse();
    }

    [Test]
    public void HasAnyFlag_ShouldReturnTrue_WhenAnyBitSet()
    {
        // Arrange
        var flag = new BigFlagsValue<TestFeatures>((BigInteger)0b0010); // Bit 1

        // Assert
        flag.HasAnyFlag(0, 1, 2)
            .Should()
            .BeTrue();
    }

    [Test]
    public void HasFlag_WithBitIndex_ShouldReturnFalse_WhenBitIsNotSet()
    {
        // Arrange
        var flag = new BigFlagsValue<TestFeatures>(5);

        // Assert
        flag.HasFlag(3)
            .Should()
            .BeFalse();
    }

    [Test]
    public void HasFlag_WithBitIndex_ShouldReturnTrue_WhenBitIsSet()
    {
        // Arrange
        var flag = new BigFlagsValue<TestFeatures>(5);

        // Assert
        flag.HasFlag(5)
            .Should()
            .BeTrue();
    }

    [Test]
    public void HasFlag_WithFlagValue_ShouldReturnFalse_WhenBitsNotSet()
    {
        // Arrange
        var flag = TestFeatures.Feature1;

        // Assert
        flag.HasFlag(TestFeatures.Feature2)
            .Should()
            .BeFalse();
    }

    [Test]
    public void HasFlag_WithFlagValue_ShouldReturnTrue_WhenAllBitsSet()
    {
        // Arrange
        var flag = TestFeatures.Feature1 | TestFeatures.Feature2;

        // Assert
        flag.HasFlag(TestFeatures.Feature1)
            .Should()
            .BeTrue();

        flag.HasFlag(TestFeatures.Feature2)
            .Should()
            .BeTrue();
    }

    [Test]
    public void IBigFlagsValue_Type_ShouldReturnMarkerType()
    {
        // Arrange
        IBigFlagsValue flag = TestFeatures.Feature1;

        // Act
        var type = flag.Type;

        // Assert
        type.Should()
            .Be(typeof(TestFeatures));
    }

    [Test]
    public void IBigFlagsValue_Value_ShouldReturnUnderlyingValue()
    {
        // Arrange
        IBigFlagsValue flag = TestFeatures.Feature1;

        // Act
        var value = flag.Value;

        // Assert
        value.Should()
             .Be(TestFeatures.Feature1.Value);
    }

    [Test]
    public void ImplicitConversion_FromBigInteger_ShouldWork()
    {
        // Arrange
        BigInteger value = 42;

        // Act
        BigFlagsValue<TestFeatures> flag = value;

        // Assert
        flag.Value
            .Should()
            .Be(value);
    }

    [Test]
    public void ImplicitConversion_FromInt_ShouldWork()
    {
        // Act
        BigFlagsValue<TestFeatures> flag = 42;

        // Assert
        flag.Value
            .Should()
            .Be(42);
    }

    [Test]
    public void ImplicitConversion_FromLong_ShouldWork()
    {
        // Act
        BigFlagsValue<TestFeatures> flag = 42L;

        // Assert
        flag.Value
            .Should()
            .Be(42);
    }

    [Test]
    public void ImplicitConversion_ToBigInteger_ShouldWork()
    {
        // Arrange
        var flag = new BigFlagsValue<TestFeatures>((BigInteger)42);

        // Act
        BigInteger value = flag;

        // Assert
        value.Should()
             .Be(42);
    }

    [Test]
    public void InequalityOperator_ShouldWork()
    {
        // Arrange
        var flag1 = TestFeatures.Feature1;
        var flag2 = TestFeatures.Feature2;

        // Assert
        (flag1 != flag2).Should()
                        .BeTrue();

        (flag1 != TestFeatures.Feature1).Should()
                                        .BeFalse();
    }

    [Test]
    public void IsEmpty_ShouldReturnFalse_WhenFlagsSet()
    {
        // Arrange
        var flag = new BigFlagsValue<TestFeatures>(42);

        // Assert
        flag.IsEmpty
            .Should()
            .BeFalse();
    }

    [Test]
    public void IsEmpty_ShouldReturnTrue_WhenNoFlagsSet()
    {
        // Arrange
        var flag = new BigFlagsValue<TestFeatures>(BigInteger.Zero);

        // Assert
        flag.IsEmpty
            .Should()
            .BeTrue();
    }

    [Test]
    public void LargeBitIndex_ShouldWork()
    {
        // Arrange
        const int LARGE_INDEX = 1000;
        var flag = new BigFlagsValue<TestFeatures>(LARGE_INDEX);

        // Assert
        flag.HasFlag(LARGE_INDEX)
            .Should()
            .BeTrue();

        flag.Value
            .Should()
            .Be(BigInteger.One << LARGE_INDEX);
    }

    [Test]
    public void None_ShouldHaveZeroValue()
    {
        // Assert
        BigFlagsValue<TestFeatures>.None
                                   .Value
                                   .Should()
                                   .Be(BigInteger.Zero);

        BigFlagsValue<TestFeatures>.None
                                   .IsEmpty
                                   .Should()
                                   .BeTrue();
    }

    [Test]
    public void NotOperator_ShouldInvertAllBits()
    {
        // Arrange
        var flag = TestFeatures.Feature1;

        // Act
        var result = ~flag;

        // Assert - In two's complement, ~flag should not have Feature1 bit set
        result.HasFlag(TestFeatures.Feature1)
              .Should()
              .BeFalse();
    }

    [Test]
    public void OrOperator_ShouldCombineFlags()
    {
        // Act
        var result = TestFeatures.Feature1 | TestFeatures.Feature2;

        // Assert
        result.HasFlag(TestFeatures.Feature1)
              .Should()
              .BeTrue();

        result.HasFlag(TestFeatures.Feature2)
              .Should()
              .BeTrue();
    }

    [Test]
    public void SetFlag_WithBitIndex_ShouldSetCorrectBit()
    {
        // Arrange
        var flag = new BigFlagsValue<TestFeatures>(0);

        // Act
        var result = flag.SetFlag(5);

        // Assert
        result.HasFlag(5)
              .Should()
              .BeTrue();

        flag.HasFlag(5)
            .Should()
            .BeFalse(); // Original should be unchanged
    }

    [Test]
    public void SetFlag_WithFlagValue_ShouldCombineFlags()
    {
        // Arrange
        var flag = TestFeatures.Feature1;

        // Act
        var result = flag.SetFlag(TestFeatures.Feature2);

        // Assert
        result.HasFlag(TestFeatures.Feature1)
              .Should()
              .BeTrue();

        result.HasFlag(TestFeatures.Feature2)
              .Should()
              .BeTrue();
    }

    [Test]
    public void ToBinaryString_ShouldReturnBinaryRepresentation()
    {
        // Arrange
        var flag = new BigFlagsValue<TestFeatures>((BigInteger)0b1010);

        // Act
        var binary = flag.ToBinaryString();

        // Assert
        binary.Should()
              .EndWith("00001010");
    }

    [Test]
    public void ToBinaryString_ShouldReturnZero_WhenEmpty()
    {
        // Arrange
        var flag = BigFlagsValue<TestFeatures>.None;

        // Act
        var binary = flag.ToBinaryString();

        // Assert
        binary.Should()
              .Be("0");
    }

    [Test]
    public void ToggleFlag_WithBitIndex_ShouldToggleBit()
    {
        // Arrange
        var flag = new BigFlagsValue<TestFeatures>((BigInteger)0b101); // Bits 0, 2

        // Act
        var result = flag.ToggleFlag(1);

        // Assert
        result.HasFlag(0)
              .Should()
              .BeTrue();

        result.HasFlag(1)
              .Should()
              .BeTrue();

        result.HasFlag(2)
              .Should()
              .BeTrue();

        // Toggle again
        result = result.ToggleFlag(1);

        result.HasFlag(1)
              .Should()
              .BeFalse();
    }

    [Test]
    public void ToggleFlag_WithFlagValue_ShouldToggleFlags()
    {
        // Arrange
        var flag = TestFeatures.Feature1;

        // Act
        var result = flag.ToggleFlag(TestFeatures.Feature2);

        // Assert
        result.HasFlag(TestFeatures.Feature1)
              .Should()
              .BeTrue();

        result.HasFlag(TestFeatures.Feature2)
              .Should()
              .BeTrue();

        // Toggle again
        result = result.ToggleFlag(TestFeatures.Feature2);

        result.HasFlag(TestFeatures.Feature2)
              .Should()
              .BeFalse();
    }

    [Test]
    public void ToString_ShouldUseBigFlagsToString()
    {
        // Act
        var str = TestFeatures.Feature1.ToString();

        // Assert
        str.Should()
           .Be("Feature1");
    }

    [Test]
    public void VeryLargeCombination_ShouldWork()
    {
        // Arrange
        var flag1 = new BigFlagsValue<TestFeatures>(100);
        var flag2 = new BigFlagsValue<TestFeatures>(500);
        var flag3 = new BigFlagsValue<TestFeatures>(1000);

        // Act
        var combined = flag1 | flag2 | flag3;

        // Assert
        combined.HasFlag(100)
                .Should()
                .BeTrue();

        combined.HasFlag(500)
                .Should()
                .BeTrue();

        combined.HasFlag(1000)
                .Should()
                .BeTrue();

        combined.GetSetBitIndices()
                .Should()
                .Equal(100, 500, 1000);
    }

    [Test]
    public void XorOperator_ShouldExclusiveOrFlags()
    {
        // Arrange
        var flag1 = TestFeatures.Feature1 | TestFeatures.Feature2;
        var flag2 = TestFeatures.Feature2 | TestFeatures.Feature3;

        // Act
        var result = flag1 ^ flag2;

        // Assert
        result.HasFlag(TestFeatures.Feature1)
              .Should()
              .BeTrue();

        result.HasFlag(TestFeatures.Feature2)
              .Should()
              .BeFalse();

        result.HasFlag(TestFeatures.Feature3)
              .Should()
              .BeTrue();
    }
}