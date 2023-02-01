using System.Buffers.Binary;
using System.Runtime.InteropServices;
using System.Text;
using FluentAssertions;
using Xunit;

namespace Chaos.Networking.Tests;

public class SerializerTests
{
    public SerializerTests()
    {
        var provider = CodePagesEncodingProvider.Instance;
        Encoding.RegisterProvider(provider);
    }

    [Fact]
    public void TestEndianness()
    {
        var buffer = new byte[4];
        var span = buffer.AsSpan();
        var num = 0x5f;

        num = BinaryPrimitives.ReverseEndianness(num);

        MemoryMarshal.Write(span, ref num);

        buffer.Should()
              .Equal(
                  0,
                  0,
                  0,
                  0x5f);
    }
}