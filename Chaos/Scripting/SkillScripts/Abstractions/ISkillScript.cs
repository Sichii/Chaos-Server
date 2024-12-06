#region
using Chaos.Models.Data;
using Chaos.Scripting.Abstractions;
using Chaos.Time.Abstractions;
#endregion

// ReSharper disable UnusedParameter.Global

namespace Chaos.Scripting.SkillScripts.Abstractions;

public interface ISkillScript : IScript, IDeltaUpdatable
{
    /// <summary>
    ///     Determines if the skill subject can be used in the given context of activation
    /// </summary>
    /// <param name="context">
    ///     An object containing details about the intended use of the skill
    /// </param>
    bool CanUse(ActivationContext context);

    /// <summary>
    ///     Triggers when the skill subject is used in the given context of activation
    /// </summary>
    /// <param name="context">
    ///     An object containing details about the intended use of the skill
    /// </param>
    void OnUse(ActivationContext context);
}