using Chaos.Geometry.Interfaces;

namespace Chaos.Networking.Model.Server;

public record WorldMapNodeInfo
{
    public ushort CheckSum { get; set; }
    public ushort DestinationMapId { get; set; }
    public IPoint DestinationPoint { get; set; } = null!;
    public IPoint Position { get; set; } = null!;
    public string Text { get; set; } = null!;
}