using Chaos.Geometry.Abstractions;

namespace Chaos.Models.Data;

public sealed record Animation
{
    public ushort AnimationSpeed { get; set; }
    public int DurationMs { get; set; } = 250; //default quarter second
    public int? Priority { get; set; }
    public ushort SourceAnimation { get; set; }
    public uint? SourceId { get; set; }
    public DateTime Started { get; set; } = DateTime.UtcNow;
    public ushort TargetAnimation { get; set; }

    public uint? TargetId { get; set; }
    public Point? TargetPoint { get; set; }

    /// <summary>
    ///     Static constructor for no animation.
    /// </summary>
    public static Animation None => new();

    private decimal GetInterpolatedPriority(Animation animation)
    {
        if (animation.Priority is null)
            return 0;

        var elapsed = DateTime.UtcNow - animation.Started;

        //get the percentage of the animation that has elapsed
        var percentElapsed = elapsed.TotalMilliseconds / animation.DurationMs;

        // if we're less than halfway through the animation, priority is normal
        if (percentElapsed < 0.5)
            return animation.Priority.Value;

        //if we're more than halfway through the animation, the priority is interpolated
        //(it falls off to half it's original value during the last half of the animation)
        percentElapsed -= 0.5;

        return Convert.ToInt32(Math.Round(animation.Priority.Value * percentElapsed, MidpointRounding.ToPositiveInfinity));
    }

    /// <summary>
    ///     Returns a re-targeted animation based on a point. (Does not remove sourceAnimation)
    /// </summary>
    public Animation GetPointAnimation(IPoint targetPoint, uint? sourceId = null)
        => this with
        {
            TargetPoint = Point.From(targetPoint),
            TargetId = null,
            SourceId = sourceId ?? SourceId
        };

    /// <summary>
    ///     Returns a re-target animation based on a point. Removes sourceAnimation.
    /// </summary>
    public Animation GetPointEffectAnimation(IPoint targetPoint, uint? sourceId = null)
        => this with
        {
            TargetPoint = Point.From(targetPoint),
            TargetId = null,
            SourceId = sourceId ?? SourceId,
            SourceAnimation = 0
        };

    /// <summary>
    ///     Returns a re-targeted animation based on IDs. (Does not remove sourceAnimation)
    /// </summary>
    public Animation GetTargetedAnimation(uint targetId, uint? sourceId = null)
        => this with
        {
            TargetPoint = null,
            TargetId = targetId,
            SourceId = sourceId ?? SourceId
        };

    /// <summary>
    ///     Returns a re-targeted animation based on IDs. Removes sourceAnimation.
    /// </summary>
    public Animation GetTargetedEffectAnimation(uint targetId, uint? sourceId = null)
        => this with
        {
            TargetPoint = null,
            TargetId = targetId,
            SourceId = sourceId ?? SourceId,
            SourceAnimation = 0
        };

    public bool ShouldAnimateOver(Animation animation)
    {
        //if other animation has no priority
        //or is lower priority than this animation
        //or has expired
        //or it's interpolated priority is lower than the current animation's normal priority
        if (animation.Priority is null //fast path
            || (Priority >= animation.Priority) //fast path
            || ((DateTime.UtcNow - animation.Started).TotalMilliseconds >= animation.DurationMs) //check if expired
            || (Priority >= GetInterpolatedPriority(animation))) //check interpolated priority
            return true;

        return false;
    }
}