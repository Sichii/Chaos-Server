using System.Text;
using Chaos.Packets.Abstractions.Definitions;
using Chaos.Packets.Definitions;

namespace Chaos.Packets;

public ref struct ClientPacket
{
    public Span<byte> Buffer;
    public bool IsEncrypted { get; }
    public ClientOpCode OpCode { get; }
    public byte Sequence { get; }
    public byte Signature { get; }

    public ClientPacket(ref Span<byte> span, bool isEncrypted)
    {
        Signature = span[0];
        OpCode = (ClientOpCode)span[3];
        Sequence = span[4];
        IsEncrypted = isEncrypted;

        var resultLength = span.Length - (IsEncrypted ? 5 : 4);
        Buffer = span[^resultLength..];
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
        var resultLength = Buffer.Length + (IsEncrypted ? 5 : 4) - 3;
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
        var resultLength = Buffer.Length + (IsEncrypted ? 5 : 4) - 3;

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