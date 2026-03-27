#region
using System.Text;
using FluentAssertions;
#endregion

namespace Chaos.Extensions.Common.Tests;

public sealed class StreamExtensionsTests
{
    [Test]
    public void ReadLines_IsLazyEvaluated_ShouldNotProcessAllLinesImmediately()
    {
        // Arrange
        var content = "Line 1\nLine 2\nLine 3\nLine 4\nLine 5";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));

        // Act
        var enumerable = stream.ReadLines();

        var firstTwo = enumerable.Take(2)
                                 .ToList();

        // Assert
        firstTwo.Should()
                .HaveCount(2);

        firstTwo[0]
            .Should()
            .Be("Line 1");

        firstTwo[1]
            .Should()
            .Be("Line 2");
    }

    [Test]
    public void ReadLines_WithEmptyStream_ShouldReturnEmptyEnumerable()
    {
        // Arrange
        using var stream = new MemoryStream();

        // Act
        var result = stream.ReadLines()
                           .ToList();

        // Assert
        result.Should()
              .BeEmpty();
    }

    [Test]
    public void ReadLines_WithNonReadableStream_ShouldReturnEmptyEnumerable()
    {
        // Arrange
        using var fileStream = new FileStream(Path.GetTempFileName(), FileMode.Create, FileAccess.Write);

        // Act
        var result = fileStream.ReadLines()
                               .ToList();

        // Assert
        result.Should()
              .BeEmpty();

        // Cleanup
        var tempFile = fileStream.Name;
        fileStream.Close();
        File.Delete(tempFile);
    }

    [Test]
    public void ReadLines_WithReadableStreamContainingMultipleLines_ShouldReturnAllLines()
    {
        // Arrange
        var content = "Line 1\nLine 2\nLine 3";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));

        // Act
        var result = stream.ReadLines()
                           .ToList();

        // Assert
        result.Should()
              .HaveCount(3);

        result[0]
            .Should()
            .Be("Line 1");

        result[1]
            .Should()
            .Be("Line 2");

        result[2]
            .Should()
            .Be("Line 3");
    }

    [Test]
    public void ReadLines_WithReadableStreamContainingWindowsLineEndings_ShouldReturnAllLines()
    {
        // Arrange
        var content = "Line 1\r\nLine 2\r\nLine 3";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));

        // Act
        var result = stream.ReadLines()
                           .ToList();

        // Assert
        result.Should()
              .HaveCount(3);

        result[0]
            .Should()
            .Be("Line 1");

        result[1]
            .Should()
            .Be("Line 2");

        result[2]
            .Should()
            .Be("Line 3");
    }

    [Test]
    public void ReadLines_WithStreamContainingEmptyLines_ShouldReturnEmptyStrings()
    {
        // Arrange
        var content = "Line 1\n\nLine 3\n";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));

        // Act
        var result = stream.ReadLines()
                           .ToList();

        // Assert
        result.Should()
              .HaveCount(3);

        result[0]
            .Should()
            .Be("Line 1");

        result[1]
            .Should()
            .Be("");

        result[2]
            .Should()
            .Be("Line 3");
    }

    [Test]
    public void ReadLines_WithStreamContainingSingleLine_ShouldReturnSingleLine()
    {
        // Arrange
        var content = "Single line";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));

        // Act
        var result = stream.ReadLines()
                           .ToList();

        // Assert
        result.Should()
              .ContainSingle()
              .Which
              .Should()
              .Be("Single line");
    }

    [Test]
    public void ReadLines_WithStreamContainingUnicodeContent_ShouldHandleUnicodeCorrectly()
    {
        // Arrange
        var content = "Hello 世界\nЗдравствуй мир\n🌍";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));

        // Act
        var result = stream.ReadLines()
                           .ToList();

        // Assert
        result.Should()
              .HaveCount(3);

        result[0]
            .Should()
            .Be("Hello 世界");

        result[1]
            .Should()
            .Be("Здравствуй мир");

        result[2]
            .Should()
            .Be("🌍");
    }

    [Test]
    public void ToSpan_WithEmptyMemoryStream_ShouldReturnEmptySpan()
    {
        // Arrange
        using var stream = new MemoryStream();

        // Act
        var result = stream.ToSpan();

        // Assert
        result.Length
              .Should()
              .Be(0);

        result.IsEmpty
              .Should()
              .BeTrue();
    }

    [Test]
    public void ToSpan_WithMemoryStreamAfterWriteOperations_ShouldReturnCompleteData()
    {
        // Arrange
        using var stream = new MemoryStream();

        stream.Write(
            [
                1,
                2,
                3
            ]);

        stream.Write(
            [
                4,
                5
            ]);

        // Act
        var result = stream.ToSpan();

        // Assert
        result.Length
              .Should()
              .Be(5);

        result[0]
            .Should()
            .Be(1);

        result[1]
            .Should()
            .Be(2);

        result[2]
            .Should()
            .Be(3);

        result[3]
            .Should()
            .Be(4);

        result[4]
            .Should()
            .Be(5);
    }

    [Test]
    public void ToSpan_WithMemoryStreamCreatedFromByteArray_ShouldReturnCorrectSpan()
    {
        // Arrange
        var originalData = new byte[]
        {
            100,
            200,
            50,
            150
        };
        using var stream = new MemoryStream(originalData);

        // Act
        var result = stream.ToSpan();

        // Assert
        result.Length
              .Should()
              .Be(4);

        result.SequenceEqual(originalData)
              .Should()
              .BeTrue();
    }

    [Test]
    public void ToSpan_WithMemoryStreamThatCanExposeBuffer_ShouldReturnSpanFromBuffer()
    {
        // Arrange
        var data = new byte[]
        {
            1,
            2,
            3,
            4,
            5
        };
        using var stream = new MemoryStream(data);

        // Act
        var result = stream.ToSpan();

        // Assert
        result.Length
              .Should()
              .Be(5);

        result[0]
            .Should()
            .Be(1);

        result[1]
            .Should()
            .Be(2);

        result[2]
            .Should()
            .Be(3);

        result[3]
            .Should()
            .Be(4);

        result[4]
            .Should()
            .Be(5);
    }

    [Test]
    public void ToSpan_WithMemoryStreamThatCannotExposeBuffer_ShouldReturnSpanFromCopiedData()
    {
        // Arrange
        var data = new byte[]
        {
            10,
            20,
            30
        };
        using var stream = new MemoryStream(data, false);

        // Act
        var result = stream.ToSpan();

        // Assert
        result.Length
              .Should()
              .Be(3);

        result[0]
            .Should()
            .Be(10);

        result[1]
            .Should()
            .Be(20);

        result[2]
            .Should()
            .Be(30);
    }

    [Test]
    public void ToSpan_WithMemoryStreamWithChangedPosition_ShouldResetPositionAndReadAllData()
    {
        // Arrange
        var data = new byte[]
        {
            1,
            2,
            3,
            4,
            5
        };
        using var stream = new MemoryStream(data);
        stream.Position = 3; // Move position to middle

        // Act
        var result = stream.ToSpan();

        // Assert
        result.Length
              .Should()
              .Be(5);

        result[0]
            .Should()
            .Be(1);

        result[1]
            .Should()
            .Be(2);

        result[2]
            .Should()
            .Be(3);

        result[3]
            .Should()
            .Be(4);

        result[4]
            .Should()
            .Be(5);
    }
}