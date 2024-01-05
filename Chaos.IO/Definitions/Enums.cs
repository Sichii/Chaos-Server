namespace Chaos.IO.Definitions;

/// <summary>
///     A value representing the endianness to use when reading/writing data
/// </summary>
public enum Endianness
{
    /// <summary>
    ///     Little endian is a term used to describe the byte order of multi-byte data types in computer memory. In little
    ///     endian, the least significant byte (LSB) is stored at the lowest memory address, and the most significant byte
    ///     (MSB) is stored at the highest memory address. In other words, the bytes are ordered from the smallest to the
    ///     largest, based on their significance.
    /// </summary>
    LittleEndian,

    /// <summary>
    ///     Big endian is a term used to describe the byte order of multi-byte data types in computer memory. In big endian,
    ///     the most significant byte (MSB) is stored at the lowest memory address, and the least significant byte (LSB) is
    ///     stored at the highest memory address. In other words, the bytes are ordered from the largest to the smallest, based
    ///     on their significance.
    /// </summary>
    BigEndian
}