using System.Text;
using Chaos.IO.Compression;
using FluentAssertions;
using Xunit;

namespace Chaos.IO.Tests;

public sealed class ZlibTests
{
    [Fact]
    public void CompressDecompress_ByteArray_ShouldReturnSameDataAfterDecompress()
    {
        // Arrange
        const string ORIGINAL_DATA = "This is some test data";
        var buffer = Encoding.ASCII.GetBytes(ORIGINAL_DATA);

        // Act
        Zlib.Compress(ref buffer);
        Zlib.Decompress(ref buffer);

        // Assert
        var decompressedString = Encoding.ASCII.GetString(buffer);

        decompressedString.Should()
                          .Be(ORIGINAL_DATA);
    }

    [Fact]
    public void CompressDecompress_ReadOnlySpan_ShouldReturnSameDataAfterDecompress()
    {
        // Arrange
        const string ORIGINAL_DATA = "This is some test data";
        var buffer = Encoding.ASCII.GetBytes(ORIGINAL_DATA);
        var readOnlySpan = new ReadOnlySpan<byte>(buffer);

        // Act
        var compressedStream = Zlib.Compress(readOnlySpan);
        var decompressedStream = Zlib.Decompress(compressedStream.ToArray());

        // Assert
        var decompressedString = Encoding.ASCII.GetString(decompressedStream.ToArray());

        decompressedString.Should()
                          .Be(ORIGINAL_DATA);
    }

    [Fact]
    public void CompressDecompress_Span_ShouldReturnSameDataAfterDecompress()
    {
        // Arrange
        const string ORIGINAL_DATA = "This is some test data";
        var buffer = Encoding.ASCII.GetBytes(ORIGINAL_DATA);
        var span = new Span<byte>(buffer);

        // Act
        Zlib.Compress(ref span);
        Zlib.Decompress(ref span);

        // Assert
        var decompressedString = Encoding.ASCII.GetString(span.ToArray());

        decompressedString.Should()
                          .Be(ORIGINAL_DATA);
    }
}