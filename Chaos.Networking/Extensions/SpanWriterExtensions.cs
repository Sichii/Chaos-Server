#region
using Chaos.Geometry.Abstractions;
using Chaos.IO.Memory;
#endregion

// ReSharper disable once CheckNamespace
namespace Chaos.Extensions.Networking;

internal static class SpanWriterExtensions
{
    extension(ref SpanWriter writer)
    {
        internal void WritePoint16(IPoint point) => writer.WritePoint16((ushort)point.X, (ushort)point.Y);
        internal void WritePoint8(IPoint point) => writer.WritePoint8((byte)point.X, (byte)point.Y);
    }
}