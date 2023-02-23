namespace Chaos.Data;

public sealed record Warp
{
    public Location Destination { get; init; }
    public Point? Source { get; init; }

    public static Warp Unsourced(Location targetLocation) => new()
    {
        Destination = targetLocation,
        Source = null
    };
}