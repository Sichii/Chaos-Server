using Chaos.Models.World.Abstractions;
using Chaos.Scripting.EffectScripts.Abstractions;

namespace Chaos.Scripting.EffectScripts.HideEffects;

public class SeeHideEffect : EffectBase
{
    /// <inheritdoc />
    public override byte Icon { get; } = 7;
    /// <inheritdoc />
    public override string Name { get; } = "See Hide";
    /// <inheritdoc />
    protected override TimeSpan Duration { get; } = TimeSpan.FromSeconds(30);

    /// <inheritdoc />
    public override void OnApplied()
    {
        AislingSubject?.SendOrangeBarMessage("You can now detect hidden things");
        AislingSubject?.Refresh(true);
    }

    /// <inheritdoc />
    public override void OnTerminated()
    {
        AislingSubject?.SendOrangeBarMessage("You can no longer detect hidden things");
        AislingSubject?.Refresh(true);
    }

    /// <inheritdoc />
    public override bool ShouldApply(Creature source, Creature target) => base.ShouldApply(source, target);
}