namespace Chaos.Entities.Schemas.Data;

public record AnimationSchema
{
    /// <summary>
    ///     Defaults to 100<br />How fast the animation plays, lower is faster
    /// </summary>
    public required ushort AnimationSpeed { get; init; } = 100;
    /// <summary>
    ///     The id of the animation to play on the source of this action
    /// </summary>
    public required ushort SourceAnimation { get; init; }
    /// <summary>
    ///     The id of the animation to play on the target of this action
    /// </summary>
    public required ushort TargetAnimation { get; init; }
}