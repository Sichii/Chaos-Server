#region
using System.Text.Json;
using Chaos.Collections.Common;
using Chaos.Testing.Infrastructure.Definitions;
using FluentAssertions;
#endregion

namespace Chaos.Common.Tests;

public sealed class BigFlagsCollectionJsonConverterTests
{
    private readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = false
    };

    [Test]
    public void Deserialize_EmptyFlagValue_ShouldAddNone()
    {
        // Arrange
        var typeName = typeof(TestFeatures).Name;
        var json = $"{{\"{typeName}\":\"\"}}";

        // Act
        var collection = JsonSerializer.Deserialize<BigFlagsCollection>(json, Options);

        // Assert
        collection.Should()
                  .NotBeNull();

        collection!.TryGetFlag<TestFeatures>(out var value)
                   .Should()
                   .BeTrue();

        value.IsEmpty
             .Should()
             .BeTrue();
    }

    [Test]
    public void Deserialize_EmptyObject_ShouldCreateEmptyCollection()
    {
        // Arrange
        const string JSON = "{}";

        // Act
        var collection = JsonSerializer.Deserialize<BigFlagsCollection>(JSON, Options);

        // Assert
        collection.Should()
                  .NotBeNull();

        collection.Should()
                  .BeEmpty();
    }

    [Test]
    public void Deserialize_ExplicitBitIndices_ShouldWork()
    {
        // Arrange
        var typeName = typeof(TestExplicitIndices).Name;
        var json = $"{{\"{typeName}\":\"Flag100, AutoFlag\"}}";

        // Act
        var collection = JsonSerializer.Deserialize<BigFlagsCollection>(json, Options);

        // Assert
        collection.Should()
                  .NotBeNull();

        collection!.HasFlag(TestExplicitIndices.Flag100)
                   .Should()
                   .BeTrue();

        collection.HasFlag(TestExplicitIndices.AutoFlag)
                  .Should()
                  .BeTrue();
    }

    [Test]
    public void Deserialize_InObject_ShouldWork()
    {
        // Arrange
        var typeName = typeof(TestFeatures).Name;
        var json = $"{{\"Flags\":{{\"{typeName}\":\"Feature1, Feature4\"}}}}";

        // Act
        var obj = JsonSerializer.Deserialize<TestObject>(json, Options);

        // Assert
        obj.Should()
           .NotBeNull();

        obj!.Flags
            .Should()
            .NotBeNull();

        obj.Flags
           .HasFlag(TestFeatures.Feature1)
           .Should()
           .BeTrue();

        obj.Flags
           .HasFlag(TestFeatures.Feature4)
           .Should()
           .BeTrue();
    }

    [Test]
    public void Deserialize_MultipleTypes_ShouldRestoreAllFlags()
    {
        // Arrange
        var featuresTypeName = typeof(TestFeatures).Name;
        var permissionsTypeName = typeof(TestPermissions).Name;
        var json = $"{{\"{featuresTypeName}\":\"Feature1\",\"{permissionsTypeName}\":\"Read, Execute\"}}";

        // Act
        var collection = JsonSerializer.Deserialize<BigFlagsCollection>(json, Options);

        // Assert
        collection.Should()
                  .NotBeNull();

        collection!.HasFlag(TestFeatures.Feature1)
                   .Should()
                   .BeTrue();

        collection.HasFlag(TestPermissions.Read)
                  .Should()
                  .BeTrue();

        collection.HasFlag(TestPermissions.Execute)
                  .Should()
                  .BeTrue();
    }

    [Test]
    public void Deserialize_ShouldHandleWhitespace_InFlagNames()
    {
        // Arrange
        var typeName = typeof(TestPermissions).Name;
        var json = $"{{\"{typeName}\":\"Read,  Write   , Execute\"}}";

        // Act
        var collection = JsonSerializer.Deserialize<BigFlagsCollection>(json, Options);

        // Assert
        collection.Should()
                  .NotBeNull();

        collection!.HasFlag(TestPermissions.Read)
                   .Should()
                   .BeTrue();

        collection.HasFlag(TestPermissions.Write)
                  .Should()
                  .BeTrue();

        collection.HasFlag(TestPermissions.Execute)
                  .Should()
                  .BeTrue();
    }

    [Test]
    public void Deserialize_ShouldThrowJsonException_ForInvalidTypeName()
    {
        // Arrange
        const string JSON = "{\"NonExistent.Type\":\"Flag1\"}";

        // Act
        var act = () => JsonSerializer.Deserialize<BigFlagsCollection>(JSON, Options);

        // Assert
        act.Should()
           .Throw<JsonException>()
           .WithMessage("*Could not resolve type*");
    }

    [Test]
    public void Deserialize_ShouldThrowJsonException_ForNullTypeName()
    {
        // Arrange
        const string JSON = "{\"\":\"Feature1\"}";

        // Act
        var act = () => JsonSerializer.Deserialize<BigFlagsCollection>(JSON, Options);

        // Assert
        act.Should()
           .Throw<JsonException>()
           .WithMessage("*Type name cannot be null or empty*");
    }

    [Test]
    public void Deserialize_ShouldThrowJsonException_ForUnknownFlagName()
    {
        // Arrange
        var typeName = typeof(TestFeatures).Name;
        var json = $"{{\"{typeName}\":\"UnknownFlag\"}}";

        // Act
        var act = () => JsonSerializer.Deserialize<BigFlagsCollection>(json, Options);

        // Assert
        act.Should()
           .Throw<JsonException>()
           .WithMessage("*Unknown flag name*");
    }

    [Test]
    public void Deserialize_ShouldThrowJsonException_WhenInputIsNotObject()
    {
        // Arrange — pass an array instead of object
        const string JSON = "[\"not\",\"an\",\"object\"]";

        // Act
        var act = () => JsonSerializer.Deserialize<BigFlagsCollection>(JSON, Options);

        // Assert
        act.Should()
           .Throw<JsonException>();
    }

    [Test]
    public void Deserialize_ShouldThrowJsonException_WhenValueIsNotString()
    {
        // Arrange — value is a number instead of a string
        var typeName = typeof(TestFeatures).Name;
        var json = $"{{\"{typeName}\":123}}";

        // Act
        var act = () => JsonSerializer.Deserialize<BigFlagsCollection>(json, Options);

        // Assert
        act.Should()
           .Throw<JsonException>()
           .WithMessage("*Expected string value for flags*");
    }

    [Test]
    public void Deserialize_SingleType_ShouldRestoreFlags()
    {
        // Arrange - Create JSON manually with type name
        var typeName = typeof(TestFeatures).Name;
        var json = $"{{\"{typeName}\":\"Feature2, Feature3\"}}";

        // Act
        var collection = JsonSerializer.Deserialize<BigFlagsCollection>(json, Options);

        // Assert
        collection.Should()
                  .NotBeNull();

        collection!.HasFlag(TestFeatures.Feature2)
                   .Should()
                   .BeTrue();

        collection.HasFlag(TestFeatures.Feature3)
                  .Should()
                  .BeTrue();

        collection.HasFlag(TestFeatures.Feature1)
                  .Should()
                  .BeFalse();
    }

    [Test]
    public void RoundTrip_ComplexScenario_ShouldPreserveAllData()
    {
        // Arrange
        var original = new BigFlagsCollection();
        original.AddFlag(TestFeatures.Feature1 | TestFeatures.Feature2 | TestFeatures.Feature3 | TestFeatures.Feature4);
        original.AddFlag(TestPermissions.Read);
        original.AddFlag(TestExplicitIndices.Flag10 | TestExplicitIndices.Flag100);

        // Act
        var json = JsonSerializer.Serialize(original, Options);
        var deserialized = JsonSerializer.Deserialize<BigFlagsCollection>(json, Options);

        // Assert - Verify all flags are preserved
        deserialized.Should()
                    .NotBeNull();

        deserialized!.GetFlag<TestFeatures>()
                     .HasFlag(TestFeatures.Feature1)
                     .Should()
                     .BeTrue();

        deserialized.GetFlag<TestFeatures>()
                    .HasFlag(TestFeatures.Feature2)
                    .Should()
                    .BeTrue();

        deserialized.GetFlag<TestFeatures>()
                    .HasFlag(TestFeatures.Feature3)
                    .Should()
                    .BeTrue();

        deserialized.GetFlag<TestFeatures>()
                    .HasFlag(TestFeatures.Feature4)
                    .Should()
                    .BeTrue();

        deserialized.GetFlag<TestPermissions>()
                    .Should()
                    .Be(TestPermissions.Read);

        deserialized.GetFlag<TestExplicitIndices>()
                    .HasFlag(TestExplicitIndices.Flag10)
                    .Should()
                    .BeTrue();

        deserialized.GetFlag<TestExplicitIndices>()
                    .HasFlag(TestExplicitIndices.Flag100)
                    .Should()
                    .BeTrue();
    }

    [Test]
    public void RoundTrip_EmptyCollection_ShouldPreserve()
    {
        // Arrange
        var original = new BigFlagsCollection();

        // Act
        var json = JsonSerializer.Serialize(original, Options);
        var deserialized = JsonSerializer.Deserialize<BigFlagsCollection>(json, Options);

        // Assert
        deserialized.Should()
                    .NotBeNull();

        deserialized.Should()
                    .BeEmpty();
    }

    [Test]
    public void RoundTrip_MultipleTypes_ShouldPreserveAllFlags()
    {
        // Arrange
        var original = new BigFlagsCollection();
        original.AddFlag(TestFeatures.Feature1 | TestFeatures.Feature2);
        original.AddFlag(TestPermissions.Read | TestPermissions.Write | TestPermissions.Execute);
        original.AddFlag(TestExplicitIndices.Flag10 | TestExplicitIndices.Flag100);

        // Act
        var json = JsonSerializer.Serialize(original, Options);
        var deserialized = JsonSerializer.Deserialize<BigFlagsCollection>(json, Options);

        // Assert
        deserialized.Should()
                    .NotBeNull();

        deserialized!.GetFlag<TestFeatures>()
                     .Should()
                     .Be(TestFeatures.Feature1 | TestFeatures.Feature2);

        deserialized.GetFlag<TestPermissions>()
                    .Should()
                    .Be(TestPermissions.Read | TestPermissions.Write | TestPermissions.Execute);

        deserialized.GetFlag<TestExplicitIndices>()
                    .Should()
                    .Be(TestExplicitIndices.Flag10 | TestExplicitIndices.Flag100);
    }

    [Test]
    public void RoundTrip_SingleType_ShouldPreserveFlags()
    {
        // Arrange
        var original = new BigFlagsCollection();
        original.AddFlag(TestFeatures.Feature1 | TestFeatures.Feature3);

        // Act
        var json = JsonSerializer.Serialize(original, Options);
        var deserialized = JsonSerializer.Deserialize<BigFlagsCollection>(json, Options);

        // Assert
        deserialized.Should()
                    .NotBeNull();

        deserialized!.GetFlag<TestFeatures>()
                     .Should()
                     .Be(TestFeatures.Feature1 | TestFeatures.Feature3);
    }

    [Test]
    public void Serialize_EmptyCollection_ShouldWriteEmptyObject()
    {
        // Arrange
        var collection = new BigFlagsCollection();

        // Act
        var json = JsonSerializer.Serialize(collection, Options);

        // Assert
        json.Should()
            .Be("{}");
    }

    [Test]
    public void Serialize_ExplicitBitIndices_ShouldWork()
    {
        // Arrange
        var collection = new BigFlagsCollection();
        collection.AddFlag(TestExplicitIndices.Flag10 | TestExplicitIndices.Flag20);

        // Act
        var json = JsonSerializer.Serialize(collection, Options);

        // Assert
        json.Should()
            .Contain("Flag10, Flag20");
    }

    [Test]
    public void Serialize_InObject_ShouldWork()
    {
        // Arrange
        var obj = new TestObject
        {
            Flags = new BigFlagsCollection()
        };

        obj.Flags.AddFlag(TestFeatures.Feature2);

        // Act
        var json = JsonSerializer.Serialize(obj, Options);

        // Assert
        json.Should()
            .Contain("TestFeatures");

        json.Should()
            .Contain("Feature2");
    }

    [Test]
    public void Serialize_MultipleTypes_ShouldWriteAllTypes()
    {
        // Arrange
        var collection = new BigFlagsCollection();
        collection.AddFlag(TestFeatures.Feature1);
        collection.AddFlag(TestPermissions.Read | TestPermissions.Write);

        // Act
        var json = JsonSerializer.Serialize(collection, Options);

        // Assert
        json.Should()
            .Contain("TestFeatures");

        json.Should()
            .Contain("Feature1");

        json.Should()
            .Contain("TestPermissions");

        json.Should()
            .Contain("Read, Write");
    }

    [Test]
    public void Serialize_ShouldUseTypeName_ForTypeKey()
    {
        // Arrange
        var collection = new BigFlagsCollection();
        collection.AddFlag(TestFeatures.Feature1);

        // Act
        var json = JsonSerializer.Serialize(collection, Options);

        // Assert - Type name is used
        json.Should()
            .Contain("TestFeatures");

        json.Should()
            .NotContain("Chaos.Testing.Infrastructure");
    }

    [Test]
    public void Serialize_SingleType_ShouldWriteTypeAndFlags()
    {
        // Arrange
        var collection = new BigFlagsCollection();
        collection.AddFlag(TestFeatures.Feature1 | TestFeatures.Feature2);

        // Act
        var json = JsonSerializer.Serialize(collection, Options);

        // Assert
        json.Should()
            .Contain("Feature1, Feature2");

        json.Should()
            .Contain("TestFeatures");
    }

    private sealed class TestObject
    {
        public BigFlagsCollection Flags { get; set; } = null!;
    }
}