namespace Chaos.Extensions.Common;

/// <summary>
///     Provides extension methods for <see cref="Stream" />
/// </summary>
public static class StreamExtensions
{
    /// <summary>
    ///     Lazily reads lines from a stream
    /// </summary>
    public static IEnumerable<string> ReadLines(this Stream stream)
    {
        if (!stream.CanRead)
            yield break;

        using var reader = new StreamReader(stream);

        while (reader.ReadLine() is { } line)
            yield return line;
    }

    /// <summary>
    ///     Converts a <see cref="MemoryStream" /> to a <see cref="Span{T}" />
    /// </summary>
    /// <param name="stream">
    /// </param>
    public static Span<byte> ToSpan(this MemoryStream stream)
    {
        if (stream.TryGetBuffer(out var segment))
            return segment.AsSpan();

        var buffer = new byte[stream.Length];

        stream.Position = 0;
        stream.ReadExactly(buffer);

        return buffer;
    }
}