using System.Text;
using Chaos.IO.Definitions;
using Chaos.IO.Memory;
using FluentAssertions;
using Xunit;

namespace Chaos.IO.Tests;

public sealed class SpanWriterTests
{
    private static readonly Encoding Encoding = Encoding.UTF8;

    [Fact]
    public void WriteBoolean_ShouldWriteBooleanValueToBuffer()
    {
        // Arrange
        var buffer = new byte[1];
        var span = new Span<byte>(buffer);
        var writer = new SpanWriter(Encoding, ref span);

        // Act
        writer.WriteBoolean(true);

        // Assert
        buffer[0]
            .Should()
            .Be(1);
    }

    [Fact]
    public void WriteByte_ShouldWriteByteValueToBuffer()
    {
        // Arrange
        var buffer = new byte[1];
        var span = new Span<byte>(buffer);
        var writer = new SpanWriter(Encoding, ref span);

        // Act
        writer.WriteByte(0xAB);

        // Assert
        buffer[0]
            .Should()
            .Be(0xAB);
    }

    [Fact]
    public void WriteBytes_ShouldWriteByteArrayToBuffer()
    {
        // Arrange
        var buffer = new byte[4];
        var span = new Span<byte>(buffer);
        var writer = new SpanWriter(Encoding, ref span);

        // Act
        writer.WriteBytes(
            0x11,
            0x22,
            0x33,
            0x44);

        // Assert
        buffer.Should()
              .Equal(
                  0x11,
                  0x22,
                  0x33,
                  0x44);
    }

    [Fact]
    public void WriteBytes_ShouldWriteEmptyByteArrayToBuffer()
    {
        // Arrange
        var buffer = new byte[1];
        var span = new Span<byte>(buffer);
        var writer = new SpanWriter(Encoding, ref span);

        // Act
        writer.WriteBytes([]);

        // Assert
        buffer[0]
            .Should()
            .Be(0);
    }

    [Fact]
    public void WriteData_ShouldWriteByteArrayToBuffer()
    {
        // Arrange
        var buffer = new byte[3];
        var span = new Span<byte>(buffer);
        var writer = new SpanWriter(Encoding, ref span);

        // Act
        writer.WriteData("ABC"u8.ToArray());

        // Assert
        buffer.Should()
              .Equal("ABC"u8.ToArray());
    }

    [Fact]
    public void WriteData16_ShouldWriteByteArrayWithLengthPrefixToBuffer()
    {
        // Arrange
        var buffer = new byte[5];
        var span = new Span<byte>(buffer);
        var writer = new SpanWriter(Encoding, ref span);

        // Act
        writer.WriteData16("ABC"u8.ToArray());

        // Assert
        buffer.Should()
              .Equal(
                  0x00,
                  0x03,
                  0x41,
                  0x42,
                  0x43);
    }

    [Fact]
    public void WriteData16_ShouldWriteEmptyByteArrayWithLengthPrefixToBuffer()
    {
        // Arrange
        var buffer = new byte[2];
        var span = new Span<byte>(buffer);
        var writer = new SpanWriter(Encoding, ref span);

        // Act
        writer.WriteData16([]);

        // Assert
        buffer.Should()
              .Equal(0x00, 0x00);
    }

    [Fact]
    public void WriteData16_ShouldWriteTrimmedData_IfLengthAboveMax()
    {
        //Arrange
        var buffer = new byte[ushort.MaxValue + 2];
        var span = new Span<byte>(buffer);
        var writer = new SpanWriter(Encoding, ref span);

        // Act
        const int LENGTH = ushort.MaxValue + 1;
        writer.WriteData16(buffer[..LENGTH]);

        // Assert
        writer.Position
              .Should()
              .Be(ushort.MaxValue + 2); // 2 bytes for length, ushort.MaxValue for string
    }

    [Fact]
    public void WriteData8_ShouldWriteByteArrayWithLengthPrefixToBuffer()
    {
        // Arrange
        var buffer = new byte[4];
        var span = new Span<byte>(buffer);
        var writer = new SpanWriter(Encoding, ref span);

        // Act
        writer.WriteData8("ABC"u8.ToArray());

        // Assert
        buffer.Should()
              .Equal(
                  0x03,
                  0x41,
                  0x42,
                  0x43);
    }

    [Fact]
    public void WriteData8_ShouldWriteEmptyByteArrayWithLengthPrefixToBuffer()
    {
        // Arrange
        var buffer = new byte[1];
        var span = new Span<byte>(buffer);
        var writer = new SpanWriter(Encoding, ref span);

        // Act
        writer.WriteData8([]);

        // Assert
        buffer.Should()
              .Equal(0x00);
    }

    [Fact]
    public void WriteData8_ShouldWriteTrimmedData_IfLengthAboveMax()
    {
        //Arrange
        var buffer = new byte[byte.MaxValue + 2];
        var span = new Span<byte>(buffer);
        var writer = new SpanWriter(Encoding, ref span);

        // Act
        const int LENGTH = byte.MaxValue + 1;
        writer.WriteData8(buffer[..LENGTH]);

        // Assert
        writer.Position
              .Should()
              .Be(byte.MaxValue + 1); // 1 byte for length, ushort.MaxValue for string
    }

    [Fact]
    public void WriteDataWithLineFeed_ShouldEndInLineFeed()
    {
        // Arrange
        var buffer = new byte[4];
        var span = new Span<byte>(buffer);
        var writer = new SpanWriter(Encoding, ref span);

        // Act
        writer.WriteData(
            [
                0x01,
                0x02,
                0x03
            ],
            true);

        // Assert
        buffer.Should()
              .Equal(
                  0x01,
                  0x02,
                  0x03,
                  0x0A);
    }

    [Fact]
    public void WriteInt16_ShouldWriteInt16ValueToBuffer_WhenIsLittleEndianIsFalse()
    {
        // Arrange
        var buffer = new byte[2];
        var span = new Span<byte>(buffer);
        var writer = new SpanWriter(Encoding, ref span);

        // Act
        writer.WriteInt16(0x1234);

        // Assert
        buffer.Should()
              .Equal(0x12, 0x34);
    }

    [Fact]
    public void WriteInt16_ShouldWriteInt16ValueToBuffer_WhenIsLittleEndianIsTrue()
    {
        // Arrange
        var buffer = new byte[2];
        var span = new Span<byte>(buffer);
        var writer = new SpanWriter(Encoding, ref span, Endianness.LittleEndian);

        // Act
        writer.WriteInt16(0x1234);

        // Assert
        buffer.Should()
              .Equal(0x34, 0x12);
    }

    [Fact]
    public void WriteInt32_ShouldWriteInt32ValueToBuffer_WhenIsLittleEndianIsFalse()
    {
        // Arrange
        var buffer = new byte[4];
        var span = new Span<byte>(buffer);
        var writer = new SpanWriter(Encoding, ref span);

        // Act
        writer.WriteInt32(0x12345678);

        // Assert
        buffer.Should()
              .Equal(
                  0x12,
                  0x34,
                  0x56,
                  0x78);
    }

    [Fact]
    public void WriteInt32_ShouldWriteInt32ValueToBuffer_WhenIsLittleEndianIsTrue()
    {
        // Arrange
        var buffer = new byte[4];
        var span = new Span<byte>(buffer);
        var writer = new SpanWriter(Encoding, ref span, Endianness.LittleEndian);

        // Act
        writer.WriteInt32(0x12345678);

        // Assert
        buffer.Should()
              .Equal(
                  0x78,
                  0x56,
                  0x34,
                  0x12);
    }

    [Fact]
    public void WritePoint16_ShouldWriteTwoUInt16ValuesToBuffer()
    {
        // Arrange
        var buffer = new byte[4];
        var span = new Span<byte>(buffer);
        var writer = new SpanWriter(Encoding, ref span);

        // Act
        writer.WritePoint16(0x1234, 0x5678);

        // Assert
        buffer.Should()
              .Equal(
                  0x12,
                  0x34,
                  0x56,
                  0x78);
    }

    [Fact]
    public void WritePoint8_ShouldWriteTwoByteValuesToBuffer()
    {
        // Arrange
        var buffer = new byte[2];
        var span = new Span<byte>(buffer);
        var writer = new SpanWriter(Encoding, ref span);

        // Act
        writer.WritePoint8(0x12, 0x34);

        // Assert
        buffer.Should()
              .Equal(0x12, 0x34);
    }

    [Fact]
    public void WriteSByte_ShouldWriteSByteValueToBuffer()
    {
        // Arrange
        var buffer = new byte[1];
        var span = new Span<byte>(buffer);
        var writer = new SpanWriter(Encoding, ref span);

        // Act
        writer.WriteSByte(-123);

        // Assert
        buffer[0]
            .Should()
            .Be(0x85);
    }

    [Fact]
    public void WriteString_ShouldWriteStringToBuffer()
    {
        // Arrange
        var buffer = new byte[12];
        var span = new Span<byte>(buffer);
        var writer = new SpanWriter(Encoding, ref span);

        // Act
        writer.WriteString("Hello, world");

        // Assert
        buffer.Should()
              .Equal(Encoding.GetBytes("Hello, world"));
    }

    [Fact]
    public void WriteString16_ShouldWriteStringWithLengthPrefixToBuffer()
    {
        // Arrange
        var buffer = new byte[15];
        var span = new Span<byte>(buffer);
        var writer = new SpanWriter(Encoding, ref span);

        // Act
        writer.WriteString16("Hello, world!");

        // Assert
        buffer.Should()
              .Equal(
                  new byte[]
                  {
                      0x00,
                      0x0D
                  }.Concat(Encoding.GetBytes("Hello, world!")));
    }

    [Fact]
    public void WriteString16_ShouldWriteTrimmedString_IfLengthAboveMax()
    {
        //Arrange
        var buffer = new byte[ushort.MaxValue + 2];
        var span = new Span<byte>(buffer);
        var writer = new SpanWriter(Encoding, ref span);

        // Act
        const int LENGTH = ushort.MaxValue + 1;
        writer.WriteString16(Encoding.GetString(buffer[..LENGTH]));

        // Assert
        writer.Position
              .Should()
              .Be(ushort.MaxValue + 2); // 2 bytes for length, ushort.MaxValue for string
    }

    [Fact]
    public void WriteString8_ShouldWriteStringWithLengthPrefixToBuffer()
    {
        // Arrange
        var buffer = new byte[14];
        var span = new Span<byte>(buffer);
        var writer = new SpanWriter(Encoding, ref span);

        // Act
        writer.WriteString8("Hello, world!");

        // Assert
        buffer.Should()
              .Equal(
                  new byte[]
                  {
                      0x0D
                  }.Concat(Encoding.GetBytes("Hello, world!")));
    }

    [Fact]
    public void WriteString8_ShouldWriteTrimmedString_IfLengthAboveMax()
    {
        //Arrange
        var buffer = new byte[byte.MaxValue + 2];
        var span = new Span<byte>(buffer);
        var writer = new SpanWriter(Encoding, ref span);

        // Act
        const int LENGTH = byte.MaxValue + 1;
        writer.WriteString8(Encoding.GetString(buffer[..LENGTH]));

        // Assert
        writer.Position
              .Should()
              .Be(byte.MaxValue + 1); // 1 byte for length, ushort.MaxValue for string
    }

    [Fact]
    public void WriteUInt16_ShouldWriteUInt16ValueToBuffer_WhenIsLittleEndianIsFalse()
    {
        // Arrange
        var buffer = new byte[2];
        var span = new Span<byte>(buffer);
        var writer = new SpanWriter(Encoding, ref span);

        // Act
        writer.WriteUInt16(0x1234);

        // Assert
        buffer.Should()
              .Equal(0x12, 0x34);
    }

    [Fact]
    public void WriteUInt16_ShouldWriteUInt16ValueToBuffer_WhenIsLittleEndianIsTrue()
    {
        // Arrange
        var buffer = new byte[2];
        var span = new Span<byte>(buffer);
        var writer = new SpanWriter(Encoding, ref span, Endianness.LittleEndian);

        // Act
        writer.WriteUInt16(0x1234);

        // Assert
        buffer.Should()
              .Equal(0x34, 0x12);
    }

    [Fact]
    public void WriteUInt32_ShouldWriteUInt32ValueToBuffer_WhenIsLittleEndianIsFalse()
    {
        // Arrange
        var buffer = new byte[4];
        var span = new Span<byte>(buffer);
        var writer = new SpanWriter(Encoding, ref span);

        // Act
        writer.WriteUInt32(0x12345678);

        // Assert
        buffer.Should()
              .Equal(
                  0x12,
                  0x34,
                  0x56,
                  0x78);
    }

    [Fact]
    public void WriteUInt32_ShouldWriteUInt32ValueToBuffer_WhenIsLittleEndianIsTrue()
    {
        // Arrange
        var buffer = new byte[4];
        var span = new Span<byte>(buffer);
        var writer = new SpanWriter(Encoding, ref span, Endianness.LittleEndian);

        // Act
        writer.WriteUInt32(0x12345678);

        // Assert
        buffer.Should()
              .Equal(
                  0x78,
                  0x56,
                  0x34,
                  0x12);
    }

    [Fact]
    public void WriteValue_ShouldWriteToBuffer()
    {
        // Arrange
        var writer = new SpanWriter(Encoding);
        const byte BYTE_VALUE = 10;
        const sbyte SBYTE_VALUE = -10;
        const ushort USHORT_VALUE = 100;
        const short SHORT_VALUE = -100;
        const uint UINT_VALUE = 1000;
        const int INT_VALUE = -1000;
        const bool BOOL_VALUE = true;
        const byte X8 = 10;
        const byte Y8 = 20;
        const ushort X16 = 100;
        const ushort Y16 = 200;

        // Act
        writer.WriteValue(BYTE_VALUE);
        writer.WriteValue(SBYTE_VALUE);
        writer.WriteValue(USHORT_VALUE);
        writer.WriteValue(SHORT_VALUE);
        writer.WriteValue(UINT_VALUE);
        writer.WriteValue(INT_VALUE);
        writer.WriteValue(BOOL_VALUE);
        writer.WriteValue((x8: X8, y8: Y8));
        writer.WriteValue((x16: X16, y16: Y16));

        // Assert
        var result = writer.ToSpan()
                           .ToArray();

        var expectedResult = new[]
            {
                BYTE_VALUE,
                unchecked((byte)SBYTE_VALUE)
            }.Concat(
                 BitConverter.GetBytes(USHORT_VALUE)
                             .Reverse())
             .Concat(
                 BitConverter.GetBytes(SHORT_VALUE)
                             .Reverse())
             .Concat(
                 BitConverter.GetBytes(UINT_VALUE)
                             .Reverse())
             .Concat(
                 BitConverter.GetBytes(INT_VALUE)
                             .Reverse())
             .Concat(
                 BitConverter.GetBytes(BOOL_VALUE)
                             .Reverse())
             .Concat(
                 new[]
                 {
                     X8,
                     Y8
                 })
             .Concat(
                 BitConverter.GetBytes(X16)
                             .Reverse())
             .Concat(
                 BitConverter.GetBytes(Y16)
                             .Reverse());

        result.Should()
              .ContainInOrder(expectedResult);
    }
}