using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Chaos.Core.Interfaces;
using Chaos.Effects.Interfaces;

namespace Chaos.Containers;

public class EffectsBar : IEnumerable<IEffect>, IDeltaUpdatable
{
    private readonly ConcurrentDictionary<string, IEffect> Effects;

    public EffectsBar(IEnumerable<IEffect>? effects = null)
    {
        effects ??= Enumerable.Empty<IEffect>();

        Effects = new ConcurrentDictionary<string, IEffect>(effects.ToDictionary(effect => effect.Name));
    }

    public void Clear() => Effects.Clear();

    public IEnumerator<IEffect> GetEnumerator()
    {
        using var enumerator = Effects.GetEnumerator();

        while (enumerator.MoveNext())
            yield return enumerator.Current.Value;
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public bool Remove(string name, [MaybeNullWhen(false)] out IEffect effect) => Effects.Remove(name, out effect);

    public bool TryAdd(IEffect effect) => Effects.TryAdd(effect.Name, effect);

    public void Update(TimeSpan delta)
    {
        foreach (var effect in this)
            effect.Update(delta);
    }
}