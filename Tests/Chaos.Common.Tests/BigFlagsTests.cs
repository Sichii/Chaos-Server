#region
using System.Numerics;
using Chaos.Common.Abstractions;
using Chaos.Common.CustomTypes;
using Chaos.Testing.Infrastructure.Definitions;
using FluentAssertions;
#endregion

namespace Chaos.Common.Tests;

/// <summary>
///     Tests for the non-generic <see cref="BigFlags" /> static utility class. The generic BigFlags&lt;T&gt; behavior is
///     tested transitively.
/// </summary>
public sealed class BigFlagsTests
{
    // A non-class type to trigger the IsValidMarkerType → false path
    private static readonly Type InvalidType = typeof(int); // struct, not a class

    // A class type that does NOT inherit BigFlags<T>
    private static readonly Type NotMarkerClass = typeof(string);

    // A valid marker type used for success-path tests
    private static readonly Type ValidMarker = typeof(TestFeatures);

    #region IsValidMarkerType — indirect via IsDefined / TryParse (returns false)
    [Test]
    public void IsDefined_ByName_InvalidMarkerType_ShouldReturnFalse()
        => BigFlags.IsDefined(InvalidType, "Feature1")
                   .Should()
                   .BeFalse();

    [Test]
    public void IsDefined_ByValue_InvalidMarkerType_ShouldReturnFalse()
    {
        IBigFlagsValue value = TestFeatures.Feature1;

        BigFlags.IsDefined(InvalidType, value)
                .Should()
                .BeFalse();
    }

    [Test]
    public void IsDefined_ByName_NotMarkerClass_ShouldReturnFalse()
        => BigFlags.IsDefined(NotMarkerClass, "Feature1")
                   .Should()
                   .BeFalse();

    [Test]
    public void TryParse_InvalidMarkerType_ShouldReturnFalse()
    {
        var result = BigFlags.TryParse(
            InvalidType,
            "Feature1",
            false,
            out var value);

        result.Should()
              .BeFalse();

        value.Should()
             .BeNull();
    }

    [Test]
    public void TryParse_NotMarkerClass_ShouldReturnFalse()
    {
        var result = BigFlags.TryParse(
            NotMarkerClass,
            "Feature1",
            false,
            out var value);

        result.Should()
              .BeFalse();

        value.Should()
             .BeNull();
    }
    #endregion

    #region ValidateMarkerType — indirect via throwing methods
    [Test]
    public void GetName_InvalidMarkerType_ShouldThrowArgumentException()
    {
        IBigFlagsValue value = TestFeatures.Feature1;

        var act = () => BigFlags.GetName(InvalidType, value);

        act.Should()
           .Throw<ArgumentException>();
    }

    [Test]
    public void GetNames_InvalidMarkerType_ShouldThrowArgumentException()
    {
        var act = () => BigFlags.GetNames(InvalidType)
                                .ToList();

        act.Should()
           .Throw<ArgumentException>();
    }

    [Test]
    public void GetNone_InvalidMarkerType_ShouldThrowArgumentException()
    {
        var act = () => BigFlags.GetNone(InvalidType);

        act.Should()
           .Throw<ArgumentException>();
    }

    [Test]
    public void GetValues_InvalidMarkerType_ShouldThrowArgumentException()
    {
        var act = () => BigFlags.GetValues(InvalidType)
                                .ToList();

        act.Should()
           .Throw<ArgumentException>();
    }

    [Test]
    public void Parse_InvalidMarkerType_ShouldThrowArgumentException()
    {
        var act = () => BigFlags.Parse(InvalidType, "Feature1");

        act.Should()
           .Throw<ArgumentException>();
    }

    [Test]
    public void ToString_InvalidMarkerType_ShouldThrowArgumentException()
    {
        IBigFlagsValue value = TestFeatures.Feature1;

        var act = () => BigFlags.ToString(InvalidType, value);

        act.Should()
           .Throw<ArgumentException>();
    }

    [Test]
    public void Create_InvalidMarkerType_BigInteger_ShouldThrowArgumentException()
    {
        var act = () => BigFlags.Create(InvalidType, BigInteger.One);

        act.Should()
           .Throw<ArgumentException>();
    }

    [Test]
    public void Create_InvalidMarkerType_BitIndex_ShouldThrowArgumentException()
    {
        var act = () => BigFlags.Create(InvalidType, 0);

        act.Should()
           .Throw<ArgumentException>();
    }
    #endregion

    #region Happy-path sanity checks (ensure the utility works for valid markers)
    [Test]
    public void GetNames_ValidMarkerType_ShouldReturnNames()
    {
        var names = BigFlags.GetNames(ValidMarker)
                            .ToList();

        names.Should()
             .NotBeEmpty();
    }

    [Test]
    public void GetValues_ValidMarkerType_ShouldReturnValues()
    {
        var values = BigFlags.GetValues(ValidMarker)
                             .ToList();

        values.Should()
              .NotBeEmpty();
    }

    [Test]
    public void GetNone_ValidMarkerType_ShouldReturnNoneValue()
    {
        var none = BigFlags.GetNone(ValidMarker);

        none.Should()
            .NotBeNull();

        none.Value
            .Should()
            .Be(BigInteger.Zero);
    }

    [Test]
    public void IsDefined_ByName_ValidMarkerType_ShouldReturnTrue()
        => BigFlags.IsDefined(ValidMarker, "Feature1")
                   .Should()
                   .BeTrue();

    [Test]
    public void TryParse_ValidMarkerType_ShouldReturnTrue()
    {
        var result = BigFlags.TryParse(
            ValidMarker,
            "Feature1",
            false,
            out var value);

        result.Should()
              .BeTrue();

        value.Should()
             .NotBeNull();
    }
    #endregion

    #region Non-generic API — success paths with valid marker types
    [Test]
    public void GetName_ValidMarkerType_ShouldReturnName()
    {
        IBigFlagsValue value = TestFeatures.Feature2;

        var name = BigFlags.GetName(ValidMarker, value);

        name.Should()
            .Be("Feature2");
    }

    [Test]
    public void GetName_ValidMarkerType_UnknownValue_ShouldReturnNull()
    {
        IBigFlagsValue value = new BigFlagsValue<TestFeatures>(999);

        var name = BigFlags.GetName(ValidMarker, value);

        name.Should()
            .BeNull();
    }

    [Test]
    public void ToString_ValidMarkerType_ShouldReturnName()
    {
        IBigFlagsValue value = TestFeatures.Feature3;

        var str = BigFlags.ToString(ValidMarker, value);

        str.Should()
           .Be("Feature3");
    }

    [Test]
    public void ToString_ValidMarkerType_CombinedFlags_ShouldReturnCommaSeparated()
    {
        IBigFlagsValue value = TestFeatures.Feature1 | TestFeatures.Feature4;

        var str = BigFlags.ToString(ValidMarker, value);

        str.Should()
           .Be("Feature1, Feature4");
    }

    [Test]
    public void IsDefined_ByValue_ValidMarkerType_ShouldReturnTrue()
    {
        IBigFlagsValue value = TestFeatures.Feature1;

        BigFlags.IsDefined(ValidMarker, value)
                .Should()
                .BeTrue();
    }

    [Test]
    public void IsDefined_ByValue_ValidMarkerType_UnknownValue_ShouldReturnFalse()
    {
        IBigFlagsValue value = new BigFlagsValue<TestFeatures>(999);

        BigFlags.IsDefined(ValidMarker, value)
                .Should()
                .BeFalse();
    }

    [Test]
    public void IsDefined_ByName_ValidMarkerType_NotFound_ShouldReturnFalse()
        => BigFlags.IsDefined(ValidMarker, "NonExistent")
                   .Should()
                   .BeFalse();

    [Test]
    public void Parse_ValidMarkerType_ShouldReturnValue()
    {
        var value = BigFlags.Parse(ValidMarker, "Feature2");

        value.Should()
             .NotBeNull();

        value.Should()
             .Be(TestFeatures.Feature2);
    }

    [Test]
    public void Parse_ValidMarkerType_CaseInsensitive_ShouldReturnValue()
    {
        var value = BigFlags.Parse(ValidMarker, "feature3", true);

        value.Should()
             .Be(TestFeatures.Feature3);
    }

    [Test]
    public void Parse_ValidMarkerType_NotFound_ShouldThrow()
    {
        var act = () => BigFlags.Parse(ValidMarker, "NonExistent");

        act.Should()
           .Throw<ArgumentException>();
    }

    [Test]
    public void TryParse_ValidMarkerType_CaseInsensitive_ShouldReturnTrue()
    {
        var result = BigFlags.TryParse(
            ValidMarker,
            "FEATURE1",
            true,
            out var value);

        result.Should()
              .BeTrue();

        value.Should()
             .NotBeNull();
    }

    [Test]
    public void TryParse_ValidMarkerType_CaseSensitive_NotFound_ShouldReturnFalse()
    {
        var result = BigFlags.TryParse(
            ValidMarker,
            "feature1",
            false,
            out var value);

        result.Should()
              .BeFalse();

        value.Should()
             .BeNull();
    }

    [Test]
    public void Create_BigInteger_ValidMarkerType_ShouldReturnValue()
    {
        var value = BigFlags.Create(ValidMarker, BigInteger.One << 2);

        value.Should()
             .NotBeNull();

        value.Should()
             .Be(TestFeatures.Feature3);
    }

    [Test]
    public void Create_BitIndex_ValidMarkerType_ShouldReturnValue()
    {
        var value = BigFlags.Create(ValidMarker, 0);

        value.Should()
             .NotBeNull();

        value.Should()
             .Be(TestFeatures.Feature1);
    }
    #endregion
}