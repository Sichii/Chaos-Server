using Chaos.Core.Synchronization;
using Chaos.Effects.Abstractions;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;

namespace Chaos.Containers;

public class EffectsBar : IEffectsBar
{
    private readonly Creature Effected;
    private readonly Dictionary<string, IEffect> Effects;
    private readonly AutoReleasingMonitor Sync;
    private readonly Aisling? User;

    public EffectsBar(Creature effected, IEnumerable<IEffect>? effects = null)
    {
        Effected = effected;
        User = Effected as Aisling;
        Sync = new AutoReleasingMonitor();
        effects ??= Enumerable.Empty<IEffect>();

        Effects = new Dictionary<string, IEffect>(StringComparer.OrdinalIgnoreCase);

        foreach (var effect in effects)
            Effects.TryAdd(effect.CommonIdentifier, effect);
    }

    public void Add(IEffect effect)
    {
        using var @lock = Sync.Enter();
        Effects.TryAdd(effect.CommonIdentifier, effect);
    }

    public virtual void Apply(IEffect effect)
    {
        using var @lock = Sync.Enter();

        if (Effects.TryAdd(effect.CommonIdentifier, effect))
            effect.OnApplied();
        else if (Effects.TryGetValue(effect.CommonIdentifier, out var existingEffect))
            effect.OnFailedToApply($"{Effected.Name} is already effected by {existingEffect.Name}");
    }

    public void Dispel(string commonIdentifier)
    {
        using var @lock = Sync.Enter();

        if (Effects.TryRemove(commonIdentifier, out var effect))
            effect.OnDispelled();
    }

    public IEnumerator<IEffect> GetEnumerator()
    {
        List<IEffect> snapshot;

        using (Sync.Enter())
            snapshot = Effects.Values.ToList();

        return snapshot.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void Terminate(string commonIdentifier)
    {
        using var @lock = Sync.Enter();

        if (Effects.TryRemove(commonIdentifier, out var effect))
            effect.OnTerminated();
    }

    public void Update(TimeSpan delta)
    {
        using var @lock = Sync.Enter();

        foreach (var effect in Effects.Values.ToList())
        {
            effect.Update(delta);

            if (effect.Remaining.HasValue && (effect.Remaining <= TimeSpan.Zero))
            {
                Terminate(effect.CommonIdentifier);

                continue;
            }

            effect.OnUpdated();
        }
    }
}