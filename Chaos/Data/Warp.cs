namespace Chaos.Data;

public sealed record Warp
{
    public Location Destination { get; init; }
    public Point? Source { get; init; }

    public override string ToString() => $@"{Source.ToString()} => {Destination.ToString()}";

    public static Warp Unsourced(Location targetLocation) => new()
    {
        Destination = targetLocation,
        Source = null
    };
}