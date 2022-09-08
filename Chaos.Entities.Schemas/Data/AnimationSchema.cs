namespace Chaos.Entities.Schemas.Data;

public record AnimationSchema
{
    public required ushort AnimationSpeed { get; init; } = 1000;
    public required ushort SourceAnimation { get; init; }
    public required ushort TargetAnimation { get; init; }
}