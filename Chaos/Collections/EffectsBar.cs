#region
using Chaos.Collections.Abstractions;
using Chaos.DarkAges.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Common;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.EffectScripts.Abstractions;
#endregion

namespace Chaos.Collections;

/// <summary>
///     Represents a bar that displays effects on a creature in game
/// </summary>
public sealed class EffectsBar : IEffectsBar
{
    private readonly Creature Affected;
    private readonly Aisling? AffectedAisling;
    private readonly Dictionary<string, IEffect> Effects;
    private readonly Lock Sync;

    /// <summary>
    ///     Initializes a new instance of the <see cref="EffectsBar" /> class.
    /// </summary>
    /// <param name="affected">
    ///     The creature this bar is for
    /// </param>
    /// <param name="effects">
    ///     The effects to populate this collection with
    /// </param>
    public EffectsBar(Creature affected, IEnumerable<IEffect>? effects = null)
    {
        Affected = affected;
        AffectedAisling = Affected as Aisling;
        Sync = new Lock();
        effects ??= [];

        Effects = new Dictionary<string, IEffect>(StringComparer.OrdinalIgnoreCase);

        foreach (var effect in effects)
            Effects[effect.Name] = effect;
    }

    /// <summary>
    ///     Applies an effect to the creature this bar is for
    /// </summary>
    /// <param name="source">
    ///     The creature that applied the effect
    /// </param>
    /// <param name="effect">
    ///     The effect to apply
    /// </param>
    public void Apply(Creature source, IEffect effect)
    {
        using var @lock = Sync.EnterScope();

        effect.Subject = Affected;

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
        using var @lock = Sync.EnterScope();

        return Effects.ContainsKey(effectName) || Effects.Values.Any(effect => effect.ScriptKey.EqualsI(effectName));
    }

    /// <inheritdoc />
    public void Dispel(string effectName)
    {
        using var @lock = Sync.EnterScope();

        if (Effects.TryRemove(effectName, out var effect))
        {
            AffectedAisling?.Client.SendEffect(EffectColor.None, effect.Icon);
            effect.OnDispelled();
            ResetDisplay();
        }
    }

    /// <inheritdoc />
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc />
    public IEnumerator<IEffect> GetEnumerator()
    {
        List<IEffect> snapshot;

        using (Sync.EnterScope())
            snapshot = Effects.Values.ToList();

        return snapshot.GetEnumerator();
    }

    /// <inheritdoc />
    public void ResetDisplay()
    {
        //clear all effects
        foreach (var effect in Effects.Values)
            AffectedAisling?.Client.SendEffect(EffectColor.None, effect.Icon);

        var orderedEffects = Effects.Values
                                    .OrderBy(e => e.Remaining)
                                    .ToList();

        //re-apply all effects sorted by ascending remaining duration
        foreach (var effect in orderedEffects)
            AffectedAisling?.Client.SendEffect(effect.Color, effect.Icon);
    }

    /// <inheritdoc />
    public void Terminate(string effectName)
    {
        using var @lock = Sync.EnterScope();

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
        using var @lock = Sync.EnterScope();

        if (Effects.TryGetValue(effectName, out effect))
            return true;

        effect = Effects.Values.FirstOrDefault(e => e.ScriptKey.EqualsI(effectName));

        return effect is not null;
    }

    /// <inheritdoc />
    public void Update(TimeSpan delta)
    {
        if (Effects.Count == 0)
            return;

        using var @lock = Sync.EnterScope();
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