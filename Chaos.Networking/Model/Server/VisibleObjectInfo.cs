namespace Chaos.Networking.Model.Server;

public record VisibleObjectInfo : WorldObjectInfo
{
    public int X { get; set; }
    public int Y { get; set; }
    public ushort Sprite { get; set; }
}