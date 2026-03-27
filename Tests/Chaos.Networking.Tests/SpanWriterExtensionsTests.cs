#region
using System.Text;
using Chaos.Extensions.Networking;
using Chaos.Geometry;
using Chaos.IO.Memory;
using FluentAssertions;
#endregion

namespace Chaos.Networking.Tests;

public sealed class SpanWriterExtensionsTests
{
    [Test]
    public void WritePoint16_ShouldWriteUshortXAndY()
    {
        var writer = new SpanWriter(Encoding.ASCII);
        var point = new Point(300, 400);

        writer.WritePoint16(point);
        var span = writer.ToSpan();

        // ushort X=300 big-endian: 0x01 0x2C, ushort Y=400: 0x01 0x90
        span.Length
            .Should()
            .Be(4);

        var x = (ushort)((span[0] << 8) | span[1]);
        var y = (ushort)((span[2] << 8) | span[3]);

        x.Should()
         .Be(300);

        y.Should()
         .Be(400);
    }

    [Test]
    public void WritePoint8_ShouldWriteByteXAndY()
    {
        var writer = new SpanWriter(Encoding.ASCII);
        var point = new Point(10, 20);

        writer.WritePoint8(point);
        var span = writer.ToSpan();

        span.Length
            .Should()
            .Be(2);

        span[0]
            .Should()
            .Be(10);

        span[1]
            .Should()
            .Be(20);
    }
}