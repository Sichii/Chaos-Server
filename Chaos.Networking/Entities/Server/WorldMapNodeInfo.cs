using Chaos.Geometry;
using Chaos.Geometry.Abstractions;

namespace Chaos.Networking.Entities.Server;

public record WorldMapNodeInfo
{
    public ushort UniqueId { get; set; }
    public Location Destination { get; set; }
    public IPoint ScreenPosition { get; set; } = null!;
    public string Text { get; set; } = null!;
}