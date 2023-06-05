namespace Chaos.MetaData.Abstractions;

/// <summary>
///     Provides the methods required to mutate a meta node into zero or more variants
/// </summary>
/// <typeparam name="T">The type of the meta node</typeparam>
public interface IMetaNodeMutator<T> where T: class
{
    /// <summary>
    ///     Mutates the specified meta node into zero or more variants
    /// </summary>
    /// <param name="obj">The node instance to mutate</param>
    /// <returns>Zero or more modified meta nodes</returns>
    IEnumerable<T> Mutate(T obj);
}