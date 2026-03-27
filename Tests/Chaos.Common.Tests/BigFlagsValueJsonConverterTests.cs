#region
using System.Numerics;
using System.Text.Json;
using Chaos.Common.CustomTypes;
using Chaos.Testing.Infrastructure.Definitions;
using FluentAssertions;
#endregion

namespace Chaos.Common.Tests;

public sealed class BigFlagsValueJsonConverterTests
{
    private readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = false
    };

    [Test]
    public void Deserialize_DifferentMarkerTypes_ShouldWork()
    {
        // Arrange
        const string JSON = "\"Execute, Delete\"";

        // Act
        var permissions = JsonSerializer.Deserialize<BigFlagsValue<TestPermissions>>(JSON, Options);

        // Assert
        permissions.HasFlag(TestPermissions.Execute)
                   .Should()
                   .BeTrue();

        permissions.HasFlag(TestPermissions.Delete)
                   .Should()
                   .BeTrue();
    }

    [Test]
    public void Deserialize_ExplicitBitIndices_ShouldWork()
    {
        // Arrange
        const string JSON = "\"Flag20, AutoFlag\"";

        // Act
        var value = JsonSerializer.Deserialize<BigFlagsValue<TestExplicitIndices>>(JSON, Options);

        // Assert
        value.HasFlag(TestExplicitIndices.Flag20)
             .Should()
             .BeTrue();

        value.HasFlag(TestExplicitIndices.AutoFlag)
             .Should()
             .BeTrue();
    }

    [Test]
    public void Deserialize_InObject_ShouldWork()
    {
        // Arrange
        const string JSON = "{\"Flags\":\"Feature2, Feature3\"}";

        // Act
        var obj = JsonSerializer.Deserialize<TestObject>(JSON, Options);

        // Assert
        obj.Should()
           .NotBeNull();

        obj!.Flags
            .HasFlag(TestFeatures.Feature2)
            .Should()
            .BeTrue();

        obj.Flags
           .HasFlag(TestFeatures.Feature3)
           .Should()
           .BeTrue();
    }

    [Test]
    public void Deserialize_NoneString_ShouldReturnNone()
    {
        // Arrange
        const string JSON = "\"None\"";

        // Act
        var value = JsonSerializer.Deserialize<BigFlagsValue<TestFeatures>>(JSON, Options);

        // Assert
        value.IsEmpty
             .Should()
             .BeTrue();
    }

    [Test]
    public void Deserialize_NonStringToken_ShouldThrowJsonException()
    {
        // Arrange
        const string JSON = "123";

        // Act
        var act = () => JsonSerializer.Deserialize<BigFlagsValue<TestFeatures>>(JSON, Options);

        // Assert
        act.Should()
           .Throw<JsonException>()
           .WithMessage("*Expected string token*");
    }

    [Test]
    public void Deserialize_NullToken_ShouldReturnNone()
    {
        // Arrange
        const string JSON = "null";

        // Act
        var value = JsonSerializer.Deserialize<BigFlagsValue<TestFeatures>>(JSON, Options);

        // Assert
        value.Should()
             .Be(TestFeatures.None);
    }

    [Test]
    public void Deserialize_ShouldHandleWhitespace_InCommaSeparatedNames()
    {
        // Arrange
        const string JSON = "\"Feature1,Feature2 , Feature3\"";

        // Act
        var value = JsonSerializer.Deserialize<BigFlagsValue<TestFeatures>>(JSON, Options);

        // Assert
        value.HasFlag(TestFeatures.Feature1)
             .Should()
             .BeTrue();

        value.HasFlag(TestFeatures.Feature2)
             .Should()
             .BeTrue();

        value.HasFlag(TestFeatures.Feature3)
             .Should()
             .BeTrue();
    }

    [Test]
    public void Deserialize_ShouldReadCommaSeparatedNames_ForCombinedFlags()
    {
        // Arrange
        const string JSON = "\"Feature1, Feature3, Feature4\"";

        // Act
        var value = JsonSerializer.Deserialize<BigFlagsValue<TestFeatures>>(JSON, Options);

        // Assert
        value.HasFlag(TestFeatures.Feature1)
             .Should()
             .BeTrue();

        value.HasFlag(TestFeatures.Feature3)
             .Should()
             .BeTrue();

        value.HasFlag(TestFeatures.Feature4)
             .Should()
             .BeTrue();

        value.HasFlag(TestFeatures.Feature2)
             .Should()
             .BeFalse();
    }

    [Test]
    public void Deserialize_ShouldReadEmptyString_AsNone()
    {
        // Arrange
        const string JSON = "\"\"";

        // Act
        var value = JsonSerializer.Deserialize<BigFlagsValue<TestFeatures>>(JSON, Options);

        // Assert
        value.Should()
             .Be(TestFeatures.None);

        value.IsEmpty
             .Should()
             .BeTrue();
    }

    [Test]
    public void Deserialize_ShouldReadFlagName_ForSingleFlag()
    {
        // Arrange
        const string JSON = "\"Feature2\"";

        // Act
        var value = JsonSerializer.Deserialize<BigFlagsValue<TestFeatures>>(JSON, Options);

        // Assert
        value.Should()
             .Be(TestFeatures.Feature2);
    }

    [Test]
    public void Deserialize_ShouldThrowJsonException_ForPartiallyUnknownFlags()
    {
        // Arrange
        const string JSON = "\"Feature1, UnknownFlag, Feature2\"";

        // Act
        var act = () => JsonSerializer.Deserialize<BigFlagsValue<TestFeatures>>(JSON, Options);

        // Assert
        act.Should()
           .Throw<JsonException>()
           .WithMessage("*Unknown flag name*");
    }

    [Test]
    public void Deserialize_ShouldThrowJsonException_ForUnknownFlagName()
    {
        // Arrange
        const string JSON = "\"NonExistentFlag\"";

        // Act
        var act = () => JsonSerializer.Deserialize<BigFlagsValue<TestFeatures>>(JSON, Options);

        // Assert
        act.Should()
           .Throw<JsonException>()
           .WithMessage("*Unknown flag name*");
    }

    [Test]
    public void RoundTrip_ShouldPreserveValue_ForCombinedFlags()
    {
        // Arrange
        var original = TestFeatures.Feature1 | TestFeatures.Feature2 | TestFeatures.Feature4;

        // Act
        var json = JsonSerializer.Serialize(original, Options);
        var deserialized = JsonSerializer.Deserialize<BigFlagsValue<TestFeatures>>(json, Options);

        // Assert
        deserialized.Should()
                    .Be(original);
    }

    [Test]
    public void RoundTrip_ShouldPreserveValue_ForNone()
    {
        // Arrange
        var original = TestFeatures.None;

        // Act
        var json = JsonSerializer.Serialize(original, Options);
        var deserialized = JsonSerializer.Deserialize<BigFlagsValue<TestFeatures>>(json, Options);

        // Assert
        deserialized.Should()
                    .Be(original);
    }

    [Test]
    public void RoundTrip_ShouldPreserveValue_ForSingleFlag()
    {
        // Arrange
        var original = TestFeatures.Feature3;

        // Act
        var json = JsonSerializer.Serialize(original, Options);
        var deserialized = JsonSerializer.Deserialize<BigFlagsValue<TestFeatures>>(json, Options);

        // Assert
        deserialized.Should()
                    .Be(original);
    }

    [Test]
    public void Serialize_DifferentMarkerTypes_ShouldWork()
    {
        // Arrange
        var permissions = TestPermissions.Read | TestPermissions.Write;

        // Act
        var json = JsonSerializer.Serialize(permissions, Options);

        // Assert
        json.Should()
            .Be("\"Read, Write\"");
    }

    [Test]
    public void Serialize_ExplicitBitIndices_ShouldWork()
    {
        // Arrange
        var value = TestExplicitIndices.Flag10 | TestExplicitIndices.Flag100;

        // Act
        var json = JsonSerializer.Serialize(value, Options);

        // Assert
        json.Should()
            .Be("\"Flag10, Flag100\"");
    }

    [Test]
    public void Serialize_InObject_ShouldWork()
    {
        // Arrange
        var obj = new TestObject
        {
            Flags = TestFeatures.Feature1 | TestFeatures.Feature2
        };

        // Act
        var json = JsonSerializer.Serialize(obj, Options);

        // Assert
        json.Should()
            .Contain("\"Feature1, Feature2\"");
    }

    [Test]
    public void Serialize_ShouldWriteCommaSeparatedNames_ForCombinedFlags()
    {
        // Arrange
        var value = TestFeatures.Feature1 | TestFeatures.Feature3;

        // Act
        var json = JsonSerializer.Serialize(value, Options);

        // Assert
        json.Should()
            .Be("\"Feature1, Feature3\"");
    }

    [Test]
    public void Serialize_ShouldWriteEmptyString_ForNone()
    {
        // Arrange
        var value = TestFeatures.None;

        // Act
        var json = JsonSerializer.Serialize(value, Options);

        // Assert
        json.Should()
            .Be("\"None\"");
    }

    [Test]
    public void Serialize_ShouldWriteFlagName_ForSingleFlag()
    {
        // Arrange
        var value = TestFeatures.Feature1;

        // Act
        var json = JsonSerializer.Serialize(value, Options);

        // Assert
        json.Should()
            .Be("\"Feature1\"");
    }

    [Test]
    public void Serialize_ShouldWriteNumericValue_ForUndefinedFlag()
    {
        // Arrange
        var value = new BigFlagsValue<TestFeatures>((BigInteger)1024); // Bit 10 - undefined

        // Act
        var json = JsonSerializer.Serialize(value, Options);

        // Assert
        json.Should()
            .Be("\"1024\"");
    }

    private sealed class TestObject
    {
        public BigFlagsValue<TestFeatures> Flags { get; set; }
    }
}