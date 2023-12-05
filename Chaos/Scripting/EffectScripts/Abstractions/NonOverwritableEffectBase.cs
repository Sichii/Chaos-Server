using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;

namespace Chaos.Scripting.EffectScripts.Abstractions;

public abstract class NonOverwritableEffectBase : EffectBase
{
    protected abstract Animation? Animation { get; }
    protected abstract IReadOnlyCollection<string> ConflictingEffectNames { get; }
    protected abstract byte? Sound { get; }

    protected virtual string GetAlreadyAffectedMessage(Creature target, IEffect existingEffect)
        => $"{target.Name} is already affected by {existingEffect.Name}";

    /// <inheritdoc />
    public override void OnApplied()
    {
        if (Sound.HasValue)
            Subject.MapInstance.PlaySound(Sound.Value, Subject);

        if (Animation != null)
            Subject.Animate(Animation);
    }

    /// <inheritdoc />
    public override bool ShouldApply(Creature source, Creature target)
    {
        var existingEffect = ConflictingEffectNames
                             .Select(effectName => target.Effects.TryGetEffect(effectName, out var effect) ? effect : null)
                             .FirstOrDefault(effect => effect != null);

        if (existingEffect != null)
        {
            var message = GetAlreadyAffectedMessage(target, existingEffect);
            (source as Aisling)?.SendOrangeBarMessage(message);

            return false;
        }

        return true;
    }
}