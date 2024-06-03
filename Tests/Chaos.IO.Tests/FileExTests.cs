using Chaos.IO.Exceptions;
using Chaos.IO.FileSystem;
using FluentAssertions;
using Xunit;

namespace Chaos.IO.Tests;

public sealed class FileExTests
{
    [Fact]
    public void SafeOpenRead_Should_CaptureExceptions_InAggregateException()
    {
        // Arrange
        var guid = Guid.NewGuid()
                       .ToString();
        var path = $"{guid}.txt";

        // Act
        Action act = () => FileEx.SafeOpenRead<string>(path, _ => null!);

        // Assert
        var exception = act.Should()
                           .Throw<AggregateException>()
                           .Which;

        exception.InnerExceptions
                 .Should()
                 .AllBeAssignableTo<FileNotFoundException>();
    }

    [Fact]
    public void SafeOpenRead_Should_Retry_RetryableExceptions()
    {
        // Arrange
        var guid = Guid.NewGuid()
                       .ToString();
        var path = $"{guid}.txt";
        var tempPath = $"{guid}.txt.temp";
        var bakPath = $"{guid}.txt.bak";
        var content = "Temp file content";
        File.WriteAllText(path, content);
        File.WriteAllText(bakPath, content);

        // Act
        var result = FileEx.SafeOpenRead(
            path,
            stream =>
            {
                var fileName = Path.GetFileName(stream.Name);

                if (fileName == path)
                    throw new RetryableException("");

                if (fileName == tempPath)
                    return false;

                return true;
            });

        // Assert
        result.Should()
              .Be(true);

        // Cleanup
        File.Delete(path);
        File.Delete(bakPath);
    }

    [Fact]
    public void SafeOpenRead_Should_ReturnResult_When_MainAndTempFilesMissing_BackupFileExists()
    {
        // Arrange
        var guid = Guid.NewGuid()
                       .ToString();
        var path = $"{guid}.txt";
        var backPath = $"{guid}.txt.bak";
        var content = "Backup file content";
        File.WriteAllText(backPath, content);

        // Act
        var result = FileEx.SafeOpenRead(
            path,
            stream =>
            {
                using var reader = new StreamReader(stream);

                return reader.ReadToEnd();
            });

        // Assert
        result.Should()
              .Be(content);

        // Cleanup
        File.Delete(backPath);
    }

    [Fact]
    public void SafeOpenRead_Should_ReturnResult_When_MainFileExists()
    {
        // Arrange
        var guid = Guid.NewGuid()
                       .ToString();
        var path = $"{guid}.txt";
        var content = "Main file content";
        File.WriteAllText(path, content);

        // Act
        var result = FileEx.SafeOpenRead(
            path,
            stream =>
            {
                using var reader = new StreamReader(stream);

                return reader.ReadToEnd();
            });

        // Assert
        result.Should()
              .Be(content);

        // Cleanup
        File.Delete(path);
    }

    [Fact]
    public void SafeOpenRead_Should_ReturnResult_When_MainFileMissing_TempFileExists()
    {
        // Arrange
        var guid = Guid.NewGuid()
                       .ToString();
        var path = $"{guid}.txt";
        var tempPath = $"{guid}.txt.temp";
        var content = "Temp file content";
        File.WriteAllText(tempPath, content);

        // Act
        var result = FileEx.SafeOpenRead(
            path,
            stream =>
            {
                using var reader = new StreamReader(stream);

                return reader.ReadToEnd();
            });

        // Assert
        result.Should()
              .Be(content);

        // Cleanup
        File.Delete(tempPath);
    }

    [Fact]
    public void SafeOpenRead_Should_ThrowAggregateException_When_AllFilesMissing()
    {
        // Arrange
        var guid = Guid.NewGuid()
                       .ToString();
        var path = $"{guid}.txt";

        // Act
        Action act = () => FileEx.SafeOpenRead<string>(path, _ => null!);

        // Assert
        act.Should()
           .Throw<AggregateException>()
           .WithMessage("Failed to read file, temp file, or backup file. See inner exceptions for details. *");
    }

    [Fact]
    public async Task SafeOpenReadAsync_Should_CaptureExceptions_InAggregateException()
    {
        // Arrange
        var guid = Guid.NewGuid()
                       .ToString();
        var path = $"{guid}.txt";

        // Act
        var act = async () => await FileEx.SafeOpenReadAsync<string>(path, _ => null!);

        // Assert
        (await act.Should()
                  .ThrowAsync<AggregateException>()).Which
                                                    .InnerExceptions
                                                    .Should()
                                                    .AllBeAssignableTo<FileNotFoundException>();
    }

    [Fact]
    public async Task SafeOpenReadAsync_Should_Retry_RetryableExceptions()
    {
        // Arrange
        var guid = Guid.NewGuid()
                       .ToString();
        var path = $"{guid}.txt";
        var tempPath = $"{guid}.txt.temp";
        var bakPath = $"{guid}.txt.bak";
        var content = "Temp file content";
        await File.WriteAllTextAsync(path, content);
        await File.WriteAllTextAsync(bakPath, content);

        // Act
        var result = await FileEx.SafeOpenReadAsync(
            path,
            stream =>
            {
                var fileName = Path.GetFileName(stream.Name);

                if (fileName == path)
                    throw new RetryableException("");

                if (fileName == tempPath)
                    return Task.FromResult(false);

                return Task.FromResult(true);
            });

        // Assert
        result.Should()
              .Be(true);

        // Cleanup
        File.Delete(path);
        File.Delete(bakPath);
    }

    [Fact]
    public async Task SafeOpenReadAsync_Should_ReturnResult_When_MainAndTempFilesMissing_BackupFileExists()
    {
        // Arrange
        var guid = Guid.NewGuid()
                       .ToString();
        var path = $"{guid}.txt";
        var backPath = $"{guid}.txt.bak";
        var content = "Backup file content";
        await File.WriteAllTextAsync(backPath, content);

        // Act
        var result = await FileEx.SafeOpenReadAsync(
            path,
            async stream =>
            {
                using var reader = new StreamReader(stream);

                return await reader.ReadToEndAsync();
            });

        // Assert
        result.Should()
              .Be(content);

        // Cleanup
        File.Delete(backPath);
    }

    [Fact]
    public async Task SafeOpenReadAsync_Should_ReturnResult_When_MainFileExists()
    {
        // Arrange
        var guid = Guid.NewGuid()
                       .ToString();
        var path = $"{guid}.txt";
        var content = "Main file content";
        await File.WriteAllTextAsync(path, content);

        // Act
        var result = await FileEx.SafeOpenReadAsync(
            path,
            async stream =>
            {
                using var reader = new StreamReader(stream);

                return await reader.ReadToEndAsync();
            });

        // Assert
        result.Should()
              .Be(content);

        // Cleanup
        File.Delete(path);
    }

    [Fact]
    public async Task SafeOpenReadAsync_Should_ReturnResult_When_MainFileMissing_TempFileExists()
    {
        // Arrange
        var guid = Guid.NewGuid()
                       .ToString();
        var path = $"{guid}.txt";
        var tempPath = $"{guid}.txt.temp";
        var content = "Temp file content";
        await File.WriteAllTextAsync(tempPath, content);

        // Act
        var result = await FileEx.SafeOpenReadAsync(
            path,
            async stream =>
            {
                using var reader = new StreamReader(stream);

                return await reader.ReadToEndAsync();
            });

        // Assert
        result.Should()
              .Be(content);

        // Cleanup
        File.Delete(tempPath);
    }

    [Fact]
    public async Task SafeOpenReadAsync_Should_ThrowAggregateException_When_AllFilesMissing()
    {
        // Arrange
        var guid = Guid.NewGuid()
                       .ToString();
        var path = $"{guid}.txt";

        // Act
        var act = async () => await FileEx.SafeOpenReadAsync<string>(path, _ => null!);

        // Assert
        await act.Should()
                 .ThrowAsync<AggregateException>()
                 .WithMessage("Failed to read file, temp file, or backup file. See inner exceptions for details. *");
    }

    [Fact]
    public void SafeWriteAllText_Creates_A_New_File_And_Writes_Text_To_It_Successfully()
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
    public void SafeWriteAllText_Handles_Empty_Text_Input_Successfully()
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
    public void SafeWriteAllText_Handles_Long_Text_Input_Successfully()
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
    public void SafeWriteAllText_Overwrites_An_Existing_File_With_New_Text_Successfully()
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
    public void SafeWriteAllText_Throws_An_Exception_When_The_File_Path_Contains_Invalid_Characters()
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
    public void SafeWriteAllText_Throws_An_Exception_When_The_File_Path_Is_Null()
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
    public void SafeWriteallText_Writes_All_Text_To_A_File_Successfully()
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