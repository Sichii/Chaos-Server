#region
using System.Buffers;
using System.Text;
#endregion

namespace Chaos.Packets;

/// <summary>
///     Represents a packet of data
/// </summary>
public ref struct Packet
{
    /// <summary>
    ///     The buffer containing the packet data
    /// </summary>
    public Span<byte> Buffer;

    /// <summary>
    ///     The memory owner for pooled memory, if pooling is enabled
    /// </summary>
    public IMemoryOwner<byte>? MemoryOwner;

    /// <summary>
    ///     The length of the packet buffer
    /// </summary>
    public int Length;

    /// <summary>
    ///     Whether or not the packet is encrypted
    /// </summary>
    public bool IsEncrypted { get; set; }

    /// <summary>
    ///     A value used to identify the type of packet and it's purpose
    /// </summary>
    public byte OpCode { get; }

    /// <summary>
    ///     A value used to ensure packets are processed in the correct order
    /// </summary>
    public byte Sequence { get; set; }

    /// <summary>
    ///     A value used to identify the start of a packet's payload
    /// </summary>
    public byte Signature { get; }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Packet" /> struct with the specified buffer and encryption status.
    /// </summary>
    /// <param name="span">
    ///     The buffer containing the packet data.
    /// </param>
    /// <param name="isEncrypted">
    ///     <c>
    ///         true
    ///     </c>
    ///     if the packet is encrypted; otherwise,
    ///     <c>
    ///         false
    ///     </c>
    /// </param>
    public Packet(ref Span<byte> span, bool isEncrypted)
    {
        Signature = span[0];
        OpCode = span[3];
        Sequence = span[4];
        IsEncrypted = isEncrypted;
        MemoryOwner = null;

        var resultLength = span.Length - (IsEncrypted ? 5 : 4);
        Buffer = span[^resultLength..];
        Length = Buffer.Length;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Packet" /> struct with the specified operation code.
    /// </summary>
    /// <param name="opcode">
    ///     The operation code of the packet.
    /// </param>
    public Packet(byte opcode)
    {
        OpCode = opcode;
        Signature = 170;
        Sequence = 0;
        Buffer = new Span<byte>();
        IsEncrypted = false;
        MemoryOwner = null;
        Length = 0;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="Packet" /> struct with the specified operation code and memory owner.
    /// </summary>
    /// <param name="opcode">
    ///     The operation code of the packet.
    /// </param>
    /// <param name="memoryOwner">
    ///     The memory owner for pooled memory.
    /// </param>
    /// <param name="length">
    ///     The length of the packet buffer within the rented memory.
    /// </param>
    public Packet(byte opcode, IMemoryOwner<byte> memoryOwner, int length)
    {
        OpCode = opcode;
        Signature = 170;
        Sequence = 0;
        Buffer = memoryOwner.Memory.Span[..length];
        Length = length;
        IsEncrypted = false;
        MemoryOwner = memoryOwner;
    }

    /// <summary>
    ///     Returns the packet data as an ASCII string.
    /// </summary>
    /// <param name="replaceNewline">
    ///     Whether to replace newline characters with spaces. Default is true.
    /// </param>
    /// <returns>
    ///     The packet data as an ASCII string.
    /// </returns>
    public readonly string GetAsciiString(bool replaceNewline = true)
    {
        var str = Encoding.ASCII.GetString(Buffer);

        if (replaceNewline)
            str = str.Replace((char)10, ' ')
                     .Replace((char)13, ' ');

        return str;
    }

    /// <summary>
    ///     Returns the packet data as a hexadecimal string.
    /// </summary>
    /// <returns>
    ///     The packet data as a hexadecimal string.
    /// </returns>
    public string GetHexString() => $"{OpCode}: {BitConverter.ToString(Buffer.ToArray()).Replace("-", " ")}";

    /// <summary>
    ///     Converts the packet data to a byte array.
    /// </summary>
    /// <returns>
    ///     The packet data as a byte array.
    /// </returns>
    public byte[] ToArray()
        => ToSpan()
            .ToArray();

    /// <summary>
    ///     Transfers ownership of the packet data to a new memory owner.
    /// </summary>
    public readonly (IMemoryOwner<byte> Owner, int Length) TransferOwnership()
    {
        if (MemoryOwner is null)
            throw new InvalidOperationException("Cannot transfer ownership of a non-rented packet.");

        //the length of the packet after the length portion of the header plus the packet tail (determined by encryption type)
        var dataLength = Buffer.Length + (IsEncrypted ? 5 : 4) - 3;
        var lengthActual = dataLength + 3;
        var owner = MemoryOwner!;
        var resized = owner.Memory.Length < lengthActual;

        if (resized)
            owner = MemoryPool<byte>.Shared.Rent(lengthActual);

        var memory = owner.Memory;
        var buffer = memory.Span[..lengthActual];

        Buffer.CopyTo(buffer[^Buffer.Length..]);

        //write packet header
        buffer[0] = Signature;
        buffer[1] = (byte)(dataLength / 256);
        buffer[2] = (byte)(dataLength % 256);
        buffer[3] = OpCode;

        if (IsEncrypted)
            buffer[4] = Sequence;

        if (resized)
            MemoryOwner!.Dispose();

        return (owner, lengthActual);
    }

    /// <summary>
    ///     Converts the packet data to a <see cref="Memory{T}" /> instance.
    /// </summary>
    /// <returns>
    ///     The packet data as a <see cref="Memory{T}" /> instance.
    /// </returns>
    public readonly Memory<byte> ToMemory()
    {
        if (MemoryOwner is not null)
            throw new InvalidOperationException("Cannot convert a rented packet to a memory.");

        //the length of the packet after the length portion of the header plus the packet tail (determined by encryption type)
        var resultLength = Buffer.Length + (IsEncrypted ? 5 : 4) - 3;
        var memBuffer = new byte[resultLength + 3];

        //write packet header
        memBuffer[0] = Signature;
        memBuffer[1] = (byte)(resultLength / 256);
        memBuffer[2] = (byte)(resultLength % 256);
        memBuffer[3] = OpCode;
        memBuffer[4] = Sequence;

        var memory = new Memory<byte>(memBuffer);
        Buffer.CopyTo(memory.Span[^Buffer.Length..]);

        return memory;
    }

    /// <summary>
    ///     Converts the packet data to a <see cref="Span{T}" /> instance.
    /// </summary>
    /// <returns>
    ///     The packet data as a <see cref="Span{T}" /> instance.
    /// </returns>
    public Span<byte> ToSpan()
    {
        if (MemoryOwner is not null)
            throw new InvalidOperationException("Cannot convert a rented packet to a span.");

        //the length of the packet after the length portion of the header plus the packet tail (determined by encryption type)
        var resultLength = Buffer.Length + (IsEncrypted ? 5 : 4) - 3;

        var resultBuffer = new Span<byte>(new byte[resultLength + 3])
        {
            //write packet header
            [0] = Signature,
            [1] = (byte)(resultLength / 256),
            [2] = (byte)(resultLength % 256),
            [3] = OpCode,
            [4] = Sequence
        };

        Buffer.CopyTo(resultBuffer[^Buffer.Length..]);

        return resultBuffer;
    }

    /// <summary>
    ///     Returns a string representation of the packet data as a hexadecimal string.
    /// </summary>
    /// <returns>
    ///     A string representation of the packet data as a hexadecimal string.
    /// </returns>
    public override string ToString() => GetHexString();

    /// <summary>
    ///     Disposes the memory owner and returns the memory to the pool.
    /// </summary>
    public readonly void Dispose() => MemoryOwner?.Dispose();
}