using System.Buffers.Binary;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using Chaos.IO.Definitions;

namespace Chaos.IO.Memory;

public ref struct SpanWriter
{
    private readonly bool AutoGrow;
    private Span<byte> Buffer;
    public int Position { get; set; }
    public Encoding Encoding { get; }
    public Endianness Endianness { get; }
    public readonly bool EndOfSpan => Position >= Buffer.Length;
    public readonly int Remaining => Buffer.Length - Position;

    public SpanWriter(Encoding encoding, ref Span<byte> buffer, Endianness endianness = Endianness.BigEndian)
    {
        Buffer = buffer;
        Encoding = encoding;
        Position = 0;
        AutoGrow = false;
        Endianness = endianness;
    }

    public SpanWriter(
        Encoding encoding,
        int initialBufferSize = 50,
        bool autoGrow = true,
        Endianness endianness = Endianness.BigEndian
    )
    {
        Buffer = new Span<byte>(new byte[initialBufferSize]);
        Encoding = encoding;
        Position = 0;
        AutoGrow = autoGrow;
        Endianness = endianness;
    }

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

    public Span<byte> ToSpan()
    {
        if (Buffer.Length != Position)
            Flush();

        return Buffer;
    }

    public void Write<T>(T num) where T: struct, INumber<T>
    {
        switch (num)
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
        }
    }

    public void Write(bool @bool) => WriteBoolean(@bool);

    public void WriteBoolean(bool value)
    {
        GrowIfNeeded(sizeof(bool));

        MemoryMarshal.Write(Buffer[Position..], ref value);

        Position++;
    }

    public void WriteByte(byte value)
    {
        GrowIfNeeded(1);
        MemoryMarshal.Write(Buffer[Position..], ref value);
        Position++;
    }

    public void WriteBytes(params byte[] buffer)
    {
        GrowIfNeeded(buffer.Length);

        buffer.CopyTo(Buffer[Position..]);
        Position += buffer.Length;
    }

    public void WriteData(byte[] buffer, bool lineFeed = false)
    {
        WriteBytes(buffer);

        if (lineFeed)
            WriteByte(10);
    }

    public void WriteData16(byte[] value)
    {
        WriteUInt16((ushort)value.Length);
        WriteData(value);
    }

    public void WriteData8(byte[] value)
    {
        WriteByte((byte)value.Length);
        WriteData(value);
    }

    public void WriteInt16(short value)
    {
        GrowIfNeeded(sizeof(short));

        if (Endianness == Endianness.BigEndian)
            value = BinaryPrimitives.ReverseEndianness(value);

        MemoryMarshal.Write(Buffer[Position..], ref value);
        Position += sizeof(short);
    }

    public void WriteInt32(int value)
    {
        GrowIfNeeded(sizeof(int));

        if (Endianness == Endianness.BigEndian)
            value = BinaryPrimitives.ReverseEndianness(value);

        MemoryMarshal.Write(Buffer[Position..], ref value);
        Position += sizeof(int);
    }

    public void WritePoint16(ushort x, ushort y)
    {
        WriteUInt16(x);
        WriteUInt16(y);
        //TODO: 00 0B 00 0B 00
    }

    public void WritePoint8(byte x, byte y)
    {
        WriteByte(x);
        WriteByte(y);
    }

    public void WriteSByte(sbyte value)
    {
        GrowIfNeeded(sizeof(sbyte));

        MemoryMarshal.Write(Buffer[Position..], ref value);

        Position++;
    }

    public void WriteString(string value, bool lineFeed = false, bool terminate = false)
    {
        var length = value.Length;
        GrowIfNeeded(length);

        var strBuffer = value.AsSpan();
        Encoding.GetBytes(strBuffer, Buffer[Position..]);
        Position += length;

        if (lineFeed)
            WriteByte(10); // LF
        else if (terminate)
            WriteByte(0); // \0
    }

    public void WriteString16(string value)
    {
        WriteUInt16((ushort)value.Length);
        WriteString(value);
    }

    public void WriteString8(string value)
    {
        WriteByte((byte)value.Length);
        WriteString(value);
    }

    public void WriteUInt16(ushort value)
    {
        GrowIfNeeded(sizeof(ushort));

        if (Endianness == Endianness.BigEndian)
            value = BinaryPrimitives.ReverseEndianness(value);

        MemoryMarshal.Write(Buffer[Position..], ref value);
        Position += sizeof(ushort);
    }

    public void WriteUInt32(uint value)
    {
        GrowIfNeeded(sizeof(uint));

        if (Endianness == Endianness.BigEndian)
            value = BinaryPrimitives.ReverseEndianness(value);

        MemoryMarshal.Write(Buffer[Position..], ref value);
        Position += sizeof(uint);
    }
}