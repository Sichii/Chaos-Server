using System.Text;
using System.Text.RegularExpressions;
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