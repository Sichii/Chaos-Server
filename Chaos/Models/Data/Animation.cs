using Chaos.Geometry.Abstractions;

namespace Chaos.Models.Data;

public sealed record Animation
{
    public ushort AnimationSpeed { get; set; }
    public ushort SourceAnimation { get; set; }
    public uint? SourceId { get; set; }
    public ushort TargetAnimation { get; set; }

    public uint? TargetId { get; set; }
    public IPoint? TargetPoint { get; set; }

    /// <summary>
    ///     Static constructor for no animation.
    /// </summary>
    public static Animation None => new();

    /// <summary>
    ///     Returns a re-targeted animation based on a point. (Does not remove sourceAnimation)
    /// </summary>
    public Animation GetPointAnimation(IPoint targetPoint, uint? sourceId = null) =>
        this with { TargetPoint = targetPoint, TargetId = null, SourceId = sourceId ?? SourceId };

    /// <summary>
    ///     Returns a re-target animation based on a point. Removes sourceAnimation.
    /// </summary>
    public Animation GetPointEffectAnimation(IPoint targetPoint, uint? sourceId = null) =>
        this with { TargetPoint = targetPoint, TargetId = null, SourceId = sourceId ?? SourceId, SourceAnimation = 0 };

    /// <summary>
    ///     Returns a re-targeted animation based on IDs. (Does not remove sourceAnimation)
    /// </summary>
    public Animation GetTargetedAnimation(uint targetId, uint? sourceId = null) =>
        this with { TargetPoint = null, TargetId = targetId, SourceId = sourceId ?? SourceId };

    /// <summary>
    ///     Returns a re-targeted animation based on IDs. Removes sourceAnimation.
    /// </summary>
    public Animation GetTargetedEffectAnimation(uint targetId, uint? sourceId = null) => this with
    {
        TargetPoint = null, TargetId = targetId, SourceId = sourceId ?? SourceId, SourceAnimation = 0
    };
}