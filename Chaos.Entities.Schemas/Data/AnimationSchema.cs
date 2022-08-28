namespace Chaos.Entities.Schemas.Data;

public record AnimationSchema
{
    public ushort AnimationSpeed { get; init; } = 1000;
    public ushort SourceAnimation { get; init; }
    public ushort TargetAnimation { get; init; }
}