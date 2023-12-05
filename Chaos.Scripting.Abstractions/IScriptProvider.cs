namespace Chaos.Scripting.Abstractions;

/// <summary>
///     Defines the methods for an object that can be used to generate many kinds of scripts with different subjects
/// </summary>
public interface IScriptProvider
{
    /// <summary>
    ///     Creates a script with the scriptKeys for the given subject
    /// </summary>
    /// <param name="scriptKeys">A collection of <see cref="Chaos.Scripting.Abstractions.IScript.ScriptKey" />s</param>
    /// <param name="subject">The <see cref="Chaos.Scripting.Abstractions.IScripted" /> subject of the script(s)</param>
    /// <typeparam name="TScript">The type of the script to generate</typeparam>
    /// <typeparam name="TScripted">The type of the subject of the script</typeparam>
    /// <remarks>
    ///     It is expected that the generated scripts be combined into an
    ///     <see cref="Chaos.Scripting.Abstractions.ICompositeScript{TScript}" />
    /// </remarks>
    TScript CreateScript<TScript, TScripted>(ICollection<string> scriptKeys, TScripted subject)
        where TScript: IScript where TScripted: IScripted;
}