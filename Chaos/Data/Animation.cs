using Chaos.Geometry.Abstractions;

namespace Chaos.Data;

public sealed record Animation
{
    public ushort AnimationSpeed { get; init; }
    public ushort SourceAnimation { get; init; }
    public uint? SourceId { get; init; }
    public ushort TargetAnimation { get; init; }
    public uint? TargetId { get; init; }
    public IPoint? TargetPoint { get; init; }

    /// <summary>
    ///     Static constructor for no animation.
    /// </summary>
    public static Animation None => new();

    /// <summary>
    ///     Returns a re-targeted animation based on a point. (Does not remove sourceAnimation)
    /// </summary>
    public Animation GetPointAnimation(Point targetPoint, uint? sourceId = null) =>
        this with { TargetPoint = targetPoint, TargetId = null, SourceId = sourceId ?? SourceId };

    /// <summary>
    ///     Returns a re-target animation based on a point. Removes sourceAnimation.
    /// </summary>
    public Animation GetPointEffectAnimation(Point targetPoint, uint? sourceId = null) =>
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

    public override string ToString()
    {
        if (TargetPoint == null)
            return
                $"SOURCE_ID: {SourceId} | SOURCE_ANIMATION: {SourceAnimation} | TARGET_ID: {TargetId} | TARGET_ANIMATION: {TargetAnimation}";

        return $"TARGET_POINT: {TargetPoint} | TARGET_ANIMATION: {TargetAnimation}";
    }
}