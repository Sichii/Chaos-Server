namespace Chaos.Networking.Model.Server;

public record DoorInfo
{
    public bool Closed { get; set; }
    public bool OpenRight { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
}