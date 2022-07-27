namespace Chaos.Data;

public record Warp
{
    public Location? SourceLocation { get; init; }
    public Location TargetLocation { get; init; }

    public override string ToString() => $@"{SourceLocation.ToString()} => {TargetLocation.ToString()}";

    public static Warp Unsourced(Location targetLocation) => new()
    {
        TargetLocation = targetLocation,
        SourceLocation = null
    };
}