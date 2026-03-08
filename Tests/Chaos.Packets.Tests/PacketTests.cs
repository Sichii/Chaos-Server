#region
using System.Buffers;
using System.Text;
using FluentAssertions;
#endregion

namespace Chaos.Packets.Tests;

public sealed class PacketTests
{
    [Test]
    public void Ctor_FromMemoryOwner_Sets_Buffer_And_Properties()
    {
        var owner = MemoryPool<byte>.Shared.Rent(10);
        owner.Memory.Span[0] = 0xAA;
        owner.Memory.Span[1] = 0xBB;

        var pkt = new Packet(0x55, owner, 2);

        pkt.OpCode
           .Should()
           .Be(0x55);

        pkt.Signature
           .Should()
           .Be(170);

        pkt.Buffer
           .Length
           .Should()
           .Be(2);

        pkt.Buffer[0]
           .Should()
           .Be(0xAA);

        pkt.MemoryOwner
           .Should()
           .NotBeNull();

        pkt.Dispose();
    }

    [Test]
    public void Ctor_FromOpcode_Sets_Defaults()
    {
        var pkt = new Packet(0x33);

        pkt.OpCode
           .Should()
           .Be(0x33);

        pkt.Signature
           .Should()
           .Be(170);

        pkt.Sequence
           .Should()
           .Be(0);

        pkt.IsEncrypted
           .Should()
           .BeFalse();

        pkt.Buffer
           .Length
           .Should()
           .Be(0);
    }

    [Test]
    public void Ctor_FromSpan_Sets_Header_And_Buffer_Correctly_Encrypted()
    {
        var full = new byte[]
        {
            0xAA,
            0x00,
            0x06,
            0x20,
            0x7E,
            0x41,
            0x42
        }; // len differs but only tail matters
        var span = new Span<byte>(full);
        var pkt = new Packet(ref span, true);

        pkt.IsEncrypted
           .Should()
           .BeTrue();

        pkt.Buffer
           .ToArray()
           .Should()
           .Equal(0x41, 0x42);
    }

    [Test]
    public void Ctor_FromSpan_Sets_Header_And_Buffer_Correctly_Unencrypted()
    {
        var full = new byte[]
        {
            0xAA,
            0x00,
            0x05,
            0x10,
            0x7F,
            0x41,
            0x42
        }; // Sig, len(5), opcode, seq, 'A','B'
        var span = new Span<byte>(full);
        var pkt = new Packet(ref span, false);

        pkt.Signature
           .Should()
           .Be(0xAA);

        pkt.OpCode
           .Should()
           .Be(0x10);

        pkt.Sequence
           .Should()
           .Be(0x7F);

        pkt.IsEncrypted
           .Should()
           .BeFalse();

        pkt.Buffer
           .ToArray()
           .Should()
           .Equal(0x7F, 0x41, 0x42);
    }

    [Test]
    public void Dispose_WithMemoryOwner_DoesNotThrow()
    {
        var owner = MemoryPool<byte>.Shared.Rent(5);
        var pkt = new Packet(0x10, owner, 3);

        pkt.Dispose();
    }

    [Test]
    public void Dispose_WithoutMemoryOwner_DoesNotThrow()
    {
        var pkt = new Packet(0x10);

        pkt.Dispose();
    }

    [Test]
    public void GetAsciiString_Replaces_Newlines_When_Requested()
    {
        var data = Encoding.ASCII.GetBytes("A\nB\rC");

        var pkt = new Packet(0x10)
        {
            Buffer = data
        };

        pkt.GetAsciiString()
           .Should()
           .Be("A B C");

        pkt.GetAsciiString(false)
           .Should()
           .Be("A\nB\rC");
    }

    [Test]
    public void GetHexString_Formats_As_Expected()
    {
        var data = new byte[]
        {
            0xDE,
            0xAD,
            0xBE,
            0xEF
        };

        var pkt = new Packet(0x99)
        {
            Buffer = data
        };

        pkt.GetHexString()
           .Should()
           .Be("153: DE AD BE EF");
    }

    [Test]
    public void ToArray_Returns_Copy_Of_Span()
    {
        var pkt = new Packet(0x11)
        {
            Buffer = new byte[]
            {
                7,
                8
            }
        };

        pkt.ToArray()
           .Should()
           .Equal(
               pkt.ToSpan()
                  .ToArray());
    }

    [Test]
    public void ToMemory_WithMemoryOwner_ThrowsInvalidOperationException()
    {
        var owner = MemoryPool<byte>.Shared.Rent(5);
        var pkt = new Packet(0x10, owner, 3);

        InvalidOperationException? caught = null;

        try
        {
            pkt.ToMemory();
        } catch (InvalidOperationException ex)
        {
            caught = ex;
        }

        caught.Should()
              .NotBeNull();

        pkt.Dispose();
    }

    [Test]
    public void ToSpan_And_ToMemory_Include_Header_And_Buffer()
    {
        var payload = new byte[]
        {
            1,
            2,
            3
        };

        var pkt = new Packet(0x44)
        {
            Sequence = 0x02,
            Buffer = payload
        };

        var span = pkt.ToSpan();

        span[0]
            .Should()
            .Be(0xAA);

        span[3]
            .Should()
            .Be(0x44);

        span[^3..]
            .ToArray()
            .Should()
            .Equal(payload);

        var mem = pkt.ToMemory();

        mem.Span[0]
           .Should()
           .Be(0xAA);

        mem.Span[^3..]
           .ToArray()
           .Should()
           .Equal(payload);
    }

    [Test]
    public void ToSpan_WithMemoryOwner_ThrowsInvalidOperationException()
    {
        var owner = MemoryPool<byte>.Shared.Rent(5);
        var pkt = new Packet(0x10, owner, 3);

        InvalidOperationException? caught = null;

        try
        {
            pkt.ToSpan();
        } catch (InvalidOperationException ex)
        {
            caught = ex;
        }

        caught.Should()
              .NotBeNull();

        pkt.Dispose();
    }

    [Test]
    public void TransferOwnership_WithMemoryOwner_ReturnsOwnerAndLength()
    {
        var owner = MemoryPool<byte>.Shared.Rent(10);
        var pkt = new Packet(0x22, owner, 3);
        pkt.Buffer[0] = 1;
        pkt.Buffer[1] = 2;
        pkt.Buffer[2] = 3;

        (var resultOwner, var resultLength) = pkt.TransferOwnership();

        resultOwner.Should()
                   .NotBeNull();

        resultLength.Should()
                    .BeGreaterThan(0);

        var span = resultOwner.Memory.Span[..resultLength];

        span[0]
            .Should()
            .Be(170);

        span[3]
            .Should()
            .Be(0x22);

        resultOwner.Dispose();
    }

    [Test]
    public void TransferOwnership_WithoutMemoryOwner_ThrowsInvalidOperationException()
    {
        var pkt = new Packet(0x10)
        {
            Buffer = new byte[]
            {
                1,
                2
            }
        };

        InvalidOperationException? caught = null;

        try
        {
            pkt.TransferOwnership();
        } catch (InvalidOperationException ex)
        {
            caught = ex;
        }

        caught.Should()
              .NotBeNull();
    }
}