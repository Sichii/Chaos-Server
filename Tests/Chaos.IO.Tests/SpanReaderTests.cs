#region
using System.Text;
using Chaos.IO.Definitions;
using Chaos.IO.Memory;
using FluentAssertions;
#endregion

namespace Chaos.IO.Tests;

public sealed class SpanReaderTests
{
    private static readonly Encoding Encoding = Encoding.UTF8;

    [Test]
    public void ReadArgs_ShouldReturnListOfStrings()
    {
        // Arrange
        var buffer = Encoding.GetBytes("hello\0world\0");
        var reader = new SpanReader(Encoding, buffer);

        // Act
        var result = reader.ReadArgs();

        // Assert
        result.Should()
              .ContainInOrder("hello", "world");
    }

    [Test]
    public void ReadArgs8_ShouldReturnListOfStrings()
    {
        // Arrange
        var buffer = Encoding.GetBytes("\x05hello\x00\x05world\x00");
        var reader = new SpanReader(Encoding, buffer);

        // Act
        var result = reader.ReadArgs8();

        // Assert
        result.Should()
              .ContainInOrder("hello", "world");
    }

    [Test]
    public void ReadBoolean_ShouldReturnBooleanValue()
    {
        // Arrange
        var buffer = new byte[]
        {
            0x01
        };
        var reader = new SpanReader(Encoding, buffer);

        // Act
        var result = reader.ReadBoolean();

        // Assert
        result.Should()
              .BeTrue();
    }

    [Test]
    public void ReadByte_ShouldReturnByteValue()
    {
        // Arrange
        var buffer = new byte[]
        {
            0xAB
        };
        var reader = new SpanReader(Encoding, buffer);

        // Act
        var result = reader.ReadByte();

        // Assert
        result.Should()
              .Be(0xAB);
    }

    [Test]
    public void ReadBytes_ShouldReturnByteArrayWithSpecifiedLength()
    {
        // Arrange
        var buffer = Encoding.GetBytes("Hello, world!");
        var reader = new SpanReader(Encoding, buffer);

        // Act
        var result = reader.ReadBytes(5);

        // Assert
        result.Should()
              .Equal(Encoding.GetBytes("Hello"));
    }

    [Test]
    public void ReadBytes_ShouldThrowEndOfStreamException_WhenRemainingBytesAreLessThanRequestedLength()
    {
        // Arrange
        var buffer = Encoding.GetBytes("Hello");
        var reader = new SpanReader(Encoding, buffer);
        EndOfStreamException? ex = null;

        try
        {
            reader.ReadBytes(10);
        } catch (EndOfStreamException e)
        {
            ex = e;
        }

        // Assert
        ex.Should()
          .NotBeNull();

        ex?.Should()
          .BeOfType<EndOfStreamException>();
    }

    [Test]
    public void ReadData_ShouldReturnAllRemainingBytes()
    {
        // Arrange
        var buffer = Encoding.GetBytes("Hello, world!");
        var reader = new SpanReader(Encoding, buffer);

        // Act
        var result = reader.ReadData();

        // Assert
        result.Should()
              .Equal(buffer);
    }

    [Test]
    public void ReadData16_ShouldReturnByteArrayWithLengthSpecifiedByUInt16()
    {
        // Arrange
        var buffer = Encoding.GetBytes("\x00\x0DHello, world!");
        var reader = new SpanReader(Encoding, buffer);

        // Act
        var result = reader.ReadData16();

        // Assert
        result.Should()
              .Equal(Encoding.GetBytes("Hello, world!"));
    }

    [Test]
    public void ReadData8_ShouldReturnByteArrayWithLengthSpecifiedByByte()
    {
        // Arrange
        var buffer = Encoding.GetBytes("\x0DHello, world!");
        var reader = new SpanReader(Encoding, buffer);

        // Act
        var result = reader.ReadData8();

        // Assert
        result.Should()
              .Equal(Encoding.GetBytes("Hello, world!"));
    }

    [Test]
    public void ReadInt16_ShouldReturnInt16Value()
    {
        // Arrange
        var buffer = new byte[]
        {
            0x12,
            0x34
        };
        var reader = new SpanReader(Encoding, buffer);

        // Act
        var result = reader.ReadInt16();

        // Assert
        result.Should()
              .Be(0x1234);
    }

    [Test]
    public void ReadInt16_ShouldReturnInt16Value_LittleEndian()
    {
        // Arrange
        var buffer = new byte[]
        {
            0x34,
            0x12
        };
        var reader = new SpanReader(Encoding, buffer, Endianness.LittleEndian);

        // Act
        var result = reader.ReadInt16();

        // Assert
        result.Should()
              .Be(0x1234);
    }

    [Test]
    public void ReadInt32_ShouldReturnInt32Value()
    {
        // Arrange
        var buffer = new byte[]
        {
            0x12,
            0x34,
            0x56,
            0x78
        };
        var reader = new SpanReader(Encoding, buffer);

        // Act
        var result = reader.ReadInt32();

        // Assert
        result.Should()
              .Be(0x12345678);
    }

    [Test]
    public void ReadInt32_ShouldReturnInt32Value_LittleEndian()
    {
        // Arrange
        var buffer = new byte[]
        {
            0x78,
            0x56,
            0x34,
            0x12
        };
        var reader = new SpanReader(Encoding, buffer, Endianness.LittleEndian);

        // Act
        var result = reader.ReadInt32();

        // Assert
        result.Should()
              .Be(0x12345678);
    }

    [Test]
    public void ReadPoint16_ShouldReturnTupleOfUInt16()
    {
        // Arrange
        var buffer = new byte[]
        {
            0x12,
            0x34,
            0x56,
            0x78
        };
        var reader = new SpanReader(Encoding, buffer);

        // Act
        var result = reader.ReadPoint16();

        // Assert
        result.Should()
              .Be((0x1234, 0x5678));
    }

    [Test]
    public void ReadPoint16_ShouldReturnTupleOfUInt16_LittleEndian()
    {
        // Arrange
        var buffer = new byte[]
        {
            0x34,
            0x12,
            0x78,
            0x56
        };
        var reader = new SpanReader(Encoding, buffer, Endianness.LittleEndian);

        // Act
        var result = reader.ReadPoint16();

        // Assert
        result.Should()
              .Be((0x1234, 0x5678));
    }

    [Test]
    public void ReadPoint8_ShouldReturnTupleOfByte()
    {
        // Arrange
        var buffer = new byte[]
        {
            0x12,
            0x34
        };
        var reader = new SpanReader(Encoding, buffer);

        // Act
        var result = reader.ReadPoint8();

        // Assert
        result.Should()
              .Be((0x12, 0x34));
    }

    [Test]
    public void ReadSByte_ShouldReturnSByteValue()
    {
        // Arrange
        var buffer = new byte[]
        {
            0xAB
        };
        var reader = new SpanReader(Encoding, buffer);

        // Act
        var result = reader.ReadSByte();

        // Assert
        result.Should()
              .Be(-0x55);
    }

    [Test]
    public void ReadString_ShouldReturnStringUntilEndOfBuffer_WhenNoNullTerminatorIsFound()
    {
        // Arrange
        var buffer = Encoding.GetBytes("Hello, world!");
        var reader = new SpanReader(Encoding, buffer);

        // Act
        var result = reader.ReadString();

        // Assert
        result.Should()
              .Be("Hello, world!");
    }

    [Test]
    public void ReadString_ShouldReturnStringUntilNullTerminator()
    {
        // Arrange
        var buffer = Encoding.GetBytes("Hello\0World");
        var reader = new SpanReader(Encoding, buffer);

        // Act
        var result = reader.ReadString();

        // Assert
        result.Should()
              .Be("Hello");
    }

    [Test]
    public void ReadString16_ShouldReturnStringWithLengthSpecifiedByUInt16()
    {
        // Arrange
        var buffer = Encoding.GetBytes("\x00\x0DHello, world!");
        var reader = new SpanReader(Encoding, buffer);

        // Act
        var result = reader.ReadString16();

        // Assert
        result.Should()
              .Be("Hello, world!");
    }

    [Test]
    public void ReadString16_ShouldThrowEndOfStreamException_WhenRemainingBytesAreLessThanSpecifiedLength()
    {
        // Arrange
        var buffer = Encoding.GetBytes("\x00\xFFHello, world");
        var reader = new SpanReader(Encoding, buffer);

        // Act
        EndOfStreamException? exception = null;

        try
        {
            reader.ReadString16();
        } catch (EndOfStreamException ex)
        {
            exception = ex;
        }

        // Assert
        exception.Should()
                 .NotBeNull();

        exception.Should()
                 .BeOfType<EndOfStreamException>();
    }

    [Test]
    public void ReadString8_ShouldReturnStringWithLengthSpecifiedByByte()
    {
        // Arrange
        var buffer = Encoding.GetBytes("\x0DHello, world!");
        var reader = new SpanReader(Encoding, buffer);

        // Act
        var result = reader.ReadString8();

        // Assert
        result.Should()
              .Be("Hello, world!");
    }

    [Test]
    public void ReadString8_ShouldThrowEndOfStreamException_WhenRemainingBytesAreLessThanSpecifiedLength()
    {
        // Arrange
        var buffer = Encoding.GetBytes("\xFFHello, world");
        var reader = new SpanReader(Encoding, buffer);

        // Act
        EndOfStreamException? exception = null;

        try
        {
            reader.ReadString8();
        } catch (EndOfStreamException ex)
        {
            exception = ex;
        }

        // Assert
        exception.Should()
                 .NotBeNull();

        exception.Should()
                 .BeOfType<EndOfStreamException>();
    }

    [Test]
    public void ReadUInt16_ShouldReturnUInt16Value()
    {
        // Arrange
        var buffer = new byte[]
        {
            0x12,
            0x34
        };
        var reader = new SpanReader(Encoding, buffer);

        // Act
        var result = reader.ReadUInt16();

        // Assert
        result.Should()
              .Be(0x1234);
    }

    [Test]
    public void ReadUInt16_ShouldReturnUInt16Value_LittleEndian()
    {
        // Arrange
        var buffer = new byte[]
        {
            0x34,
            0x12
        };
        var reader = new SpanReader(Encoding, buffer, Endianness.LittleEndian);

        // Act
        var result = reader.ReadUInt16();

        // Assert
        result.Should()
              .Be(0x1234);
    }

    [Test]
    public void ReadUInt32_ShouldReturnUInt32Value()
    {
        // Arrange
        var buffer = new byte[]
        {
            0x12,
            0x34,
            0x56,
            0x78
        };
        var reader = new SpanReader(Encoding, buffer);

        // Act
        var result = reader.ReadUInt32();

        // Assert
        result.Should()
              .Be(0x12345678);
    }

    [Test]
    public void ReadUInt32_ShouldReturnUInt32Value_LittleEndian()
    {
        // Arrange
        var buffer = new byte[]
        {
            0x78,
            0x56,
            0x34,
            0x12
        };
        var reader = new SpanReader(Encoding, buffer, Endianness.LittleEndian);

        // Act
        var result = reader.ReadUInt32();

        // Assert
        result.Should()
              .Be(0x12345678);
    }
}