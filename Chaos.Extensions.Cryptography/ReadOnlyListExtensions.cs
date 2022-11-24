using Chaos.Cryptography;

namespace Chaos.Extensions.Cryptography;

/// <summary>
///     Provides extensions methods for <see cref="IReadOnlyList{T}"/>
/// </summary>
public static class ReadOnlyListExtensions
{
    /// <summary>
    ///     Generates a 16bit checksum for a given byte array.
    /// </summary>
    public static ushort Generate16(this IReadOnlyList<byte> data) => Generate16(data, 0, data.Count);

    /// <summary>
    ///     Generates a 16bit checksum for a subset of a given byte array.
    /// </summary>
    public static ushort Generate16(this IReadOnlyList<byte> data, int index, int length)
    {
        uint checkSum = 0;

        for (var i = 0; i < length; ++i)
            checkSum = (ushort)(data[index + i] ^ (checkSum << 8) ^ Tables.TABLE16[(int)(checkSum >> 8)]);

        return (ushort)checkSum;
    }

    /// <summary>
    ///     Generates a 32bit checksum for a given byte array.
    /// </summary>
    public static uint Generate32(this IReadOnlyList<byte> data) => Generate32(data, 0, data.Count);

    /// <summary>
    ///     Generates a 32bit checksum for a subset of a given byte array.
    /// </summary>
    public static uint Generate32(this IReadOnlyList<byte> data, int index, int length)
    {
        var checkSum = uint.MaxValue;

        for (var i = index; i < length; ++i)
            checkSum = (checkSum >> 8) ^ Tables.TABLE32[(int)((checkSum & byte.MaxValue) ^ data[i])];

        return checkSum;
    }
}