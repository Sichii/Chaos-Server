using System.Text;
using System.Text.RegularExpressions;
using Chaos.Packets.Definitions;

namespace Chaos.Packets;

public ref struct ClientPacket
{
    public Span<byte> Buffer;
    public bool IsEncrypted { get; set; }
    public ClientOpCode OpCode { get; set; }
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

    public string GetAsciiString(bool replaceNewline = true)
    {
        var str = Encoding.ASCII.GetString(Buffer);

        if (replaceNewline)
            str = str.Replace((char)10, ' ')
                     .Replace((char)13, ' ');

        return str;
    }

    public string GetHexString() => $"{OpCode}: {Regex.Replace(Convert.ToHexString(Buffer), "(.{2})", "$1 ", RegexOptions.Compiled)}";

    public byte[] ToArray() => ToSpan().ToArray();

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