using System.Buffers.Binary;
using System.Runtime.InteropServices;
using System.Text;
using Chaos.IO.Definitions;

namespace Chaos.IO.Memory;

/// <summary>
///     A ref struct for reading data from a span of bytes with customizable endianness and encoding.
/// </summary>
public ref struct SpanReader
{
    private readonly Span<byte> Buffer;
    private readonly bool IsLittleEndian;

    /// <summary>
    ///     Gets the current position in the Span.
    /// </summary>
    public int Position { get; set; }

    /// <summary>
    ///     Gets the Encoding used for string operations.
    /// </summary>
    public Encoding Encoding { get; }

    /// <summary>
    ///     Gets the endianness of the data.
    /// </summary>
    public Endianness Endianness { get; }

    /// <summary>
    ///     Gets a value indicating whether the reader reached the end of the Span.
    /// </summary>
    public readonly bool EndOfSpan => Position >= Buffer.Length;

    /// <summary>
    ///     Gets the number of remaining bytes in the Span.
    /// </summary>
    public readonly int Remaining => Buffer.Length - Position;

    /// <summary>
    ///     Initializes a new instance of the <see cref="SpanReader" /> struct.
    /// </summary>
    /// <param name="encoding">The encoding to use for string operations.</param>
    /// <param name="buffer">The span of bytes to read from.</param>
    /// <param name="endianness">The endianness of the data (default is BigEndian).</param>
    public SpanReader(Encoding encoding, in Span<byte> buffer, Endianness endianness = Endianness.BigEndian)
    {
        Buffer = buffer;
        Encoding = encoding;
        Position = 0;
        Endianness = endianness;
        IsLittleEndian = Endianness == Endianness.LittleEndian;
    }

    /// <summary>
    ///     Reads a list of strings separated by null terminators from the current position in the Span.
    /// </summary>
    public List<string> ReadArgs()
    {
        var args = new List<string>();

        while (!EndOfSpan)
            args.Add(ReadString());

        return args;
    }

    /// <summary>
    ///     Reads a list of strings with 8-bit length prefixes from the current position in the Span.
    /// </summary>
    public List<string> ReadArgs8()
    {
        var args = new List<string>();

        while (!EndOfSpan)
            args.Add(ReadString8());

        return args;
    }

    /// <summary>
    ///     Reads a boolean value from the current position in the Span.
    /// </summary>
    public bool ReadBoolean()
    {
        var ret = MemoryMarshal.Read<bool>(Buffer[Position..]);
        Position++;

        return ret;
    }

    /// <summary>
    ///     Reads a byte from the current position in the Span.
    /// </summary>
    public byte ReadByte()
    {
        var ret = MemoryMarshal.Read<byte>(Buffer[Position..]);
        Position++;

        return ret;
    }

    /// <summary>
    ///     Reads the specified number of bytes from the current position in the Span.
    /// </summary>
    public byte[] ReadBytes(int length)
    {
        if (Remaining < length)
            throw new EndOfStreamException();

        var start = Position;
        var end = Position += length;

        return Buffer[start..end]
            .ToArray();
    }

    /// <summary>
    ///     Reads all remaining bytes from the current position in the Span.
    /// </summary>
    public byte[] ReadData() => ReadBytes(Remaining);

    /// <summary>
    ///     Reads a byte array with a 16-bit length prefix from the current position in the Span.
    /// </summary>
    public byte[] ReadData16()
    {
        int lengthPrefix = ReadUInt16();

        return ReadBytes(lengthPrefix);
    }

    /// <summary>
    ///     Reads a byte array with an 8-bit length prefix from the current position in the Span.
    /// </summary>
    public byte[] ReadData8()
    {
        int lengthPrefix = ReadByte();

        return ReadBytes(lengthPrefix);
    }

    /// <summary>
    ///     Reads a 16-bit signed integer from the current position in the Span.
    /// </summary>
    public short ReadInt16()
    {
        var ret = MemoryMarshal.Read<short>(Buffer[Position..]);

        if (!IsLittleEndian && BitConverter.IsLittleEndian)
            ret = BinaryPrimitives.ReverseEndianness(ret);

        Position += sizeof(short);

        return ret;
    }

    /// <summary>
    ///     Reads a 32-bit signed integer from the current position in the Span.
    /// </summary>
    public int ReadInt32()
    {
        var ret = MemoryMarshal.Read<int>(Buffer[Position..]);

        if (!IsLittleEndian && BitConverter.IsLittleEndian)
            ret = BinaryPrimitives.ReverseEndianness(ret);

        Position += sizeof(int);

        return ret;
    }

    /// <summary>
    ///     Reads a tuple of two 16-bit unsigned integers from the current position in the Span.
    /// </summary>
    public (ushort X, ushort Y) ReadPoint16() => (ReadUInt16(), ReadUInt16());

    /// <summary>
    ///     Reads a tuple of two 8-bit unsigned integers from the current position in the Span.
    /// </summary>
    public (byte X, byte Y) ReadPoint8() => (ReadByte(), ReadByte());

    /// <summary>
    ///     Reads an 8-bit signed byte from the current position in the Span.
    /// </summary>
    public sbyte ReadSByte()
    {
        var ret = MemoryMarshal.Read<sbyte>(Buffer[Position..]);
        Position++;

        return ret;
    }

    /// <summary>
    ///     Reads a string separated ending with a null terminator from the current position in the Span, or the rest of the
    ///     Span if no null
    ///     terminator is found.
    /// </summary>
    public string ReadString()
    {
        var index = -1;

        for (var i = Position; i < Buffer.Length; i++)
            if ((Buffer[i] == 10) || (Buffer[i] == 0))
            {
                index = i;

                break;
            }

        var length = index - Position;

        //if no terminators were found, just read the rest of the buffer as a string
        if (length < 0)
            length = Buffer.Length - Position;

        var ret = Encoding.GetString(Buffer.Slice(Position, length));
        Position += length;

        if (Position != Buffer.Length)
            Position++;

        return ret;
    }

    /// <summary>
    ///     Reads a string with a 16-bit length prefix from the current position in the Span.
    /// </summary>
    public string ReadString16()
    {
        int length = ReadUInt16();

        if (Remaining < length)
            throw new EndOfStreamException();

        var ret = Encoding.GetString(Buffer.Slice(Position, length));
        Position += length;

        return ret;
    }

    /// <summary>
    ///     Reads a string with an 8-bit length prefix from the current position in the Span.
    /// </summary>
    public string ReadString8()
    {
        int length = ReadByte();

        if (Remaining < length)
            throw new EndOfStreamException();

        var ret = Encoding.GetString(Buffer.Slice(Position, length));
        Position += length;

        return ret;
    }

    /// <summary>
    ///     Reads a 16-bit unsigned integer from the current position in the Span.
    /// </summary>
    public ushort ReadUInt16()
    {
        var ret = MemoryMarshal.Read<ushort>(Buffer[Position..]);

        if (!IsLittleEndian && BitConverter.IsLittleEndian)
            ret = BinaryPrimitives.ReverseEndianness(ret);

        Position += sizeof(ushort);

        return ret;
    }

    /// <summary>
    ///     Reads a 32-bit unsigned integer from the current position in the Span.
    /// </summary>
    public uint ReadUInt32()
    {
        var ret = MemoryMarshal.Read<uint>(Buffer[Position..]);

        if (!IsLittleEndian && BitConverter.IsLittleEndian)
            ret = BinaryPrimitives.ReverseEndianness(ret);

        Position += sizeof(uint);

        return ret;
    }
}