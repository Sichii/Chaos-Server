namespace Chaos.Scripting.Abstractions;

/// <summary>
///     Defines a pattern for generating scripts
/// </summary>
/// <typeparam name="TScript">The type of the script to generate</typeparam>
/// <typeparam name="TScripted">The type of the subject of the script</typeparam>
public interface IScriptFactory<out TScript, in TScripted> where TScript: IScript
                                                         where TScripted: IScripted
{
    /// <summary>
    ///     Creates a script with the scriptKeys for the given subject
    /// </summary>
    /// <param name="scriptKeys">A collection of <see cref="IScript.ScriptKey"/>s</param>
    /// <param name="subject">The <see cref="IScripted"/> subject of the script(s)</param>
    /// <remarks>It is expected that the generated scripts be combined into an <see cref="ICompositeScript{TScript}"/></remarks>
    TScript CreateScript(ICollection<string> scriptKeys, TScripted subject);
}