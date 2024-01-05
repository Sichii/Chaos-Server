namespace Chaos.Scripting.Abstractions;

/// <summary>
///     Defines the contract for an object that can have scripts attached to it
/// </summary>
public interface IScripted
{
    /// <summary>
    ///     A unique collection of unique identifiers for the scripts attached to this object
    /// </summary>
    ISet<string> ScriptKeys { get; }
}

/// <inheritdoc />
/// <typeparam name="TScript">
///     The type of script that will be loaded for this object
/// </typeparam>
public interface IScripted<out TScript> : IScripted where TScript: IScript
{
    /// <summary>
    ///     The attached script
    /// </summary>
    TScript Script { get; }
}