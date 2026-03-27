#region
using Chaos.Collections.Common;
using Chaos.Testing.Infrastructure.Definitions;
using FluentAssertions;
#endregion

namespace Chaos.Common.Tests;

public sealed class BigFlagsCollectionTests
{
    [Test]
    public void AddFlag_Generic_ShouldAddFlag_WhenTypeDoesNotExist()
    {
        // Arrange
        var collection = new BigFlagsCollection();

        // Act
        collection.AddFlag(TestFeatures.Feature1);

        // Assert
        collection.HasFlag(TestFeatures.Feature1)
                  .Should()
                  .BeTrue();
    }

    [Test]
    public void AddFlag_Generic_ShouldCombineFlags_WhenTypeists()
    {
        // Arrange
        var collection = new BigFlagsCollection();
        collection.AddFlag(TestFeatures.Feature1);

        // Act
        collection.AddFlag(TestFeatures.Feature2);

        // Assert
        collection.HasFlag(TestFeatures.Feature1)
                  .Should()
                  .BeTrue();

        collection.HasFlag(TestFeatures.Feature2)
                  .Should()
                  .BeTrue();
    }

    [Test]
    public void AddFlag_NonGeneric_ShouldAddFlag()
    {
        // Arrange
        var collection = new BigFlagsCollection();

        // Act
        collection.AddFlag(typeof(TestFeatures), TestFeatures.Feature3);

        // Assert
        collection.HasFlag(typeof(TestFeatures), TestFeatures.Feature3)
                  .Should()
                  .BeTrue();
    }

    [Test]
    public void AddFlag_NonGeneric_ShouldCombineFlags_WhenTypeists()
    {
        // Arrange
        var collection = new BigFlagsCollection();
        collection.AddFlag(typeof(TestPermissions), TestPermissions.Read);

        // Act
        collection.AddFlag(typeof(TestPermissions), TestPermissions.Write);

        // Assert
        collection.HasFlag(typeof(TestPermissions), TestPermissions.Read)
                  .Should()
                  .BeTrue();

        collection.HasFlag(typeof(TestPermissions), TestPermissions.Write)
                  .Should()
                  .BeTrue();
    }

    [Test]
    public void Clear_ShouldRemoveAllFlags()
    {
        // Arrange
        var collection = new BigFlagsCollection();
        collection.AddFlag(TestFeatures.Feature1);
        collection.AddFlag(TestPermissions.Read);

        // Act
        collection.Clear();

        // Assert
        collection.Should()
                  .BeEmpty();
    }

    [Test]
    public void Collection_ShouldBeThreadSafe()
    {
        // Arrange
        var collection = new BigFlagsCollection();
        const int THREAD_COUNT = 10;
        const int OPERATIONS_PER_THREAD = 100;

        // Act
        var tasks = Enumerable.Range(0, THREAD_COUNT)
                              .Select(_ => Task.Run(() =>
                              {
                                  for (var i = 0; i < OPERATIONS_PER_THREAD; i++)
                                  {
                                      collection.AddFlag(TestFeatures.Feature1);
                                      collection.HasFlag(TestFeatures.Feature1);
                                  }
                              }))
                              .ToArray();

        Task.WaitAll(tasks);

        // Assert
        collection.HasFlag(TestFeatures.Feature1)
                  .Should()
                  .BeTrue();
    }

    [Test]
    public void Collection_ShouldSupportMultipleMarkerTypes()
    {
        // Arrange
        var collection = new BigFlagsCollection();

        // Act
        collection.AddFlag(TestFeatures.Feature1 | TestFeatures.Feature2);
        collection.AddFlag(TestPermissions.Read | TestPermissions.Write);
        collection.AddFlag(TestExplicitIndices.Flag10);

        // Assert
        collection.GetFlag<TestFeatures>()
                  .Should()
                  .Be(TestFeatures.Feature1 | TestFeatures.Feature2);

        collection.GetFlag<TestPermissions>()
                  .Should()
                  .Be(TestPermissions.Read | TestPermissions.Write);

        collection.GetFlag<TestExplicitIndices>()
                  .Should()
                  .Be(TestExplicitIndices.Flag10);
    }

    [Test]
    public void Constructor_ShouldCreateEmptyCollection()
    {
        // Act
        var collection = new BigFlagsCollection();

        // Assert
        collection.Should()
                  .BeEmpty();
    }

    [Test]
    public void GetEnumerator_ShouldEnumerateAllTypes()
    {
        // Arrange
        var collection = new BigFlagsCollection();
        collection.AddFlag(TestFeatures.Feature1);
        collection.AddFlag(TestPermissions.Read);

        // Act
        var types = collection.Select(kvp => kvp.Key)
                              .ToList();

        // Assert
        types.Should()
             .HaveCount(2);

        types.Should()
             .Contain(typeof(TestFeatures));

        types.Should()
             .Contain(typeof(TestPermissions));
    }

    [Test]
    public void GetFlag_Generic_ShouldReturnCorrectValue_WhenTypeists()
    {
        // Arrange
        var collection = new BigFlagsCollection();
        var expected = TestFeatures.Feature1 | TestFeatures.Feature3;
        collection.AddFlag(expected);

        // Act
        var actual = collection.GetFlag<TestFeatures>();

        // Assert
        actual.Should()
              .Be(expected);
    }

    [Test]
    public void GetFlag_Generic_ShouldThrowKeyNotFoundException_WhenTypeNotFound()
    {
        // Arrange
        var collection = new BigFlagsCollection();

        // Act
        var act = () => collection.GetFlag<TestFeatures>();

        // Assert
        act.Should()
           .Throw<KeyNotFoundException>();
    }

    [Test]
    public void HasFlag_Generic_ShouldReturnFalse_WhenFlagNotSet()
    {
        // Arrange
        var collection = new BigFlagsCollection();
        collection.AddFlag(TestFeatures.Feature1);

        // Assert
        collection.HasFlag(TestFeatures.Feature2)
                  .Should()
                  .BeFalse();
    }

    [Test]
    public void HasFlag_Generic_ShouldReturnFalse_WhenTypeNotInCollection()
    {
        // Arrange
        var collection = new BigFlagsCollection();

        // Assert
        collection.HasFlag(TestFeatures.Feature1)
                  .Should()
                  .BeFalse();
    }

    [Test]
    public void HasFlag_Generic_ShouldReturnTrue_WhenFlagExists()
    {
        // Arrange
        var collection = new BigFlagsCollection();
        collection.AddFlag(TestFeatures.Feature1 | TestFeatures.Feature2);

        // Assert
        collection.HasFlag(TestFeatures.Feature1)
                  .Should()
                  .BeTrue();

        collection.HasFlag(TestFeatures.Feature2)
                  .Should()
                  .BeTrue();
    }

    [Test]
    public void HasFlag_NonGeneric_ShouldReturnFalse_WhenFlagNotSet()
    {
        // Arrange
        var collection = new BigFlagsCollection();
        collection.AddFlag(typeof(TestPermissions), TestPermissions.Read);

        // Assert
        collection.HasFlag(typeof(TestPermissions), TestPermissions.Write)
                  .Should()
                  .BeFalse();
    }

    [Test]
    public void HasFlag_NonGeneric_ShouldReturnTrue_WhenFlagExists()
    {
        // Arrange
        var collection = new BigFlagsCollection();
        collection.AddFlag(typeof(TestPermissions), TestPermissions.Execute);

        // Assert
        collection.HasFlag(typeof(TestPermissions), TestPermissions.Execute)
                  .Should()
                  .BeTrue();
    }

    [Test]
    public void Remove_ShouldRemoveEntireType()
    {
        // Arrange
        var collection = new BigFlagsCollection();
        collection.AddFlag(TestFeatures.Feature1);
        collection.AddFlag(TestPermissions.Read);

        // Act
        var removed = collection.Remove<TestFeatures>();

        // Assert
        removed.Should()
               .BeTrue();

        collection.TryGetFlag<TestFeatures>(out _)
                  .Should()
                  .BeFalse();

        collection.TryGetFlag<TestPermissions>(out _)
                  .Should()
                  .BeTrue();
    }

    [Test]
    public void Remove_ShouldReturnFalse_WhenTypeNotFound()
    {
        // Arrange
        var collection = new BigFlagsCollection();

        // Act
        var removed = collection.Remove<TestFeatures>();

        // Assert
        removed.Should()
               .BeFalse();
    }

    [Test]
    public void RemoveFlag_Generic_ShouldRemoveSpecificFlags()
    {
        // Arrange
        var collection = new BigFlagsCollection();
        collection.AddFlag(TestFeatures.Feature1 | TestFeatures.Feature2 | TestFeatures.Feature3);

        // Act
        collection.RemoveFlag(TestFeatures.Feature2);

        // Assert
        collection.HasFlag(TestFeatures.Feature1)
                  .Should()
                  .BeTrue();

        collection.HasFlag(TestFeatures.Feature2)
                  .Should()
                  .BeFalse();

        collection.HasFlag(TestFeatures.Feature3)
                  .Should()
                  .BeTrue();
    }

    [Test]
    public void RemoveFlag_NonGeneric_ShouldRemoveSpecificFlags()
    {
        // Arrange
        var collection = new BigFlagsCollection();
        collection.AddFlag(typeof(TestPermissions), TestPermissions.Read | TestPermissions.Write | TestPermissions.Execute);

        // Act
        collection.RemoveFlag(typeof(TestPermissions), TestPermissions.Write);

        // Assert
        collection.HasFlag(typeof(TestPermissions), TestPermissions.Read)
                  .Should()
                  .BeTrue();

        collection.HasFlag(typeof(TestPermissions), TestPermissions.Write)
                  .Should()
                  .BeFalse();

        collection.HasFlag(typeof(TestPermissions), TestPermissions.Execute)
                  .Should()
                  .BeTrue();
    }

    [Test]
    public void RemoveFlag_ShouldDoNothing_WhenTypeNotInCollection()
    {
        // Arrange
        var collection = new BigFlagsCollection();

        // Act
        collection.RemoveFlag(TestFeatures.Feature1);

        // Assert - Should not throw
        collection.Should()
                  .BeEmpty();
    }

    [Test]
    public void SetFlag_ShouldAddNewType_WhenNotExists()
    {
        // Arrange
        var collection = new BigFlagsCollection();

        // Act
        collection.SetFlag(TestFeatures.Feature4);

        // Assert
        collection.HasFlag(TestFeatures.Feature4)
                  .Should()
                  .BeTrue();
    }

    [Test]
    public void SetFlag_ShouldReplaceExistingValue()
    {
        // Arrange
        var collection = new BigFlagsCollection();
        collection.AddFlag(TestFeatures.Feature1 | TestFeatures.Feature2);

        // Act
        collection.SetFlag(TestFeatures.Feature3);

        // Assert
        collection.HasFlag(TestFeatures.Feature1)
                  .Should()
                  .BeFalse();

        collection.HasFlag(TestFeatures.Feature2)
                  .Should()
                  .BeFalse();

        collection.HasFlag(TestFeatures.Feature3)
                  .Should()
                  .BeTrue();
    }

    [Test]
    public void TryGetFlag_Generic_ShouldReturnFalse_WhenTypeNotFound()
    {
        // Arrange
        var collection = new BigFlagsCollection();

        // Act
        var success = collection.TryGetFlag<TestFeatures>(out var value);

        // Assert
        success.Should()
               .BeFalse();

        value.Should()
             .Be(TestFeatures.None);
    }

    [Test]
    public void TryGetFlag_Generic_ShouldReturnTrue_WhenTypeists()
    {
        // Arrange
        var collection = new BigFlagsCollection();
        var expected = TestFeatures.Feature2;
        collection.AddFlag(expected);

        // Act
        var success = collection.TryGetFlag<TestFeatures>(out var actual);

        // Assert
        success.Should()
               .BeTrue();

        actual.Should()
              .Be(expected);
    }

    [Test]
    public void TryGetFlag_NonGeneric_ShouldReturnFalse_WhenTypeNotFound()
    {
        // Arrange
        var collection = new BigFlagsCollection();

        // Act
        var success = collection.TryGetFlag(typeof(TestPermissions), out var value);

        // Assert
        success.Should()
               .BeFalse();

        value.Should()
             .BeNull();
    }

    [Test]
    public void TryGetFlag_NonGeneric_ShouldReturnTrue_WhenTypeists()
    {
        // Arrange
        var collection = new BigFlagsCollection();
        collection.AddFlag(TestPermissions.Delete);

        // Act
        var success = collection.TryGetFlag(typeof(TestPermissions), out var value);

        // Assert
        success.Should()
               .BeTrue();

        value.Should()
             .NotBeNull();

        value!.Value
              .Should()
              .Be(TestPermissions.Delete.Value);
    }
}