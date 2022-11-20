namespace Chaos.Scripting.Abstractions;

/// <summary>
///     Defines the contract for a script that can be attached at runtime
/// </summary>
public interface IScript
{
    /// <summary>
    ///     A key that can identify the script
    /// </summary>
    public string ScriptKey { get; }
}