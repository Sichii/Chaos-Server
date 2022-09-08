using Chaos.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.IO.Memory;

namespace Chaos.Networking.Extensions;

public static class SpanReaderExtensions
{
    public static IPoint ReadPoint16(this SpanReader reader) => (Point)reader.ReadPoint16();
    public static IPoint ReadPoint8(this SpanReader reader) => (Point)reader.ReadPoint8();
}