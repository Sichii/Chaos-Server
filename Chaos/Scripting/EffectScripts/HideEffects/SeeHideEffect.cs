using Chaos.Scripting.EffectScripts.Abstractions;

namespace Chaos.Scripting.EffectScripts.HideEffects;

public sealed class SeeHideEffect : EffectBase
{
    /// <inheritdoc />
    public override byte Icon => 7;
    /// <inheritdoc />
    public override string Name => "See Hide";
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
}