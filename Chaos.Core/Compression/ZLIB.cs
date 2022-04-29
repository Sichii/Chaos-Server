using ComponentAce.Compression.Libs.zlib;

namespace Chaos.Core.Compression;

public static class ZLIB
{
    public static MemoryStream Compress(byte[] buffer)
    {
        var compressed = new MemoryStream();
        using var uncompressed = new MemoryStream(buffer);
        using var compressor = new ZOutputStream(compressed, -1);

        uncompressed.Seek(0L, SeekOrigin.Begin);
        uncompressed.CopyTo(compressor);
        compressor.finish();

        return compressed;
    }

    public static void CompressInPlace(ref byte[] buffer)
    {
        using var data = Compress(buffer);
        buffer = data.ToArray();
    }

    public static MemoryStream Decompress(byte[] buffer)
    {
        var uncompressed = new MemoryStream();
        var compressed = new MemoryStream(buffer);
        using var decompressor = new ZOutputStream(uncompressed);

        compressed.CopyTo(decompressor);
        decompressor.finish();

        return uncompressed;
    }

    public static void DecompressInPlace(ref byte[] buffer)
    {
        using var data = Decompress(buffer);
        buffer = data.ToArray();
    }
}