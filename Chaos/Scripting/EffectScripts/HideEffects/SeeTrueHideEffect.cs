using Chaos.Scripting.EffectScripts.Abstractions;

namespace Chaos.Scripting.EffectScripts.HideEffects;

public class SeeTrueHideEffect : EffectBase
{
    /// <inheritdoc />
    public override byte Icon { get; } = 7;
    /// <inheritdoc />
    public override string Name { get; } = "See True Hide";
    /// <inheritdoc />
    protected override TimeSpan Duration { get; } = TimeSpan.FromSeconds(15);

    /// <inheritdoc />
    public override void OnApplied()
    {
        AislingSubject?.SendOrangeBarMessage("You can now detect well hidden things");
        AislingSubject?.Refresh(true);
    }

    /// <inheritdoc />
    public override void OnTerminated()
    {
        AislingSubject?.SendOrangeBarMessage("You can no longer detect well hidden things");
        AislingSubject?.Refresh(true);
    }
}