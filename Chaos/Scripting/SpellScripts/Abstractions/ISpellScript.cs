#region
using Chaos.Models.Data;
using Chaos.Scripting.Abstractions;
using Chaos.Time.Abstractions;
#endregion

namespace Chaos.Scripting.SpellScripts.Abstractions;

[SuppressMessage("ReSharper", "UnusedParameter.Global")]
public interface ISpellScript : IScript, IDeltaUpdatable
{
    /// <summary>
    ///     Determines if the spell subject can be used in the given context of activation
    /// </summary>
    /// <param name="context">
    ///     An object containing details about the intended use of the spell
    /// </param>
    bool CanUse(SpellContext context);

    /// <summary>
    ///     Triggers when the spell subject is used in the given context of activation
    /// </summary>
    /// <param name="context">
    ///     An object containing details about the intended use of the spell
    /// </param>
    void OnUse(SpellContext context);
}