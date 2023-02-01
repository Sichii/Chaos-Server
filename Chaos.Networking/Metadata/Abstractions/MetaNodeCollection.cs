namespace Chaos.Networking.Metadata.Abstractions;

public abstract class MetaNodeCollection<TNode> where TNode: MetaNodeBase
{
    protected const ushort MAX_SIZE = (ushort)(ushort.MaxValue * 0.95);
    public List<TNode> Nodes { get; }

    protected MetaNodeCollection() => Nodes = new List<TNode>();

    public void AddNode(TNode node) => Nodes.Add(node);
}