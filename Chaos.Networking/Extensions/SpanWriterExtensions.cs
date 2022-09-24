using Chaos.Geometry.Abstractions;
using Chaos.IO.Memory;

namespace Chaos.Networking.Extensions;

internal static class SpanWriterExtensions
{
    internal static void WritePoint16(this ref SpanWriter writer, IPoint point) => writer.WritePoint16((ushort)point.X, (ushort)point.Y);
    internal static void WritePoint8(this ref SpanWriter writer, IPoint point) => writer.WritePoint8((byte)point.X, (byte)point.Y);
}