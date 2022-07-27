using Chaos.Geometry.Interfaces;
using Chaos.IO.Memory;

namespace Chaos.Networking.Extensions;

public static class SpanWriterExtensions
{
    public static void WritePoint8(this ref SpanWriter writer, IPoint point) => writer.WritePoint8((byte)point.X, (byte)point.Y);

    public static void WritePoint16(this ref SpanWriter writer, IPoint point) => writer.WritePoint16((ushort)point.X, (ushort)point.Y);
}