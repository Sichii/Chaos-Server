using Chaos.Common.Utilities;
using FluentAssertions;
using Xunit;

namespace Chaos.Common.Tests;

public sealed class ShallowCopyTests
{
    [Fact]
    public void Copy_ShouldReflectChangesInOriginal()
    {
        // Arrange
        var subClass = new MockCopyableSub { SubValue = 10 };
        var original = new MockCopyable { Value = 5, MockCopyableSub = subClass };
        var copy = ShallowCopy<MockCopyable>.Create(original);

        // Act
        original.Value = 7;
        original.MockCopyableSub.SubValue = 15;

        // Assert
        copy.Value.Should().NotBe(original.Value); // Value type
        copy.MockCopyableSub.SubValue.Should().Be(original.MockCopyableSub.SubValue); // Reference type
    }

    [Fact]
    public void Create_ShouldReturnShallowCopy()
    {
        // Arrange
        var subClass = new MockCopyableSub { SubValue = 10 };
        var original = new MockCopyable { Value = 5, MockCopyableSub = subClass };

        // Act
        var copy = ShallowCopy<MockCopyable>.Create(original);

        // Assert
        copy.Should().NotBeSameAs(original);
        copy.Value.Should().Be(original.Value);
        copy.MockCopyableSub.Should().BeSameAs(original.MockCopyableSub);
    }

    [Fact]
    public void Merge_ShouldShallowMergeObjects()
    {
        // Arrange
        var subClass1 = new MockCopyableSub { SubValue = 10 };
        var original = new MockCopyable { Value = 5, MockCopyableSub = subClass1 };

        var subClass2 = new MockCopyableSub { SubValue = 20 };
        var target = new MockCopyable { Value = 10, MockCopyableSub = subClass2 };

        // Act
        ShallowCopy<MockCopyable>.Merge(original, target);

        // Assert
        target.Value.Should().Be(original.Value);
        target.MockCopyableSub.Should().BeSameAs(original.MockCopyableSub);
        target.MockCopyableSub.SubValue.Should().Be(original.MockCopyableSub.SubValue);
    }

    internal sealed class MockCopyable
    {
        public MockCopyableSub MockCopyableSub { get; set; } = null!;
        public int Value { get; set; }
    }

    internal sealed class MockCopyableSub
    {
        public int SubValue { get; set; }
    }
}