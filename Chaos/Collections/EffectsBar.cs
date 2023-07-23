using Chaos.Collections.Abstractions;
using Chaos.Common.Definitions;
using Chaos.Common.Synchronization;
using Chaos.Extensions;
using Chaos.Extensions.Common;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.EffectScripts.Abstractions;

namespace Chaos.Collections;

public sealed class EffectsBar : IEffectsBar
{
    private readonly Creature Affected;
    private readonly Aisling? AffectedAisling;
    private readonly Dictionary<string, IEffect> Effects;
    private readonly AutoReleasingMonitor Sync;

    public EffectsBar(Creature affected, IEnumerable<IEffect>? effects = null)
    {
        Affected = affected;
        AffectedAisling = Affected as Aisling;
        Sync = new AutoReleasingMonitor();
        effects ??= Enumerable.Empty<IEffect>();

        Effects = new Dictionary<string, IEffect>(StringComparer.OrdinalIgnoreCase);

        foreach (var effect in effects)
            Effects[effect.Name] = effect;
    }

    public void Apply(Creature source, IEffect effect)
    {
        using var @lock = Sync.Enter();

        effect.Subject = Affected;
        effect.Source = source;

        if (effect.ShouldApply(source, Affected))
        {
            //set color here because the bar will be fully reset anyway
            effect.Color = effect.GetColor();
            Effects[effect.Name] = effect;
            effect.OnApplied();
            ResetDisplay();
        }
    }

    /// <inheritdoc />
    public bool Contains(string effectName)
    {
        using var @lock = Sync.Enter();

        return Effects.ContainsKey(effectName) || Effects.Values.Any(effect => effect.ScriptKey.EqualsI(effectName));
    }

    public void Dispel(string effectName)
    {
        using var @lock = Sync.Enter();

        if (Effects.TryRemove(effectName, out var effect))
        {
            AffectedAisling?.Client.SendEffect(EffectColor.None, effect.Icon);
            effect.OnDispelled();
            ResetDisplay();
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public IEnumerator<IEffect> GetEnumerator()
    {
        List<IEffect> snapshot;

        using (Sync.Enter())
            snapshot = Effects.Values.ToList();

        return snapshot.GetEnumerator();
    }

    public void Terminate(string effectName)
    {
        using var @lock = Sync.Enter();

        if (Effects.TryRemove(effectName, out var effect))
        {
            AffectedAisling?.Client.SendEffect(EffectColor.None, effect.Icon);
            effect.OnTerminated();
            ResetDisplay();
        }
    }

    /// <inheritdoc />
    public bool TryGetEffect(string effectName, [MaybeNullWhen(false)] out IEffect effect)
    {
        using var @lock = Sync.Enter();

        if (Effects.TryGetValue(effectName, out effect))
            return true;

        effect = Effects.Values.FirstOrDefault(e => e.ScriptKey.EqualsI(effectName));

        return effect is not null;
    }

    public void Update(TimeSpan delta)
    {
        if (Effects.Count == 0)
            return;

        using var @lock = Sync.Enter();
        var shouldResetDisplay = false;

        foreach (var effect in Effects.Values.ToList())
        {
            effect.Update(delta);

            if (effect.Remaining <= TimeSpan.Zero)
            {
                Effects.Remove(effect.Name);
                effect.OnTerminated();
                shouldResetDisplay = true;
            }
        }

        if (shouldResetDisplay)
            ResetDisplay();
    }

    private void ResetDisplay()
    {
        //clear all effects
        foreach (var effect in Effects.Values)
            AffectedAisling?.Client.SendEffect(EffectColor.None, effect.Icon);

        var orderedEffects = Effects.Values.OrderBy(e => e.Remaining).ToList();

        //re-apply all effects sorted by ascending remaining duration
        foreach (var effect in orderedEffects)
            AffectedAisling?.Client.SendEffect(effect.Color, effect.Icon);
    }
}