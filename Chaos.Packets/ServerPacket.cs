using System.Text;
using Chaos.Packets.Abstractions.Definitions;
using Chaos.Packets.Definitions;

namespace Chaos.Packets;

public ref struct ServerPacket
{
    public Span<byte> Buffer;
    public ServerOpCode OpCode { get; set; }
    public byte Sequence { get; set; }
    public bool ShouldEncrypt { get; set; }
    public byte Signature { get; }

    public ServerPacket()
    {
        Signature = 170;
        Sequence = 0;
        OpCode = ServerOpCode.ConnectionInfo;
        Buffer = new Span<byte>();
        ShouldEncrypt = false;
    }

    public ServerPacket(ServerOpCode opcode)
    {
        Signature = 170;
        Sequence = 0;
        OpCode = opcode;
        Buffer = new Span<byte>();
        ShouldEncrypt = false;
    }

    public readonly string GetAsciiString(bool replaceNewline = true)
    {
        var str = Encoding.ASCII.GetString(Buffer);

        if (replaceNewline)
            str = str.Replace((char)10, ' ')
                     .Replace((char)13, ' ');

        return str;
    }

    public string GetHexString() => $"{OpCode}: {RegexCache.DOUBLE_BYTE_REGEX.Replace(Convert.ToHexString(Buffer), "$1 ")}";

    public byte[] ToArray() => ToSpan().ToArray();

    public readonly Memory<byte> ToMemory()
    {
        //the length of the packet after the length portion of the header plus the packet tail (determined by encryption type)
        var resultLength = Buffer.Length + (ShouldEncrypt ? 5 : 4) - 3;
        var memBuffer = new byte[resultLength + 3];

        //write packet header
        memBuffer[0] = Signature;
        memBuffer[1] = (byte)(resultLength / 256);
        memBuffer[2] = (byte)(resultLength % 256);
        memBuffer[3] = (byte)OpCode;
        memBuffer[4] = Sequence;

        var memory = new Memory<byte>(memBuffer);

        Buffer.CopyTo(memory.Span[^Buffer.Length..]);

        return memory;
    }

    public Span<byte> ToSpan()
    {
        //the length of the packet after the length portion of the header plus the packet tail (determined by encryption type)
        var resultLength = Buffer.Length + (ShouldEncrypt ? 5 : 4) - 3;

        var resultBuffer = new Span<byte>(new byte[resultLength + 3])
        {
            //write packet header
            [0] = Signature,
            [1] = (byte)(resultLength / 256),
            [2] = (byte)(resultLength % 256),
            [3] = (byte)OpCode,
            [4] = Sequence
        };

        Buffer.CopyTo(resultBuffer[^Buffer.Length..]);

        return resultBuffer;
    }

    public override string ToString() => GetHexString();
}