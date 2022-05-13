using Chaos.Core.Geometry;

namespace Chaos.Core.Data;

public record WorldMapNode(
    Point Position,
    string Text,
    ushort DestinationMapId,
    Point DestinationPoint,
    ushort CheckSum
);