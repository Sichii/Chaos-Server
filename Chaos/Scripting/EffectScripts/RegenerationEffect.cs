#region
using Chaos.DarkAges.Definitions;
using Chaos.Models.Data;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.EffectScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;
#endregion

namespace Chaos.Scripting.EffectScripts;

public sealed class RegenerationEffect : ContinuousAnimationEffectBase
{
    private int HealAmount;

    /// <inheritdoc />
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(10);

    /// <inheritdoc />
    protected override Animation Animation { get; } = new()
    {
        AnimationSpeed = 100,
        TargetAnimation = 187
    };

    /// <inheritdoc />
    protected override IIntervalTimer AnimationInterval { get; } = new IntervalTimer(TimeSpan.FromSeconds(1));

    /// <inheritdoc />
    protected override IIntervalTimer Interval { get; } = new IntervalTimer(TimeSpan.FromMilliseconds(100));

    /// <inheritdoc />
    public override byte Icon => 146;

    /// <inheritdoc />
    public override string Name => "Regeneration";

    /// <inheritdoc />
    public override void OnApplied() => HealAmount = GetVar<int>("healAmount");

    /// <inheritdoc />
    protected override void OnIntervalElapsed()
    {
        Subject.StatSheet.AddHp(HealAmount);

        //if the subject was a player, update their vit
        AislingSubject?.Client.SendAttributes(StatUpdateType.Vitality);
    }

    /// <inheritdoc />
    public override void PrepareSnapshot(Creature source) => SetVar("healAmount", source.StatSheet.EffectiveWis * 5 / 10);
}