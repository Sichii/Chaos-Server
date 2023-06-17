using System.Diagnostics.CodeAnalysis;

namespace Chaos.Cryptography;

/// <summary>
///     Provides extensions methods for <see cref="System.Collections.Generic.IReadOnlyCollection{T}" />
/// </summary>
[ExcludeFromCodeCoverage]
public static class Crc
{
    /// <summary>
    ///     Generates a 16bit checksum for a subset of a given span.
    /// </summary>
    public static ushort Generate16(ReadOnlySpan<byte> data)
    {
        uint checkSum = 0;

        for (var i = 0; i < data.Length; ++i)
            checkSum = (ushort)(data[i] ^ (checkSum << 8) ^ Tables.TABLE16[(int)(checkSum >> 8)]);

        return (ushort)checkSum;
    }

    /// <summary>
    ///     Generates a 32bit checksum for a subset of a given span.
    /// </summary>
    public static uint Generate32(ReadOnlySpan<byte> data)
    {
        var checkSum = uint.MaxValue;

        for (var i = 0; i < data.Length; ++i)
            checkSum = (checkSum >> 8) ^ Tables.TABLE32[(int)((checkSum & byte.MaxValue) ^ data[i])];

        return ~checkSum;
    }
}