using Chaos.Geometry.Abstractions;
using Chaos.IO.Memory;

// ReSharper disable once CheckNamespace
namespace Chaos.Extensions.Networking;

internal static class SpanWriterExtensions
{
    internal static void WritePoint16(this ref SpanWriter writer, IPoint point) => writer.WritePoint16((ushort)point.X, (ushort)point.Y);
    internal static void WritePoint8(this ref SpanWriter writer, IPoint point) => writer.WritePoint8((byte)point.X, (byte)point.Y);
}