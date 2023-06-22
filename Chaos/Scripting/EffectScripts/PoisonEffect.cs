using Chaos.Common.Definitions;
using Chaos.Models.Data;
using Chaos.Scripting.EffectScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.EffectScripts;

public class PoisonEffect : ContinuousAnimationEffectBase
{
    /// <inheritdoc />
    protected override Animation Animation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 247
    };
    /// <inheritdoc />
    protected override IIntervalTimer AnimationInterval { get; } = new IntervalTimer(TimeSpan.FromMilliseconds(1500));
    /// <inheritdoc />
    protected override TimeSpan Duration { get; } = TimeSpan.FromMinutes(1);
    /// <inheritdoc />
    protected override IIntervalTimer Interval { get; } = new IntervalTimer(TimeSpan.FromMilliseconds(100));
    /// <inheritdoc />
    public override byte Icon => 35;
    /// <inheritdoc />
    public override string Name => "Poison";

    /// <inheritdoc />
    protected override void OnIntervalElapsed()
    {
        const int DAMAGE_PER_TICK = 5;

        if (Subject.StatSheet.CurrentHp <= DAMAGE_PER_TICK)
            return;

        if (Subject.StatSheet.TrySubtractHp(DAMAGE_PER_TICK))
            AislingSubject?.Client.SendAttributes(StatUpdateType.Vitality);
    }
}