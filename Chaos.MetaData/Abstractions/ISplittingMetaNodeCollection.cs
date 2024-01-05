namespace Chaos.MetaData.Abstractions;

/// <summary>
///     Represents a collection of nodes that can be split into multiple sequences
/// </summary>
/// <typeparam name="TNode">
///     The type of node contained in the collection
/// </typeparam>
public interface ISplittingMetaNodeCollection<TNode> where TNode: IMetaNode
{
    /// <summary>
    ///     Splits the collection into multiple sequences
    /// </summary>
    IEnumerable<MetaDataBase<TNode>> Split();
}