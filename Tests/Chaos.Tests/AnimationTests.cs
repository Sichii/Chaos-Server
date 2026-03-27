#region
using Chaos.Geometry;
using Chaos.Models.Data;
using FluentAssertions;
#endregion

namespace Chaos.Tests;

public sealed class AnimationTests
{
    private static Animation CreateAnimation(ushort sourceAnim = 100, ushort targetAnim = 200)
        => new()
        {
            SourceAnimation = sourceAnim,
            TargetAnimation = targetAnim,
            SourceId = 1u
        };

    #region GetPointAnimation
    [Test]
    public void GetPointAnimation_ShouldSetTargetPoint_AndClearTargetId()
    {
        var anim = CreateAnimation();
        var point = new Point(5, 10);

        var result = anim.GetPointAnimation(point);

        result.TargetPoint
              .Should()
              .Be(point);

        result.TargetId
              .Should()
              .BeNull();

        result.SourceAnimation
              .Should()
              .Be(100);

        result.SourceId
              .Should()
              .Be(1u);
    }

    [Test]
    public void GetPointAnimation_WithSourceId_ShouldOverrideSourceId()
    {
        var anim = CreateAnimation();
        var point = new Point(5, 10);

        var result = anim.GetPointAnimation(point, 42u);

        result.SourceId
              .Should()
              .Be(42u);
    }
    #endregion

    #region GetPointEffectAnimation
    [Test]
    public void GetPointEffectAnimation_ShouldZeroSourceAnimation()
    {
        var anim = CreateAnimation();
        var point = new Point(5, 10);

        var result = anim.GetPointEffectAnimation(point);

        result.TargetPoint
              .Should()
              .Be(point);

        result.TargetId
              .Should()
              .BeNull();

        result.SourceAnimation
              .Should()
              .Be(0);
    }

    [Test]
    public void GetPointEffectAnimation_WithSourceId_ShouldOverrideSourceId()
    {
        var anim = CreateAnimation();
        var point = new Point(5, 10);

        var result = anim.GetPointEffectAnimation(point, 42u);

        result.SourceId
              .Should()
              .Be(42u);

        result.SourceAnimation
              .Should()
              .Be(0);
    }
    #endregion

    #region GetTargetedAnimation
    [Test]
    public void GetTargetedAnimation_ShouldSetTargetId_AndClearTargetPoint()
    {
        var anim = CreateAnimation();

        var result = anim.GetTargetedAnimation(99u);

        result.TargetId
              .Should()
              .Be(99u);

        result.TargetPoint
              .Should()
              .BeNull();

        result.SourceAnimation
              .Should()
              .Be(100);
    }

    [Test]
    public void GetTargetedAnimation_WithSourceId_ShouldOverrideSourceId()
    {
        var anim = CreateAnimation();

        var result = anim.GetTargetedAnimation(99u, 42u);

        result.SourceId
              .Should()
              .Be(42u);

        result.TargetId
              .Should()
              .Be(99u);
    }
    #endregion

    #region GetTargetedEffectAnimation
    [Test]
    public void GetTargetedEffectAnimation_ShouldZeroSourceAnimation()
    {
        var anim = CreateAnimation();

        var result = anim.GetTargetedEffectAnimation(99u);

        result.TargetId
              .Should()
              .Be(99u);

        result.TargetPoint
              .Should()
              .BeNull();

        result.SourceAnimation
              .Should()
              .Be(0);
    }

    [Test]
    public void GetTargetedEffectAnimation_WithSourceId_ShouldOverrideSourceId()
    {
        var anim = CreateAnimation();

        var result = anim.GetTargetedEffectAnimation(99u, 42u);

        result.SourceId
              .Should()
              .Be(42u);

        result.SourceAnimation
              .Should()
              .Be(0);
    }
    #endregion

    #region ShouldAnimateOver
    [Test]
    public void ShouldAnimateOver_ShouldReturnTrue_WhenOtherHasNoPriority()
    {
        var current = new Animation
        {
            Priority = 5
        };

        var other = new Animation
        {
            Priority = null
        };

        current.ShouldAnimateOver(other)
               .Should()
               .BeTrue();
    }

    [Test]
    public void ShouldAnimateOver_ShouldReturnTrue_WhenCurrentPriorityIsHigher()
    {
        var current = new Animation
        {
            Priority = 10
        };

        var other = new Animation
        {
            Priority = 5
        };

        current.ShouldAnimateOver(other)
               .Should()
               .BeTrue();
    }

    [Test]
    public void ShouldAnimateOver_ShouldReturnFalse_WhenOtherPriorityIsHigher_AndNotExpired()
    {
        var current = new Animation
        {
            Priority = 1
        };

        var other = new Animation
        {
            Priority = 100,
            DurationMs = 10000,
            Started = DateTime.UtcNow
        };

        current.ShouldAnimateOver(other)
               .Should()
               .BeFalse();
    }

    [Test]
    public void ShouldAnimateOver_ShouldReturnTrue_WhenOtherIsExpired()
    {
        var current = new Animation
        {
            Priority = 1
        };

        var other = new Animation
        {
            Priority = 100,
            DurationMs = 100,
            Started = DateTime.UtcNow.AddMilliseconds(-200)
        };

        current.ShouldAnimateOver(other)
               .Should()
               .BeTrue();
    }

    [Test]
    public void ShouldAnimateOver_ShouldReturnTrue_WhenInterpolatedPriorityDropsBelowCurrent()
    {
        // Other animation has priority 100, started 900ms ago with 1000ms duration
        // percentElapsed ~= 0.90, adjustedPercent ~= 0.40
        // interpolated = Round(100 * 0.40, ToPositiveInfinity) = 40
        // Current priority 50 >= 40 => true
        // Even with 50ms timing variance (elapsed=950ms), adjustedPercent=0.45,
        // interpolated=45, and 50 >= 45 still holds
        var current = new Animation
        {
            Priority = 50
        };

        var other = new Animation
        {
            Priority = 100,
            DurationMs = 1000,
            Started = DateTime.UtcNow.AddMilliseconds(-900)
        };

        current.ShouldAnimateOver(other)
               .Should()
               .BeTrue();
    }

    [Test]
    public void ShouldAnimateOver_ShouldReturnFalse_WhenCurrentPriorityIsNull_AndOtherHasPriority()
    {
        var current = new Animation
        {
            Priority = null
        };

        var other = new Animation
        {
            Priority = 50,
            DurationMs = 10000,
            Started = DateTime.UtcNow
        };

        // null >= 50 is false, null >= interpolated is false, not expired
        // Only animation.Priority is null check matters — but that checks OTHER's priority
        // other.Priority is 50 (not null), so fast path fails
        // Priority (null) >= animation.Priority (50) => null >= 50 => false
        // Not expired
        // Priority (null) >= GetInterpolatedPriority(other) => null >= 50 => false
        // All conditions fail => returns false
        current.ShouldAnimateOver(other)
               .Should()
               .BeFalse();
    }
    #endregion
}