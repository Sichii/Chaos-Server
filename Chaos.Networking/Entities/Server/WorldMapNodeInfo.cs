using Chaos.Geometry.Abstractions;

namespace Chaos.Networking.Entities.Server;

public record WorldMapNodeInfo
{
    public ushort CheckSum { get; set; }
    public ushort DestinationMapId { get; set; }
    public IPoint DestinationPoint { get; set; } = null!;
    public IPoint Position { get; set; } = null!;
    public string Text { get; set; } = null!;
}