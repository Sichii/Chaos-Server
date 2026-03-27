#region
using System.Numerics;
using Chaos.Common.Abstractions;
using Chaos.Common.CustomTypes;
using Chaos.Testing.Infrastructure.Definitions;
using FluentAssertions;
#endregion

namespace Chaos.Common.Tests;

public sealed class BigFlagsGenericTests
{
    [Test]
    public void DifferentMarkerTypes_ShouldHaveIndependentFlags()
    {
        // Assert - Different marker types should be distinct
        var featureValue = TestFeatures.Feature1;
        var permissionValue = TestPermissions.Read;

        // Values can be the same if bit indices are the same, but types should differ
        featureValue.Value
                    .Should()
                    .Be(permissionValue.Value); // Both are bit 0, so both equal 1

        // Get the marker types from IBigFlagsValue interface
        IBigFlagsValue featureInterface = featureValue;
        IBigFlagsValue permissionInterface = permissionValue;

        featureInterface.Type
                        .Should()
                        .Be(typeof(TestFeatures));

        permissionInterface.Type
                           .Should()
                           .Be(typeof(TestPermissions));

        featureInterface.Type
                        .Should()
                        .NotBe(permissionInterface.Type);
    }

    [Test]
    public void GetName_ShouldReturnCorrectName_WhenValueExists()
    {
        // Act
        var name = TestFeatures.GetName(TestFeatures.Feature2);

        // Assert
        name.Should()
            .Be("Feature2");
    }

    [Test]
    public void GetName_ShouldReturnNull_WhenValueNotFound()
    {
        // Arrange
        var unknownValue = new BigFlagsValue<TestFeatures>(999);

        // Act
        var name = TestFeatures.GetName(unknownValue);

        // Assert
        name.Should()
            .BeNull();
    }

    [Test]
    public void GetNames_ShouldReturnAllFlagNames()
    {
        // Act
        var names = TestFeatures.GetNames()
                                .ToList();

        // Assert
        names.Should()
             .HaveCount(4);

        names.Should()
             .Contain(
                 [
                     "Feature1",
                     "Feature2",
                     "Feature3",
                     "Feature4"
                 ]);
    }

    [Test]
    public void GetValues_ShouldReturnAllFlagValues()
    {
        // Act
        var values = TestFeatures.GetValues()
                                 .ToList();

        // Assert
        values.Should()
              .HaveCount(4);

        values.Should()
              .Contain(
                  [
                      TestFeatures.Feature1,
                      TestFeatures.Feature2,
                      TestFeatures.Feature3,
                      TestFeatures.Feature4
                  ]);
    }

    [Test]
    public void IsDefined_WithName_ShouldReturnFalse_WhenNameNotFound()
        =>

            // Act & Assert
            TestPermissions.IsDefined("NonExistent")
                           .Should()
                           .BeFalse();

    [Test]
    public void IsDefined_WithName_ShouldReturnTrue_WhenNameExists()
    {
        // Act & Assert
        TestPermissions.IsDefined("Read")
                       .Should()
                       .BeTrue();

        TestPermissions.IsDefined("Write")
                       .Should()
                       .BeTrue();
    }

    [Test]
    public void IsDefined_WithValue_ShouldReturnFalse_WhenValueNotFound()
    {
        // Arrange
        var unknownValue = new BigFlagsValue<TestPermissions>(999);

        // Act & Assert
        TestPermissions.IsDefined(unknownValue)
                       .Should()
                       .BeFalse();
    }

    [Test]
    public void IsDefined_WithValue_ShouldReturnTrue_WhenValueExists()
        =>

            // Act & Assert
            TestPermissions.IsDefined(TestPermissions.Execute)
                           .Should()
                           .BeTrue();

    [Test]
    public void None_ShouldBeZero()
    {
        // Assert
        TestFeatures.None
                    .Value
                    .Should()
                    .Be(BigInteger.Zero);

        TestFeatures.None
                    .IsEmpty
                    .Should()
                    .BeTrue();
    }

    [Test]
    public void Parse_ShouldBeCaseInsensitive_WhenSpecified()
    {
        // Act
        var value = TestFeatures.Parse("feature1", true);

        // Assert
        value.Should()
             .Be(TestFeatures.Feature1);
    }

    [Test]
    public void Parse_ShouldReturnCorrectValue_WhenNameExists()
    {
        // Act
        var value = TestFeatures.Parse("Feature3");

        // Assert
        value.Should()
             .Be(TestFeatures.Feature3);
    }

    [Test]
    public void Parse_ShouldThrowArgumentException_WhenNameNotFound()
    {
        // Act
        var act = () => TestFeatures.Parse("NonExistent");

        // Assert
        act.Should()
           .Throw<ArgumentException>();
    }

    [Test]
    public void StaticFields_ShouldAutoAssignAfterExplicitIndices()
        =>

            // The AutoFlag field comes after Flag100, so it should get bit index 101
            // Assert
            TestExplicitIndices.AutoFlag
                               .Value
                               .Should()
                               .Be(BigInteger.One << 101);

    [Test]
    public void StaticFields_ShouldBeAutomaticallyInitialized()
    {
        // Assert
        TestFeatures.Feature1
                    .Should()
                    .NotBe(default);

        TestFeatures.Feature2
                    .Should()
                    .NotBe(default);

        TestFeatures.Feature3
                    .Should()
                    .NotBe(default);

        TestFeatures.Feature4
                    .Should()
                    .NotBe(default);
    }

    [Test]
    public void StaticFields_ShouldHaveSequentialBitIndices_WhenNotExplicit()
    {
        // Assert
        TestFeatures.Feature1
                    .Value
                    .Should()
                    .Be(BigInteger.One << 0); // Bit 0

        TestFeatures.Feature2
                    .Value
                    .Should()
                    .Be(BigInteger.One << 1); // Bit 1

        TestFeatures.Feature3
                    .Value
                    .Should()
                    .Be(BigInteger.One << 2); // Bit 2

        TestFeatures.Feature4
                    .Value
                    .Should()
                    .Be(BigInteger.One << 3); // Bit 3
    }

    [Test]
    public void StaticFields_ShouldRespectExplicitBitIndices()
    {
        // Assert
        TestExplicitIndices.Flag10
                           .Value
                           .Should()
                           .Be(BigInteger.One << 10);

        TestExplicitIndices.Flag20
                           .Value
                           .Should()
                           .Be(BigInteger.One << 20);

        TestExplicitIndices.Flag100
                           .Value
                           .Should()
                           .Be(BigInteger.One << 100);
    }

    [Test]
    public void ToString_ShouldReturnCommaSeparatedNames_ForCombinedFlags()
    {
        // Arrange
        var combined = TestFeatures.Feature1 | TestFeatures.Feature3;

        // Act
        var str = TestFeatures.ToString(combined);

        // Assert
        str.Should()
           .Be("Feature1, Feature3");
    }

    [Test]
    public void ToString_ShouldReturnFlagName_ForSingleFlag()
    {
        // Act
        var str = TestFeatures.ToString(TestFeatures.Feature1);

        // Assert
        str.Should()
           .Be("Feature1");
    }

    [Test]
    public void ToString_ShouldReturnNumericValue_ForUndefinedFlags()
    {
        // Arrange
        var undefined = new BigFlagsValue<TestFeatures>((BigInteger)999);

        // Act
        var str = TestFeatures.ToString(undefined);

        // Assert
        str.Should()
           .Be("999");
    }

    [Test]
    public void TryParse_ShouldBeCaseInsensitive_WhenSpecified()
    {
        // Act
        var success = TestFeatures.TryParse("FEATURE4", true, out var value);

        // Assert
        success.Should()
               .BeTrue();

        value.Should()
             .Be(TestFeatures.Feature4);
    }

    [Test]
    public void TryParse_ShouldReturnFalse_WhenNameNotFound()
    {
        // Act
        var success = TestFeatures.TryParse("NonExistent", out var value);

        // Assert
        success.Should()
               .BeFalse();

        value.Should()
             .Be(BigFlagsValue<TestFeatures>.None);
    }

    [Test]
    public void TryParse_ShouldReturnTrue_WhenNameExists()
    {
        // Act
        var success = TestFeatures.TryParse("Feature2", out var value);

        // Assert
        success.Should()
               .BeTrue();

        value.Should()
             .Be(TestFeatures.Feature2);
    }
}