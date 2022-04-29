using Chaos.Core.Geometry;

namespace Chaos.Core.Data;

public record Animation
{
    public ushort AnimationSpeed { get; init; }
    public ushort SourceAnimation { get; init; }
    public int? SourceId { get; init; }
    public ushort TargetAnimation { get; init; }
    public int? TargetId { get; init; }
    public Point? TargetPoint { get; init; }

    /// <summary>
    ///     Static constructor for no animation.
    /// </summary>
    public static Animation None => new();

    /// <summary>
    ///     Returns a re-targeted animation based on IDs. (Does not remove sourceAnimation)
    /// </summary>
    public Animation GetTargetedAnimation(int targetId, int sourceId) =>
        this with { TargetPoint = null, TargetId = targetId, SourceId = sourceId };

    /// <summary>
    ///     Returns a re-targeted animation based on a point. (Does not remove sourceAnimation)
    /// </summary>
    public Animation GetTargetedAnimation(Point targetPoint) =>
        this with { TargetPoint = targetPoint, TargetId = null, SourceId = null };

    /// <summary>
    ///     Returns a re-targeted animation based on IDs. Removes sourceAnimation.
    /// </summary>
    public Animation GetTargetedEffectAnimation(int targetId, int sourceId) => this with
    {
        TargetPoint = null, TargetId = targetId, SourceId = sourceId, SourceAnimation = 0
    };

    /// <summary>
    ///     Returns a re-target animation based on a point. Removes sourceAnimation.
    /// </summary>
    public Animation GetTargetedEffectAnimation(Point targetPoint) =>
        this with { TargetPoint = targetPoint, TargetId = null, SourceId = null, SourceAnimation = 0 };

    public override string ToString()
    {
        if (TargetPoint == null)
            return
                $"SOURCE_ID: {SourceId} | SOURCE_ANIMATION: {SourceAnimation} | TARGET_ID: {TargetId} | TARGET_ANIMATION: {TargetAnimation}";

        return $"TARGET_POINT: {TargetPoint} | TARGET_ANIMATION: {TargetAnimation}";
    }
}