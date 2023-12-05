using Chaos.Scripting.EffectScripts.Abstractions;

namespace Chaos.Scripting.EffectScripts.HideEffects;

public sealed class SeeTrueHideEffect : EffectBase
{
    /// <inheritdoc />
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(15);

    /// <inheritdoc />
    public override byte Icon => 7;

    /// <inheritdoc />
    public override string Name => "See True Hide";

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