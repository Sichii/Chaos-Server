using System.Text;
using Chaos.Core.Geometry;

namespace Chaos.Core.Utilities;

public ref struct SpanReader
{
    private readonly Span<byte> Buffer;
    public Encoding Encoding { get; set; }
    public int Position { get; set; }
    public bool EndOfSpan => Position >= Buffer.Length;
    public int Remaining => Buffer.Length - Position;

    public SpanReader(Encoding encoding, ref Span<byte> buffer)
    {
        Buffer = buffer;
        Encoding = encoding;
        Position = 0;
    }

    public List<string> ReadArgs()
    {
        var args = new List<string>();

        while (!EndOfSpan)
            args.Add(ReadString());

        return args;
    }

    public List<string> ReadArgs8()
    {
        var args = new List<string>();

        while (!EndOfSpan)
            args.Add(ReadString8());

        return args;
    }

    public bool ReadBoolean() => ReadByte() != 0;

    public byte ReadByte()
    {
        if (Remaining < 1)
            throw new EndOfStreamException();

        return Buffer[Position++];
    }

    public byte[] ReadBytes(int length)
    {
        if (Remaining < length)
            throw new EndOfStreamException();

        Position += length;

        return Buffer[..length].ToArray();
    }

    public byte[] ReadData() => ReadBytes(Remaining);

    public byte[] ReadData16()
    {
        int lengthPrefix = ReadUInt16();

        return ReadBytes(lengthPrefix);
    }

    public byte[] ReadData8()
    {
        int lengthPrefix = ReadByte();

        return ReadBytes(lengthPrefix);
    }

    public short ReadInt16() => (short)((ReadByte() << 8) | ReadByte());

    public int ReadInt32() =>
        (ReadByte() << 24)
        | (ReadByte() << 16)
        | (ReadByte() << 8)
        | ReadByte();

    public Point ReadPoint16()
    {
        var x = ReadUInt16();
        var y = ReadUInt16();

        return new Point
        {
            X = x,
            Y = y
        };
    }

    public Point ReadPoint8()
    {
        var x = ReadByte();
        var y = ReadByte();

        return new Point
        {
            X = x,
            Y = y
        };
    }

    public sbyte ReadSByte() => (sbyte)ReadByte();

    public string ReadString()
    {
        var terminatorPosition = 0;

        for (var i = Position; i < Buffer.Length; i++)
        {
            terminatorPosition = i;

            if (Buffer[i] == 10)
                break;
        }

        var length = terminatorPosition - Position;
        var ret = Encoding.GetString(Buffer.Slice(Position, length));
        Position += length;

        return ret;
    }

    public string ReadString16()
    {
        int length = ReadUInt16();

        if (Remaining < length)
            throw new EndOfStreamException();

        var ret = Encoding.GetString(Buffer.Slice(Position, length));
        Position += length;

        return ret;
    }

    public string ReadString8()
    {
        int length = ReadByte();

        if (Remaining < length)
            throw new EndOfStreamException();

        var ret = Encoding.GetString(Buffer.Slice(Position, length));
        Position += length;

        return ret;
    }

    public ushort ReadUInt16() => (ushort)((ReadByte() << 8) | ReadByte());

    public uint ReadUInt32() =>
        (uint)((ReadByte() << 24)
               | (ReadByte() << 16)
               | (ReadByte() << 8)
               | ReadByte());
}