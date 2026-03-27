#region
using System.Numerics;
using Chaos.Common.CustomTypes;
using Chaos.Testing.Infrastructure.Definitions;
using FluentAssertions;
#endregion

namespace Chaos.Common.Tests;

public sealed class BigFlagsTypeTests
{
    [Test]
    public void Create_Generic_WithBigInteger_ShouldWork()
    {
        // Arrange
        var value = new BigInteger(42);

        // Act
        var flag = BigFlags.Create<TestFeatures>(value);

        // Assert
        flag.Value
            .Should()
            .Be(value);
    }

    [Test]
    public void Create_Generic_WithBitIndex_ShouldWork()
    {
        // Act
        var flag = BigFlags.Create<TestFeatures>(10);

        // Assert
        flag.HasFlag(10)
            .Should()
            .BeTrue();

        flag.Value
            .Should()
            .Be(BigInteger.One << 10);
    }

    [Test]
    public void Create_NonGeneric_WithBigInteger_ShouldWork()
    {
        // Arrange
        var value = new BigInteger(123);

        // Act
        var flag = BigFlags.Create(typeof(TestFeatures), value);

        // Assert
        flag.Value
            .Should()
            .Be(value);

        flag.Type
            .Should()
            .Be(typeof(TestFeatures));
    }

    [Test]
    public void Create_NonGeneric_WithBitIndex_ShouldWork()
    {
        // Act
        var flag = BigFlags.Create(typeof(TestFeatures), 15);

        // Assert
        flag.Value
            .Should()
            .Be(BigInteger.One << 15);
    }

    [Test]
    public void GenericAndNonGeneric_ShouldReturnEquivalentResults()
    {
        // Arrange
        const string FLAG_NAME = "Feature2";

        // Act
        var genericValue = BigFlags.Parse<TestFeatures>(FLAG_NAME);
        var nonGenericValue = BigFlags.Parse(typeof(TestFeatures), FLAG_NAME);

        // Assert
        genericValue.Value
                    .Should()
                    .Be(nonGenericValue.Value);
    }

    [Test]
    public void GetName_Generic_ShouldReturnCorrectName()
    {
        // Act
        var name = BigFlags.GetName(TestFeatures.Feature2);

        // Assert
        name.Should()
            .Be("Feature2");
    }

    [Test]
    public void GetName_Generic_ShouldReturnNull_WhenValueNotFound()
    {
        // Arrange
        var unknownValue = new BigFlagsValue<TestFeatures>(999);

        // Act
        var name = BigFlags.GetName(unknownValue);

        // Assert
        name.Should()
            .BeNull();
    }

    [Test]
    public void GetName_NonGeneric_ShouldReturnCorrectName()
    {
        // Act
        var name = BigFlags.GetName(typeof(TestFeatures), TestFeatures.Feature2);

        // Assert
        name.Should()
            .Be("Feature2");
    }

    [Test]
    public void GetName_NonGeneric_ShouldReturnNull_WhenValueNotFound()
    {
        // Arrange
        var unknownValue = new BigFlagsValue<TestFeatures>(999);

        // Act
        var name = BigFlags.GetName(typeof(TestFeatures), unknownValue);

        // Assert
        name.Should()
            .BeNull();
    }

    [Test]
    public void GetNames_Generic_ShouldReturnAllNames()
    {
        // Act
        var names = BigFlags.GetNames<TestFeatures>()
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
    public void GetNames_NonGeneric_ShouldReturnAllNames()
    {
        // Act
        var names = BigFlags.GetNames(typeof(TestFeatures))
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
    public void GetNone_Generic_ShouldReturnEmptyValue()
    {
        // Act
        var none = BigFlags.GetNone<TestFeatures>();

        // Assert
        none.Should()
            .Be(TestFeatures.None);

        none.IsEmpty
            .Should()
            .BeTrue();
    }

    [Test]
    public void GetNone_NonGeneric_ShouldReturnEmptyValue()
    {
        // Act
        var none = BigFlags.GetNone(typeof(TestFeatures));

        // Assert
        none.Value
            .Should()
            .Be(BigInteger.Zero);

        none.Type
            .Should()
            .Be(typeof(TestFeatures));
    }

    [Test]
    public void GetValues_Generic_ShouldReturnAllValues()
    {
        // Act
        var values = BigFlags.GetValues<TestFeatures>()
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
    public void GetValues_NonGeneric_ShouldReturnAllValues()
    {
        // Act
        var values = BigFlags.GetValues(typeof(TestFeatures))
                             .ToList();

        // Assert
        values.Should()
              .HaveCount(4);

        values.Select(v => v.Value)
              .Should()
              .Contain(
                  [
                      TestFeatures.Feature1.Value,
                      TestFeatures.Feature2.Value,
                      TestFeatures.Feature3.Value,
                      TestFeatures.Feature4.Value
                  ]);
    }

    [Test]
    public void IsDefined_Generic_WithName_ShouldReturnFalse_WhenNotFound()
        =>

            // Act & Assert
            BigFlags.IsDefined<TestPermissions>("NonExistent")
                    .Should()
                    .BeFalse();

    [Test]
    public void IsDefined_Generic_WithName_ShouldReturnTrue_WhenExists()
        =>

            // Act & Assert
            BigFlags.IsDefined<TestPermissions>("Read")
                    .Should()
                    .BeTrue();

    [Test]
    public void IsDefined_Generic_WithValue_ShouldReturnFalse_WhenNotFound()
    {
        // Arrange
        var unknownValue = new BigFlagsValue<TestPermissions>(999);

        // Act & Assert
        BigFlags.IsDefined(unknownValue)
                .Should()
                .BeFalse();
    }

    [Test]
    public void IsDefined_Generic_WithValue_ShouldReturnTrue_WhenExists()
        =>

            // Act & Assert
            BigFlags.IsDefined(TestPermissions.Write)
                    .Should()
                    .BeTrue();

    [Test]
    public void IsDefined_NonGeneric_WithName_ShouldReturnFalse_WhenNotFound()
        =>

            // Act & Assert
            BigFlags.IsDefined(typeof(TestPermissions), "NonExistent")
                    .Should()
                    .BeFalse();

    [Test]
    public void IsDefined_NonGeneric_WithName_ShouldReturnTrue_WhenExists()
        =>

            // Act & Assert
            BigFlags.IsDefined(typeof(TestPermissions), "Read")
                    .Should()
                    .BeTrue();

    [Test]
    public void IsDefined_NonGeneric_WithValue_ShouldReturnFalse_WhenNotFound()
    {
        // Arrange
        var unknownValue = new BigFlagsValue<TestPermissions>(999);

        // Act & Assert
        BigFlags.IsDefined(typeof(TestPermissions), unknownValue)
                .Should()
                .BeFalse();
    }

    [Test]
    public void IsDefined_NonGeneric_WithValue_ShouldReturnTrue_WhenExists()
        =>

            // Act & Assert
            BigFlags.IsDefined(typeof(TestPermissions), TestPermissions.Execute)
                    .Should()
                    .BeTrue();

    [Test]
    public void NonGeneric_WithInvalidMarkerType_ShouldThrow()
    {
        // Act
        var act = () => BigFlags.GetNames(typeof(string));

        // Assert
        act.Should()
           .Throw<ArgumentException>();
    }

    [Test]
    public void NonGeneric_WithNullType_ShouldHandleGracefully()
    {
        // Act
        var isDefinedResult = BigFlags.IsDefined(null!, "test");
        var tryParseResult = BigFlags.TryParse(null!, "test", out _);

        // Assert
        isDefinedResult.Should()
                       .BeFalse();

        tryParseResult.Should()
                      .BeFalse();
    }

    [Test]
    public void Parse_Generic_ShouldBeCaseInsensitive_WhenSpecified()
    {
        // Act
        var value = BigFlags.Parse<TestFeatures>("feature3", true);

        // Assert
        value.Should()
             .Be(TestFeatures.Feature3);
    }

    // Generic method tests

    [Test]
    public void Parse_Generic_ShouldReturnCorrectValue_WhenNameExists()
    {
        // Act
        var value = BigFlags.Parse<TestFeatures>("Feature2");

        // Assert
        value.Should()
             .Be(TestFeatures.Feature2);
    }

    [Test]
    public void Parse_Generic_ShouldThrowArgumentException_WhenNameNotFound()
    {
        // Act
        var act = () => BigFlags.Parse<TestFeatures>("NonExistent");

        // Assert
        act.Should()
           .Throw<ArgumentException>();
    }

    [Test]
    public void Parse_NonGeneric_ShouldBeCaseInsensitive_WhenSpecified()
    {
        // Act
        var value = BigFlags.Parse(typeof(TestFeatures), "feature1", true);

        // Assert
        value.Value
             .Should()
             .Be(TestFeatures.Feature1.Value);
    }

    // Non-generic method tests

    [Test]
    public void Parse_NonGeneric_ShouldReturnCorrectValue_WhenNameExists()
    {
        // Act
        var value = BigFlags.Parse(typeof(TestFeatures), "Feature2");

        // Assert
        value.Value
             .Should()
             .Be(TestFeatures.Feature2.Value);

        value.Type
             .Should()
             .Be(typeof(TestFeatures));
    }

    [Test]
    public void Parse_NonGeneric_ShouldThrowArgumentException_WhenNameNotFound()
    {
        // Act
        var act = () => BigFlags.Parse(typeof(TestFeatures), "NonExistent");

        // Assert
        act.Should()
           .Throw<ArgumentException>();
    }

    [Test]
    public void ToString_Generic_ShouldReturnCommaSeparated_ForCombinedFlags()
    {
        // Arrange
        var combined = TestFeatures.Feature1 | TestFeatures.Feature4;

        // Act
        var str = BigFlags.ToString(combined);

        // Assert
        str.Should()
           .Be("Feature1, Feature4");
    }

    [Test]
    public void ToString_Generic_ShouldReturnFlagName()
    {
        // Act
        var str = BigFlags.ToString(TestFeatures.Feature3);

        // Assert
        str.Should()
           .Be("Feature3");
    }

    [Test]
    public void ToString_NonGeneric_ShouldReturnCommaSeparated_ForCombinedFlags()
    {
        // Arrange
        var combined = TestFeatures.Feature2 | TestFeatures.Feature3;

        // Act
        var str = BigFlags.ToString(typeof(TestFeatures), combined);

        // Assert
        str.Should()
           .Be("Feature2, Feature3");
    }

    [Test]
    public void ToString_NonGeneric_ShouldReturnFlagName()
    {
        // Act
        var str = BigFlags.ToString(typeof(TestFeatures), TestFeatures.Feature4);

        // Assert
        str.Should()
           .Be("Feature4");
    }

    [Test]
    public void TryParse_Generic_ShouldReturnFalse_WhenNameNotFound()
    {
        // Act
        var success = BigFlags.TryParse<TestFeatures>("NonExistent", out var value);

        // Assert
        success.Should()
               .BeFalse();

        value.Should()
             .Be(BigFlagsValue<TestFeatures>.None);
    }

    [Test]
    public void TryParse_Generic_ShouldReturnTrue_WhenNameExists()
    {
        // Act
        var success = BigFlags.TryParse<TestFeatures>("Feature1", out var value);

        // Assert
        success.Should()
               .BeTrue();

        value.Should()
             .Be(TestFeatures.Feature1);
    }

    [Test]
    public void TryParse_Generic_WithIgnoreCase_ShouldWork()
    {
        // Act
        var success = BigFlags.TryParse<TestFeatures>("FEATURE4", true, out var value);

        // Assert
        success.Should()
               .BeTrue();

        value.Should()
             .Be(TestFeatures.Feature4);
    }

    [Test]
    public void TryParse_NonGeneric_Overload_ShouldWork()
    {
        // Act
        var success = BigFlags.TryParse(typeof(TestFeatures), "Feature2", out var value);

        // Assert
        success.Should()
               .BeTrue();

        value.Should()
             .NotBeNull();
    }

    [Test]
    public void TryParse_NonGeneric_ShouldReturnFalse_WhenNameNotFound()
    {
        // Act
        var success = BigFlags.TryParse(
            typeof(TestFeatures),
            "NonExistent",
            false,
            out var value);

        // Assert
        success.Should()
               .BeFalse();

        value.Should()
             .BeNull();
    }

    [Test]
    public void TryParse_NonGeneric_ShouldReturnTrue_WhenNameExists()
    {
        // Act
        var success = BigFlags.TryParse(
            typeof(TestFeatures),
            "Feature3",
            false,
            out var value);

        // Assert
        success.Should()
               .BeTrue();

        value.Should()
             .NotBeNull();

        value!.Value
              .Should()
              .Be(TestFeatures.Feature3.Value);
    }
}