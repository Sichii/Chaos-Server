using Chaos.MetaData.Abstractions;

namespace Chaos.MetaData.ItemMetadata;

/// <summary>
///     Represents a collection of <see cref="ItemMetaNode" /> that can be split into sub-sequences
/// </summary>
public sealed class ItemMetaNodeCollection : MetaNodeCollection<ItemMetaNode>, ISplittingMetaNodeCollection<ItemMetaNode>
{
    /// <inheritdoc />
    public IEnumerable<MetaDataBase<ItemMetaNode>> Split()
    {
        var index = 0;
        var metadata = new ItemMetaData(index++);
        var size = 0;

        for (var i = 0; i < Nodes.Count; i++)
        {
            var node = Nodes[i];

            if ((node.Length + size) > MAX_SIZE)
            {
                metadata.Compress();

                yield return metadata;

                metadata = new ItemMetaData(index++);
                size = 0;
            }

            metadata.AddNode(node);
            size += node.Length;
        }

        metadata.Compress();

        yield return metadata;
    }
}