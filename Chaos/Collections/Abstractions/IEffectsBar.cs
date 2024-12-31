#region
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.EffectScripts.Abstractions;
using Chaos.Time.Abstractions;
#endregion

namespace Chaos.Collections.Abstractions;

/// <summary>
///     Provides methods for managing effects on a target entity
/// </summary>
public interface IEffectsBar : IEnumerable<IEffect>, IDeltaUpdatable
{
    /// <summary>
    ///     Applies the specified effect
    /// </summary>
    /// <param name="source">
    ///     The source creature applying the effect
    /// </param>
    /// <param name="effect">
    ///     The effect to be applied
    /// </param>
    void Apply(Creature source, IEffect effect);

    /// <summary>
    ///     Determines whether the specified effect is present in the collection
    /// </summary>
    /// <param name="effectName">
    ///     The name of the effect to search for
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     when the effect is found, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    bool Contains(string effectName);

    /// <summary>
    ///     Dispels the specified effect, removing it from the collection
    /// </summary>
    /// <param name="effectName">
    ///     The name of the effect to dispel
    /// </param>
    /// <remarks>
    ///     This is different from terminating an effect in that dispelling an effect is premature. This can be used to
    ///     differentiate effects that do something after a time, such as "Doom" from final fantasy. If "Doom" is dispelled, it
    ///     does not kill the target, but if the effect is terminated normally, it rolls the dice
    /// </remarks>
    void Dispel(string effectName);

    /// <summary>
    ///     Resets the display of all effects on the target entity by clearing and reapplying them
    /// </summary>
    void ResetDisplay();

    /// <summary>
    ///     Terminates the specified effect, removing it from the collection
    /// </summary>
    /// <param name="effectName">
    ///     The name of the effect to be terminated
    /// </param>
    /// <remarks>
    ///     This is different from dispelling an effect in that dispelling an effect is premature. This can be used to
    ///     differentiate effects that do something after a time, such as "Doom" from final fantasy. If "Doom" is dispelled, it
    ///     does not kill the target, but if the effect is terminated normally, it rolls the dice
    /// </remarks>
    void Terminate(string effectName);

    /// <summary>
    ///     Attempts to retrieve an effect from the collection
    /// </summary>
    /// <param name="effectName">
    ///     The name of the effect to retrieve
    /// </param>
    /// <param name="effect">
    ///     The effect, if found; otherwise,
    ///     <c>
    ///         null
    ///     </c>
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     when the effect is found, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    bool TryGetEffect(string effectName, [MaybeNullWhen(false)] out IEffect effect);
}