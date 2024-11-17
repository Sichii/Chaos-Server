#region
using Chaos.Common.Synchronization;
using FluentAssertions;
#endregion

namespace Chaos.Common.Tests;

public sealed class InterlockedObjectTests
{
    [Test]
    public void Get_WithInitializedObject_ReturnsObject()
    {
        // Arrange
        var expected = new object();
        var interlockedObject = new InterlockedObject<object>();
        interlockedObject.Set(expected);

        // Act
        var result = interlockedObject.Get();

        // Assert
        result.Should()
              .BeSameAs(expected);
    }

    [Test]
    public void Get_WithNullObject_ReturnsNull()
    {
        // Arrange
        var interlockedObject = new InterlockedObject<object>();

        // Act
        var result = interlockedObject.Get();

        // Assert
        result.Should()
              .BeNull();
    }

    [Test]
    public void Set_SetsObject()
    {
        // Arrange
        var expected = new object();
        var interlockedObject = new InterlockedObject<object>();

        // Act
        interlockedObject.Set(expected);

        // Assert
        var result = interlockedObject.Get();

        result.Should()
              .BeSameAs(expected);
    }

    [Test]
    public void SetIfNull_WithNonNullObject_DoesNotSetObject()
    {
        // Arrange
        var existing = new object();
        var newValue = new object();
        var interlockedObject = new InterlockedObject<object>();
        interlockedObject.Set(existing);

        // Act
        var result = interlockedObject.SetIfNull(newValue);

        // Assert
        result.Should()
              .BeFalse();

        interlockedObject.Get()
                         .Should()
                         .BeSameAs(existing);
    }

    [Test]
    public void SetIfNull_WithNullObject_SetsObject()
    {
        // Arrange
        var expected = new object();
        var interlockedObject = new InterlockedObject<object>();

        // Act
        var result = interlockedObject.SetIfNull(expected);

        // Assert
        result.Should()
              .BeTrue();

        interlockedObject.Get()
                         .Should()
                         .BeSameAs(expected);
    }

    [Test]
    public void TryRemove_WithMatchingObject_RemovesObject()
    {
        // Arrange
        var existing = new object();
        var interlockedObject = new InterlockedObject<object>();
        interlockedObject.Set(existing);

        // Act
        var result = interlockedObject.TryRemove(existing);

        // Assert
        result.Should()
              .BeTrue();

        interlockedObject.Get()
                         .Should()
                         .BeNull();
    }

    [Test]
    public void TryRemove_WithNonMatchingObject_DoesNotRemoveObject()
    {
        // Arrange
        var existing = new object();
        var nonMatching = new object();
        var interlockedObject = new InterlockedObject<object>();
        interlockedObject.Set(existing);

        // Act
        var result = interlockedObject.TryRemove(nonMatching);

        // Assert
        result.Should()
              .BeFalse();

        interlockedObject.Get()
                         .Should()
                         .BeSameAs(existing);
    }

    [Test]
    public void TryReplace_WithMatchingObject_ReplacesObject()
    {
        // Arrange
        var existing = new object();
        var replacement = new object();
        var interlockedObject = new InterlockedObject<object>();
        interlockedObject.Set(existing);

        // Act
        var result = interlockedObject.TryReplace(replacement, existing);

        // Assert
        result.Should()
              .BeTrue();

        interlockedObject.Get()
                         .Should()
                         .BeSameAs(replacement);
    }

    [Test]
    public void TryReplace_WithNonMatchingObject_DoesNotReplaceObject()
    {
        // Arrange
        var existing = new object();
        var replacement = new object();
        var nonMatching = new object();
        var interlockedObject = new InterlockedObject<object>();
        interlockedObject.Set(existing);

        // Act
        var result = interlockedObject.TryReplace(replacement, nonMatching);

        // Assert
        result.Should()
              .BeFalse();

        interlockedObject.Get()
                         .Should()
                         .BeSameAs(existing);
    }
}