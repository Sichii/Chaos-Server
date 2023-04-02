using System.Text;
using Chaos.Packets.Abstractions.Definitions;
using Chaos.Packets.Definitions;

namespace Chaos.Packets;

/// <summary>
///     Represents a server packet with various properties and methods for manipulation.
/// </summary>
public ref struct ServerPacket
{
    /// <summary>
    ///     The byte buffer containing the packet data.
    /// </summary>
    public Span<byte> Buffer;
    /// <summary>
    ///     The sequence number of the packet.
    /// </summary>
    public byte Sequence { get; set; }
    /// <summary>
    ///     Determines whether the packet should be encrypted.
    /// </summary>
    public bool ShouldEncrypt { get; set; }
    /// <summary>
    ///     The operation code of the server packet.
    /// </summary>
    public ServerOpCode OpCode { get; }
    /// <summary>
    ///     The signature byte of the packet.
    /// </summary>
    public byte Signature { get; }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ServerPacket" /> struct with default values.
    /// </summary>
    public ServerPacket()
    {
        Signature = 170;
        Sequence = 0;
        OpCode = ServerOpCode.ConnectionInfo;
        Buffer = new Span<byte>();
        ShouldEncrypt = false;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ServerPacket" /> struct with the specified operation code.
    /// </summary>
    /// <param name="opcode">The operation code of the server packet.</param>
    public ServerPacket(ServerOpCode opcode)
    {
        Signature = 170;
        Sequence = 0;
        OpCode = opcode;
        Buffer = new Span<byte>();
        ShouldEncrypt = false;
    }

    /// <summary>
    ///     Gets the ASCII string representation of the packet buffer.
    /// </summary>
    /// <param name="replaceNewline">Indicates whether newline characters should be replaced with spaces.</param>
    /// <returns>The ASCII string representation of the packet buffer.</returns>
    public readonly string GetAsciiString(bool replaceNewline = true)
    {
        var str = Encoding.ASCII.GetString(Buffer);

        if (replaceNewline)
            str = str.Replace((char)10, ' ')
                     .Replace((char)13, ' ');

        return str;
    }

    /// <summary>
    ///     Gets the hexadecimal string representation of the packet buffer.
    /// </summary>
    /// <returns>The hexadecimal string representation of the packet buffer.</returns>
    public string GetHexString() => $"{OpCode}: {RegexCache.DOUBLE_BYTE_REGEX.Replace(Convert.ToHexString(Buffer), "$1 ")}";

    /// <summary>
    ///     Converts the packet data to an array of bytes.
    /// </summary>
    /// <returns>An array of bytes representing the packet data.</returns>
    public byte[] ToArray() => ToSpan().ToArray();

    /// <summary>
    ///     Converts the packet data to a memory block of bytes.
    /// </summary>
    /// <returns>A memory block of bytes representing the packet data.</returns>
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

    /// <summary>
    ///     Converts the packet data to a span of bytes.
    /// </summary>
    /// <returns>A span of bytes representing the packet data.</returns>
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

    /// <summary>
    ///     Returns the hexadecimal string representation of the packet.
    /// </summary>
    /// <returns>The hexadecimal string representation of the packet.</returns>
    public override string ToString() => GetHexString();
}