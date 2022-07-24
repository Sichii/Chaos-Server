using Chaos.Geometry.Interfaces;

namespace Chaos.Networking.Model.Server;

public record WorldMapNodeInfo
{
    public IPoint Position { get; set; } = null!;
    public string Text { get; set; } = null!;
    public ushort DestinationMapId { get; set; }
    public IPoint DestinationPoint { get; set; } = null!;
    public ushort CheckSum { get; set; }
}