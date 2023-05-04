using Chaos.Common.Definitions;
using Chaos.Models.Data;
using Chaos.Scripting.EffectScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.EffectScripts;

public sealed class RegenerationEffect : ContinuousAnimationEffectBase
{
    /// <inheritdoc />
    public override byte Icon => 146;
    /// <inheritdoc />
    public override string Name => "Regeneration";

    /// <inheritdoc />
    protected override Animation Animation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 187
    };
    /// <inheritdoc />
    protected override IIntervalTimer AnimationInterval { get; } = new IntervalTimer(TimeSpan.FromSeconds(1));
    /// <inheritdoc />
    protected override TimeSpan Duration { get; } = TimeSpan.FromSeconds(10);
    /// <inheritdoc />
    protected override IIntervalTimer Interval { get; } = new IntervalTimer(TimeSpan.FromMilliseconds(100));

    /// <inheritdoc />
    protected override void OnIntervalElapsed()
    {
        //the interval is 100ms, so this will be applied 10 times a second
        const int HEAL_PER_TICK = 5;

        Subject.StatSheet.AddHp(HEAL_PER_TICK);
        //if the subject was a player, update their vit
        AislingSubject?.Client.SendAttributes(StatUpdateType.Vitality);
    }
}