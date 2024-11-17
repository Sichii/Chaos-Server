#region
using Chaos.IO.FileSystem;
using FluentAssertions;
#endregion

namespace Chaos.IO.Tests;

public sealed class PathExTests
{
    [Test]
    public void IsSubPathOf_ShouldReturnFalse_WhenPathHasLessPartsThanParentPath()
    {
        // Arrange
        const string PATH = @"C:\Parent";
        const string PARENT_PATH = @"C:\Parent\Child\SubChild";

        // Act
        var result = PathEx.IsSubPathOf(PATH, PARENT_PATH);

        // Assert
        result.Should()
              .BeFalse();
    }

    [Test]
    public void IsSubPathOf_ShouldReturnFalse_WhenPathIsNotSubPathOfParentPath()
    {
        // Arrange
        const string PATH = @"C:\Parent\Child\SubChild";
        const string PARENT_PATH = @"C:\AnotherParent";

        // Act
        var result = PathEx.IsSubPathOf(PATH, PARENT_PATH);

        // Assert
        result.Should()
              .BeFalse();
    }

    [Test]
    public void IsSubPathOf_ShouldReturnTrue_WhenPathIsSubPathOfParentPath()
    {
        // Arrange
        const string PATH = @"C:\Parent\Child\SubChild";
        const string PARENT_PATH = @"C:\Parent";

        // Act
        var result = PathEx.IsSubPathOf(PATH, PARENT_PATH);

        // Assert
        result.Should()
              .BeTrue();
    }

    [Test]
    public void IsSubPathOf_ShouldThrow_WhenParentPathIsEmpty()
    {
        // Arrange
        const string PATH = @"C:\Parent\Child\SubChild";
        var parentPath = string.Empty;

        // Act
        var action = () => PathEx.IsSubPathOf(PATH, parentPath);

        // Assert
        action.Should()
              .ThrowExactly<ArgumentNullException>();
    }

    [Test]
    public void IsSubPathOf_ShouldThrow_WhenParentPathIsNull()
    {
        // Arrange
        const string PATH = @"C:\Parent\Child\SubChild";
        string parentPath = null!;

        // Act
        var action = () => PathEx.IsSubPathOf(PATH, parentPath);

        // Assert
        action.Should()
              .ThrowExactly<ArgumentNullException>();
    }

    [Test]
    public void IsSubPathOf_ShouldThrow_WhenPathIsEmpty()
    {
        // Arrange
        var path = string.Empty;
        const string PARENT_PATH = @"C:\Parent";

        // Act
        var action = () => PathEx.IsSubPathOf(path, PARENT_PATH);

        // Assert
        action.Should()
              .ThrowExactly<ArgumentNullException>();
    }

    [Test]
    public void IsSubPathOf_ShouldThrow_WhenPathIsNull()
    {
        // Arrange
        string path = null!;
        const string PARENT_PATH = @"C:\Parent";

        // Act
        var action = () => PathEx.IsSubPathOf(path, PARENT_PATH);

        // Assert
        action.Should()
              .ThrowExactly<ArgumentNullException>();
    }
}