using ComponentAce.Compression.Libs.zlib;

namespace Chaos.IO.Compression;

public static class ZLIB
{
    public static MemoryStream Compress(ReadOnlySpan<byte> buffer)
    {
        var compressed = new MemoryStream();
        using var compressor = new ZOutputStream(compressed, -1);

        compressor.Write(buffer);
        compressor.finish();

        return compressed;
    }

    public static void Compress(ref Span<byte> buffer)
    {
        using var compressed = Compress(buffer);
        compressed.Position = 0;

        var resultLength = compressed.Read(buffer);
        buffer = buffer[..resultLength];
    }

    public static void Compress(ref byte[] buffer)
    {
        using var compressed = Compress(buffer);
        buffer = compressed.ToArray();
    }

    public static MemoryStream Decompress(ReadOnlySpan<byte> buffer)
    {
        var uncompressed = new MemoryStream();
        using var decompressor = new ZOutputStream(uncompressed);

        decompressor.Write(buffer);
        decompressor.finish();

        return uncompressed;
    }

    public static void Decompress(ref Span<byte> buffer)
    {
        using var decompressed = Decompress(buffer);
        var resultArr = new byte[decompressed.Length];
        var resultBuffer = new Span<byte>(resultArr);
        _ = decompressed.Read(resultBuffer);
        buffer = resultBuffer;
    }

    public static void Decompress(ref byte[] buffer)
    {
        using var decompressed = Decompress(buffer);
        buffer = decompressed.ToArray();
    }
}