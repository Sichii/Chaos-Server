using Chaos.IO.FileSystem;
using FluentAssertions;
using Xunit;

namespace Chaos.IO.Tests;

public sealed class FileExTests
{
    [Fact]
    public void The_Method_Creates_A_New_File_And_Writes_Text_To_It_Successfully()
    {
        // Arrange
        var path = "test.txt";
        var text = "This is a test";

        // Act
        FileEx.SafeWriteAllText(path, text);

        // Assert
        File.Exists(path)
            .Should()
            .BeTrue();

        File.ReadAllText(path)
            .Should()
            .Be(text);
    }

    [Fact]
    public void The_Method_Handles_Empty_Text_Input_Successfully()
    {
        // Arrange
        var path = "test.txt";
        var text = "";

        // Act
        FileEx.SafeWriteAllText(path, text);

        // Assert
        File.ReadAllText(path)
            .Should()
            .Be(text);
    }

    [Fact]
    public void The_Method_Handles_Long_Text_Input_Successfully()
    {
        // Arrange
        var path = "test.txt";
        var text = new string('a', 1000000);

        // Act
        FileEx.SafeWriteAllText(path, text);

        // Assert
        File.ReadAllText(path)
            .Should()
            .Be(text);
    }

    [Fact]
    public void The_Method_Overwrites_An_Existing_File_With_New_Text_Successfully()
    {
        // Arrange
        var path = "test.txt";
        var initialText = "Initial text";
        var newText = "New text";

        File.WriteAllText(path, initialText);

        // Act
        FileEx.SafeWriteAllText(path, newText);

        // Assert
        File.ReadAllText(path)
            .Should()
            .Be(newText);
    }

    [Fact]
    public void The_Method_Throws_An_Exception_When_The_File_Path_Contains_Invalid_Characters()
    {
        // Arrange
        var path = "test?.txt";
        var text = "This is a test";

        // Act
        var act = () => FileEx.SafeWriteAllText(path, text);

        // Assert
        act.Should()
           .Throw<IOException>();
    }

    [Fact]
    public void The_Method_Throws_An_Exception_When_The_File_Path_Is_Null()
    {
        // Arrange
        string? path = null;
        var text = "This is a test";

        // Act
        var act = () => FileEx.SafeWriteAllText(path, text);

        // Assert
        act.Should()
           .Throw<ArgumentNullException>();
    }

    [Fact]
    public void The_Method_Writes_All_Text_To_A_File_Successfully()
    {
        // Arrange
        var path = "test.txt";
        var text = "This is a test";

        // Act
        FileEx.SafeWriteAllText(path, text);

        // Assert
        File.ReadAllText(path)
            .Should()
            .Be(text);
    }
}