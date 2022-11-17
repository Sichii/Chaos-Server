using Chaos.Common.Definitions;
using Chaos.Common.Synchronization;
using Chaos.Containers.Abstractions;
using Chaos.Extensions.Common;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.EffectScripts.Abstractions;

namespace Chaos.Containers;

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

        if (effect.ShouldApply(source, Affected))
        {
            Effects[effect.Name] = effect;
            effect.OnApplied(Affected);
            ResetDisplay();
        }
    }

    /// <inheritdoc />
    public bool Contains(string effectName)
    {
        using var @lock = Sync.Enter();

        return Effects.ContainsKey(effectName);
    }

    public void Dispel(string effectName)
    {
        using var @lock = Sync.Enter();

        if (Effects.TryRemove(effectName, out var effect))
        {
            effect.OnDispelled();
            ResetDisplay();
        }
    }

    public IEnumerator<IEffect> GetEnumerator()
    {
        List<IEffect> snapshot;

        using (Sync.Enter())
            snapshot = Effects.Values.ToList();

        return snapshot.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private void ResetDisplay()
    {
        //clear all effects
        foreach (var effect in Effects.Values)
            AffectedAisling?.Client.SendEffect(EffectColor.None, effect.Icon);

        //re-apply all effects sorted by ascending remaining duration
        foreach (var effect in Effects.Values.OrderBy(e => e.Remaining))
            AffectedAisling?.Client.SendEffect(effect.Color, effect.Icon);
    }

    public void SimpleAdd(IEffect effect)
    {
        using var @lock = Sync.Enter();
        Effects[effect.Name] = effect;
        effect.OnReApplied(Affected);
    }

    public void Terminate(string effectName)
    {
        using var @lock = Sync.Enter();

        if (Effects.TryRemove(effectName, out var effect))
        {
            effect.OnTerminated();
            ResetDisplay();
        }
    }

    /// <inheritdoc />
    public bool TryGetEffect(string effectName, [MaybeNullWhen(false)] out IEffect effect)
    {
        using var @lock = Sync.Enter();

        return Effects.TryGetValue(effectName, out effect);
    }

    public void Update(TimeSpan delta)
    {
        if(Effects.Count == 0)
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
}