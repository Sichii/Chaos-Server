namespace Chaos.Networking.Model.Server;

public record VisibleArg : WorldObject
{
    public Point Point { get; set; }
    public ushort Sprite { get; set; }
}