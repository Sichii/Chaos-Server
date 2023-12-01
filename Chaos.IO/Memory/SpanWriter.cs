using System.Buffers.Binary;
using System.Runtime.InteropServices;
using System.Text;
using Chaos.IO.Definitions;

namespace Chaos.IO.Memory;

/// <summary>
///     A ref struct for writing primitive types, strings, and other data to a <see cref="Span{T}" /> with support for
///     endianness and growing.
/// </summary>
public ref struct SpanWriter
{
    private readonly bool AutoGrow;
    private readonly bool IsLittleEndian;
    private Span<byte> Buffer;

    /// <summary>
    ///     Gets or sets the current position within the Span.
    /// </summary>
    public int Position { get; set; }

    /// <summary>
    ///     Gets the Encoding used for writing strings.
    /// </summary>
    public Encoding Encoding { get; }

    /// <summary>
    ///     Gets the Endianness used for writing numbers.
    /// </summary>
    public Endianness Endianness { get; }

    /// <summary>
    ///     Gets a value indicating whether the current position is at the end of the Span.
    /// </summary>
    public readonly bool EndOfSpan => Position >= Buffer.Length;

    /// <summary>
    ///     Gets the number of bytes remaining in the Span from the current position.
    /// </summary>
    public readonly int Remaining => Buffer.Length - Position;

    /// <summary>
    ///     Initializes a new instance of the <see cref="SpanWriter" /> struct with the specified encoding and buffer.
    /// </summary>
    /// <param name="encoding">The encoding used for writing strings.</param>
    /// <param name="buffer">A reference to the buffer.</param>
    /// <param name="endianness">The endianness used for writing numbers.</param>
    public SpanWriter(Encoding encoding, ref Span<byte> buffer, Endianness endianness = Endianness.BigEndian)
    {
        Buffer = buffer;
        Encoding = encoding;
        Position = 0;
        AutoGrow = false;
        Endianness = endianness;
        IsLittleEndian = Endianness == Endianness.LittleEndian;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="SpanWriter" /> struct with the specified encoding, initial buffer
    ///     size, and endianness.
    /// </summary>
    /// <param name="encoding">The encoding used for writing strings.</param>
    /// <param name="initialBufferSize">The initial size of the buffer.</param>
    /// <param name="autoGrow">A flag indicating whether the buffer should automatically grow when needed.</param>
    /// <param name="endianness">The endianness used for writing numbers.</param>
    public SpanWriter(
        Encoding encoding,
        int initialBufferSize = 50,
        bool autoGrow = true,
        Endianness endianness = Endianness.BigEndian)
    {
        Buffer = new Span<byte>(new byte[initialBufferSize]);
        Encoding = encoding;
        Position = 0;
        AutoGrow = autoGrow;
        Endianness = endianness;
        IsLittleEndian = Endianness == Endianness.LittleEndian;
    }

    /// <summary>
    ///     Trims the buffer to the current position.
    /// </summary>
    public void Flush() => Buffer = Buffer[..Position];

    private void GrowIfNeeded(int bytesToWrite)
    {
        if (Remaining >= bytesToWrite)
            return;

        if (!AutoGrow)
            throw new EndOfStreamException();

        var buffer = Buffer;

        //create a new buffer of length * 3 OR (length + bytesToWrite) * 1.5 (whichever is bigger)
        var newLength = (int)Math.Max(buffer.Length * 3, (buffer.Length + bytesToWrite) * 1.5);
        var newBuffer = new byte[newLength];
        Buffer = new Span<byte>(newBuffer);
        buffer.CopyTo(Buffer);
    }

    private void InnerWriteString(ReadOnlySpan<byte> buffer, bool lineFeed = false, bool terminate = false)
    {
        GrowIfNeeded(buffer.Length);

        buffer.CopyTo(Buffer[Position..]);
        Position += buffer.Length;

        if (lineFeed)
            WriteByte(10); // LF
        else if (terminate)
            WriteByte(0); // \0
    }

    /// <summary>
    ///     Returns a trimmed span of the underlying buffer.
    /// </summary>
    /// <returns>The trimmed span of the underlying buffer.</returns>
    public Span<byte> ToSpan()
    {
        if (Buffer.Length != Position)
            Flush();

        return Buffer;
    }

    /// <summary>
    ///     Writes a value of a numeric type to the buffer.
    /// </summary>
    /// <typeparam name="T">The numeric type of the value to write.</typeparam>
    /// <param name="value">The value to write.</param>
    public void WriteValue<T>(T value) where T: unmanaged
    {
        switch (value)
        {
            case byte @byte:
                WriteByte(@byte);

                break;
            case sbyte @sbyte:
                WriteSByte(@sbyte);

                break;
            case ushort @ushort:
                WriteUInt16(@ushort);

                break;
            case short @short:
                WriteInt16(@short);

                break;
            case uint @uint:
                WriteUInt32(@uint);

                break;
            case int @int:
                WriteInt32(@int);

                break;
            case bool @bool:
                WriteBoolean(@bool);

                break;
            case ValueTuple<byte, byte> vTuple8:
                WritePoint8(vTuple8.Item1, vTuple8.Item2);

                break;
            case ValueTuple<ushort, ushort> vTuple16:
                WritePoint16(vTuple16.Item1, vTuple16.Item2);

                break;
        }
    }

    /// <summary>
    ///     Writes a boolean value to the buffer.
    /// </summary>
    /// <param name="value">The boolean value to write.</param>
    public void WriteBoolean(bool value)
    {
        GrowIfNeeded(sizeof(bool));

        MemoryMarshal.Write(Buffer[Position..], in value);

        Position++;
    }

    /// <summary>
    ///     Writes a byte value to the buffer.
    /// </summary>
    /// <param name="value">The byte value to write.</param>
    public void WriteByte(byte value)
    {
        GrowIfNeeded(1);
        MemoryMarshal.Write(Buffer[Position..], in value);
        Position++;
    }

    /// <summary>
    ///     Writes an array of bytes to the buffer.
    /// </summary>
    /// <param name="buffer">The byte array to write.</param>
    public void WriteBytes(params byte[] buffer)
    {
        GrowIfNeeded(buffer.Length);

        buffer.CopyTo(Buffer[Position..]);
        Position += buffer.Length;
    }

    /// <summary>
    ///     Writes a byte array to the buffer with an optional line feed.
    /// </summary>
    /// <param name="buffer">The byte array to write.</param>
    /// <param name="lineFeed">A flag indicating whether to append a line feed after the byte array.</param>
    public void WriteData(byte[] buffer, bool lineFeed = false)
    {
        WriteBytes(buffer);

        if (lineFeed)
            WriteByte(10);
    }

    /// <summary>
    ///     Writes a byte array with a prepended 16-bit length to the buffer.
    /// </summary>
    /// <param name="buffer">The byte array to write.</param>
    public void WriteData16(byte[] buffer)
    {
        if (buffer.Length > ushort.MaxValue)
            buffer = buffer[..ushort.MaxValue];

        WriteUInt16((ushort)buffer.Length);
        WriteData(buffer);
    }

    /// <summary>
    ///     Writes a byte array with a prepended 8-bit length to the buffer.
    /// </summary>
    /// <param name="buffer">The byte array to write.</param>
    public void WriteData8(byte[] buffer)
    {
        if (buffer.Length > byte.MaxValue)
            buffer = buffer[..byte.MaxValue];

        WriteByte((byte)buffer.Length);
        WriteData(buffer);
    }

    /// <summary>
    ///     Writes a 16-bit signed integer value to the buffer.
    /// </summary>
    /// <param name="value">The 16-bit signed integer value to write.</param>
    public void WriteInt16(short value)
    {
        GrowIfNeeded(sizeof(short));

        if (!IsLittleEndian && BitConverter.IsLittleEndian)
            value = BinaryPrimitives.ReverseEndianness(value);

        MemoryMarshal.Write(Buffer[Position..], in value);
        Position += sizeof(short);
    }

    /// <summary>
    ///     Writes a 32-bit signed integer value to the buffer.
    /// </summary>
    /// <param name="value">The 32-bit signed integer value to write.</param>
    public void WriteInt32(int value)
    {
        GrowIfNeeded(sizeof(int));

        if (!IsLittleEndian && BitConverter.IsLittleEndian)
            value = BinaryPrimitives.ReverseEndianness(value);

        MemoryMarshal.Write(Buffer[Position..], in value);
        Position += sizeof(int);
    }

    /// <summary>
    ///     Writes two 16-bit unsigned integers as a point to the buffer.
    /// </summary>
    /// <param name="x">The x-coordinate of the point.</param>
    /// <param name="y">The y-coordinate of the point.</param>
    public void WritePoint16(ushort x, ushort y)
    {
        WriteUInt16(x);
        WriteUInt16(y);

        //TODO: 00 0B 00 0B 00
    }

    /// <summary>
    ///     Writes two 8-bit unsigned integers as a point to the buffer.
    /// </summary>
    /// <param name="x">The x-coordinate of the point.</param>
    /// <param name="y">The y-coordinate of the point.</param>
    public void WritePoint8(byte x, byte y)
    {
        WriteByte(x);
        WriteByte(y);
    }

    /// <summary>
    ///     Writes an 8-bit signed byte value to the buffer.
    /// </summary>
    /// <param name="value">The 8-bit signed integer value to write.</param>
    public void WriteSByte(sbyte value)
    {
        GrowIfNeeded(sizeof(sbyte));

        MemoryMarshal.Write(Buffer[Position..], in value);

        Position++;
    }

    /// <summary>
    ///     Writes a string to the buffer with optional line feed or null termination.
    /// </summary>
    /// <param name="value">The string to write.</param>
    /// <param name="lineFeed">A flag indicating whether to append a line feed after the string.</param>
    /// <param name="terminate">A flag indicating whether to append a null terminator after the string.</param>
    public void WriteString(string value, bool lineFeed = false, bool terminate = false)
    {
        var buffer = Encoding.GetBytes(value);
        InnerWriteString(buffer, lineFeed, terminate);
    }

    /// <summary>
    ///     Writes a string with a prepended 16-bit length to the buffer.
    /// </summary>
    /// <param name="value">The string to write.</param>
    public void WriteString16(string value)
    {
        var buffer = Encoding.GetBytes(value)
                             .AsSpan();

        if (buffer.Length > ushort.MaxValue)
            buffer = buffer[..ushort.MaxValue];

        WriteUInt16((ushort)buffer.Length);
        InnerWriteString(buffer);
    }

    /// <summary>
    ///     Writes a string with a prepended 8-bit length to the buffer.
    /// </summary>
    /// <param name="value">The string to write.</param>
    public void WriteString8(string value)
    {
        var buffer = Encoding.GetBytes(value)
                             .AsSpan();

        if (buffer.Length > byte.MaxValue)
            buffer = buffer[..byte.MaxValue];

        WriteByte((byte)buffer.Length);
        InnerWriteString(buffer);
    }

    /// <summary>
    ///     Writes a 16-bit unsigned integer value to the buffer.
    /// </summary>
    /// <param name="value">The 16-bit unsigned integer value to write.</param>
    public void WriteUInt16(ushort value)
    {
        GrowIfNeeded(sizeof(ushort));

        if (!IsLittleEndian && BitConverter.IsLittleEndian)
            value = BinaryPrimitives.ReverseEndianness(value);

        MemoryMarshal.Write(Buffer[Position..], in value);
        Position += sizeof(ushort);
    }

    /// <summary>
    ///     Writes a 32-bit unsigned integer value to the buffer.
    /// </summary>
    /// <param name="value">The 32-bit unsigned integer value to write.</param>
    public void WriteUInt32(uint value)
    {
        GrowIfNeeded(sizeof(uint));

        if (!IsLittleEndian && BitConverter.IsLittleEndian)
            value = BinaryPrimitives.ReverseEndianness(value);

        MemoryMarshal.Write(Buffer[Position..], in value);
        Position += sizeof(uint);
    }
}