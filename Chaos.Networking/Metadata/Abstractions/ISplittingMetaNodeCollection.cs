namespace Chaos.Networking.Metadata.Abstractions;

public interface ISplittingMetaNodeCollection<TNode> where TNode: MetaNodeBase
{
    IEnumerable<MetaDataBase<TNode>> Split();
}