using Chaos.Common.Definitions;
using Chaos.Data;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.EffectScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripts.EffectScripts;

public class PoisonEffect : AnimatingEffectBase
{
    /// <inheritdoc />
    public override byte Icon { get; } = 35;
    /// <inheritdoc />
    public override string Name { get; } = "Poison";
    /// <inheritdoc />
    protected override TimeSpan Duration { get; } = TimeSpan.FromMinutes(1);
    /// <inheritdoc />
    protected override IIntervalTimer Interval { get; } = new IntervalTimer(TimeSpan.FromMilliseconds(100));

    /// <inheritdoc />
    protected override void OnIntervalElapsed()
    {
        const int DAMAGE_PER_TICK = 5;

        if (Subject.StatSheet.CurrentHp <= DAMAGE_PER_TICK)
            return;
        
        Subject.StatSheet.SubtractHp(DAMAGE_PER_TICK);
        AislingSubject?.Client.SendAttributes(StatUpdateType.Vitality);
    }

    /// <inheritdoc />
    protected override Animation Animation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 247
    };
    /// <inheritdoc />
    protected override IIntervalTimer AnimationInterval { get; } = new IntervalTimer(TimeSpan.FromMilliseconds(1500));
}