using System.Buffers.Binary;
using System.Runtime.InteropServices;
using System.Text;
using Chaos.IO.Definitions;

namespace Chaos.IO.Memory;

public ref struct SpanReader
{
    private readonly Span<byte> Buffer;
    public int Position { get; set; }
    public Encoding Encoding { get; }
    public Endianness Endianness { get; }
    public readonly bool EndOfSpan => Position >= Buffer.Length;
    public readonly int Remaining => Buffer.Length - Position;

    public SpanReader(Encoding encoding, in Span<byte> buffer, Endianness endianness = Endianness.BigEndian)
    {
        Buffer = buffer;
        Encoding = encoding;
        Position = 0;
        Endianness = endianness;
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

    public bool ReadBoolean()
    {
        var ret = MemoryMarshal.Read<bool>(Buffer[Position..]);
        Position++;

        return ret;
    }

    public byte ReadByte()
    {
        var ret = MemoryMarshal.Read<byte>(Buffer[Position..]);
        Position++;

        return ret;
    }

    public byte[] ReadBytes(int length)
    {
        if (Remaining < length)
            throw new EndOfStreamException();

        var start = Position;
        var end = Position += length;

        return Buffer[start..end].ToArray();
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

    public short ReadInt16()
    {
        var ret = MemoryMarshal.Read<short>(Buffer[Position..]);

        if (Endianness == Endianness.BigEndian)
            ret = BinaryPrimitives.ReverseEndianness(ret);

        Position += sizeof(short);

        return ret;
    }

    public int ReadInt32()
    {
        var ret = MemoryMarshal.Read<int>(Buffer[Position..]);

        if (Endianness == Endianness.BigEndian)
            ret = BinaryPrimitives.ReverseEndianness(ret);

        Position += sizeof(int);

        return ret;
    }

    public (ushort X, ushort Y) ReadPoint16() => (ReadUInt16(), ReadUInt16());

    public (byte X, byte Y) ReadPoint8() => (ReadByte(), ReadByte());

    public sbyte ReadSByte()
    {
        var ret = MemoryMarshal.Read<sbyte>(Buffer[Position..]);
        Position++;

        return ret;
    }

    public string ReadString()
    {
        var length = -1;

        for (var i = Position; i < Buffer.Length; i++)
            if (Buffer[i] == 10)
            {
                length = i;

                break;
            }

        //if no terminators were found, just read the rest of the buffer as a string
        if (length == -1)
            length = Buffer.Length - Position;

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

    public ushort ReadUInt16()
    {
        var ret = MemoryMarshal.Read<ushort>(Buffer[Position..]);

        if (Endianness == Endianness.BigEndian)
            ret = BinaryPrimitives.ReverseEndianness(ret);

        Position += sizeof(ushort);

        return ret;
    }

    public uint ReadUInt32()
    {
        var ret = MemoryMarshal.Read<uint>(Buffer[Position..]);

        if (Endianness == Endianness.BigEndian)
            ret = BinaryPrimitives.ReverseEndianness(ret);

        Position += sizeof(uint);

        return ret;
    }
}