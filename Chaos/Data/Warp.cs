using Chaos.Entities.Schemas.Data;

namespace Chaos.Data;

public record Warp
{
    public Location? SourceLocation { get; init; }
    public Location TargetLocation { get; init; }

    public Warp(WarpSchema schema, string sourceInstanceId)
    {
        SourceLocation = new Location(sourceInstanceId, schema.Source.X, schema.Source.Y);
        TargetLocation = schema.Destination;
    }

    private Warp() { }

    public override string ToString() => $@"{SourceLocation.ToString()} => {TargetLocation.ToString()}";

    public static Warp Unsourced(Location targetLocation) => new()
    {
        TargetLocation = targetLocation,
        SourceLocation = null
    };
}