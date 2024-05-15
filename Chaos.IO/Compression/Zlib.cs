using System.IO.Compression;

namespace Chaos.IO.Compression;

/// <summary>
///     Provides methods for compressing and decompressing data using the ZLIB algorithm.
/// </summary>
public static class Zlib
{
    /// <summary>
    ///     Compresses the specified buffer and returns the compressed data as a MemoryStream.
    /// </summary>
    /// <param name="buffer">
    ///     The buffer to compress.
    /// </param>
    /// <returns>
    ///     A MemoryStream containing the compressed data.
    /// </returns>
    public static MemoryStream Compress(ReadOnlySpan<byte> buffer)
    {
        var compressed = new MemoryStream();
        using var compressor = new ZLibStream(compressed, CompressionMode.Compress);

        compressor.Write(buffer);

        return compressed;
    }

    /// <summary>
    ///     Compresses the specified buffer in-place.
    /// </summary>
    /// <param name="buffer">
    ///     The buffer to compress. The compressed data will replace the original data in the buffer.
    /// </param>
    public static void Compress(ref Span<byte> buffer)
    {
        using var compressed = Compress(buffer);

        buffer = compressed.ToArray();
    }

    /// <summary>
    ///     Compresses the specified buffer in-place.
    /// </summary>
    /// <param name="buffer">
    ///     The buffer to compress. The compressed data will replace the original data in the buffer.
    /// </param>
    public static void Compress(ref byte[] buffer)
    {
        using var compressed = Compress(buffer);

        buffer = compressed.ToArray();
    }

    /// <summary>
    ///     Decompresses the specified buffer and returns the decompressed data as a MemoryStream.
    /// </summary>
    /// <param name="buffer">
    ///     The buffer to decompress.
    /// </param>
    /// <returns>
    ///     A MemoryStream containing the decompressed data.
    /// </returns>
    public static MemoryStream Decompress(ReadOnlySpan<byte> buffer)
    {
        var decompressed = new MemoryStream();
        using var compressed = new MemoryStream();
        using var decompressor = new ZLibStream(compressed, CompressionMode.Decompress);

        compressed.Write(buffer);
        compressed.Seek(0, SeekOrigin.Begin);
        decompressor.CopyTo(decompressed);

        return decompressed;
    }

    /// <summary>
    ///     Decompresses the specified buffer in-place.
    /// </summary>
    /// <param name="buffer">
    ///     The buffer to decompress. The decompressed data will replace the original data in the buffer.
    /// </param>
    public static void Decompress(ref Span<byte> buffer)
    {
        using var decompressed = Decompress(buffer);

        buffer = decompressed.ToArray();
    }

    /// <summary>
    ///     Decompresses the specified buffer in-place.
    /// </summary>
    /// <param name="buffer">
    ///     The buffer to decompress. The decompressed data will replace the original data in the buffer.
    /// </param>
    public static void Decompress(ref byte[] buffer)
    {
        using var decompressed = Decompress(buffer);

        buffer = decompressed.ToArray();
    }
}