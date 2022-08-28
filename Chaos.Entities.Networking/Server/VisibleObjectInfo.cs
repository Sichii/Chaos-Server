namespace Chaos.Entities.Networking.Server;

public record VisibleObjectInfo : WorldObjectInfo
{
    public ushort Sprite { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
}