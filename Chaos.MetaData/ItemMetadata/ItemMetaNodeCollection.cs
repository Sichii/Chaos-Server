using Chaos.MetaData.Abstractions;

namespace Chaos.MetaData.ItemMetaData;

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

        for (var i = 0; i < Nodes.Count; i++)
        {
            var node = Nodes[i];

            if ((node.Length + metadata.Size) >= MAX_SIZE)
            {
                metadata.Compress();

                yield return metadata;

                metadata = new ItemMetaData(index++);
            }

            metadata.AddNode(node);
        }

        metadata.Compress();

        yield return metadata;
    }
}