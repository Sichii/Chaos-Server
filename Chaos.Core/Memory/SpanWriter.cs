using System.Text;

namespace Chaos.Core.Memory;

public ref struct SpanWriter
{
    private readonly bool AutoGrow;
    private Span<byte> Buffer;
    public Encoding Encoding { get; set; }
    public int Position { get; set; }
    public bool EndOfSpan => Position >= Buffer.Length;
    public int Remaining => Buffer.Length - Position;

    public SpanWriter(Encoding encoding, ref Span<byte> buffer)
    {
        Buffer = buffer;
        Encoding = encoding;
        Position = 0;
        AutoGrow = false;
    }

    public SpanWriter(Encoding encoding, int initialBufferSize = 50, bool autoGrow = true)
    {
        Buffer = new Span<byte>(new byte[initialBufferSize]);
        Encoding = encoding;
        Position = 0;
        AutoGrow = autoGrow;
    }

    public Span<byte> Flush() => Buffer[..Position];

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

    public void WriteBoolean(bool value) => WriteByte((byte)(value ? 1 : 0));

    public void WriteByte(byte value)
    {
        GrowIfNeeded(1);
        Buffer[Position++] = value;
    }

    public void WriteBytes(params byte[] buffer)
    {
        GrowIfNeeded(buffer.Length);

        for (var i = 0; i < buffer.Length; i++)
            Buffer[Position++] = buffer[i];
    }

    public void WriteData(byte[] buffer, bool terminate = false)
    {
        WriteBytes(buffer);

        if (terminate)
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

    public void WriteInt16(short value) => WriteBytes((byte)(value >> 8), (byte)value);

    public void WriteInt32(int value) => WriteBytes(
        (byte)(value >> 24),
        (byte)(value >> 16),
        (byte)(value >> 8),
        (byte)value);

    public void WriteObjects(params object[] objects)
    {
        for (var i = 0; i < objects.Length; i++)
            switch (objects[i])
            {
                case byte @byte:
                    WriteByte(@byte);

                    break;
                case bool @bool:
                    WriteBoolean(@bool);

                    break;
                case char @char:
                    WriteByte((byte)@char);

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
                case string @string:
                    WriteString8(@string);

                    break;
                case Point point:
                    WritePoint16(point);

                    break;
            }
    }

    public void WritePoint16(Point point)
    {
        WriteUInt16(point.X);
        WriteUInt16(point.Y);
    }

    public void WritePoint8(Point point)
    {
        WriteByte((byte)point.X);
        WriteByte((byte)point.Y);
    }

    public void WriteSByte(sbyte value) => WriteByte((byte)value);

    public void WriteString(string value, bool terminate = false)
    {
        var length = value.Length;
        GrowIfNeeded(length);

        var strBuffer = value.AsSpan();
        Encoding.GetBytes(strBuffer, Buffer[Position..]);
        Position += length;

        if (terminate)
            WriteByte(10);
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

    public void WriteUInt16(ushort value) => WriteBytes((byte)(value >> 8), (byte)value);

    public void WriteUInt32(uint value) =>
        WriteBytes(
            (byte)(value >> 24),
            (byte)(value >> 16),
            (byte)(value >> 8),
            (byte)value);
}