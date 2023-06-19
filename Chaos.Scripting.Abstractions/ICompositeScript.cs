namespace Chaos.Scripting.Abstractions;

/// <summary>
///     Defines a pattern for a script that is composed of multiple scripts
/// </summary>
public interface ICompositeScript
{
    /// <summary>
    ///     Gets a script of the specified type
    /// </summary>
    /// <typeparam name="T">The type of the script to retreive</typeparam>
    T? GetScript<T>();

    /// <summary>
    ///     Gets all scripts of the specified type
    /// </summary>
    /// <typeparam name="T">The type of the script to retreive</typeparam>
    IEnumerable<T> GetScripts<T>();
}

/// <inheritdoc cref="ICompositeScript"/>
/// <typeparam name="TScript">A type that inherits from <see cref="Chaos.Scripting.Abstractions.IScript" /></typeparam>
public interface ICompositeScript<TScript> : ICompositeScript, IEnumerable<TScript> where TScript: IScript
{
    /// <summary>
    ///     Adds a script to this script
    /// </summary>
    /// <param name="script">The script to add</param>
    void Add(TScript script);

    /// <summary>
    ///     Removes a script from this script
    /// </summary>
    /// <param name="script">The script to remove</param>
    void Remove(TScript script);
}