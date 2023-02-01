using ComponentAce.Compression.Libs.zlib;

namespace Chaos.IO.Compression;

public static class ZLIB
{
    public static MemoryStream Compress(ReadOnlySpan<byte> buffer)
    {
        var ret = new MemoryStream();
        using var compressed = new MemoryStream();
        using var compressor = new ZOutputStream(compressed, zlibConst.Z_DEFAULT_COMPRESSION);

        compressor.Write(buffer);
        compressor.finish();

        compressed.Position = 0;
        compressed.CopyTo(ret);
        ret.Position = 0;

        return ret;
    }

    public static void Compress(ref Span<byte> buffer)
    {
        using var compressed = Compress(buffer);
        compressed.Position = 0;
        var resultArr = new byte[compressed.Length];
        var resultBuffer = new Span<byte>(resultArr);

        _ = compressed.Read(resultBuffer);
        buffer = resultBuffer;
    }

    public static void Compress(ref byte[] buffer)
    {
        using var compressed = Compress(buffer);
        buffer = compressed.ToArray();
    }

    public static MemoryStream Decompress(ReadOnlySpan<byte> buffer)
    {
        var ret = new MemoryStream();
        using var outData = new MemoryStream();
        using var decompressor = new ZOutputStream(outData);

        decompressor.Write(buffer);
        decompressor.finish();

        outData.Position = 0;
        outData.CopyTo(ret);
        ret.Position = 0;

        return ret;
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