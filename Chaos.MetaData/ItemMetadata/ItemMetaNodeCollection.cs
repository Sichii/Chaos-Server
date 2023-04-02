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
        var metafile = new ItemMetaData(index++);
        var size = 0;

        for (var i = 0; i < Nodes.Count; i++)
        {
            if (size > MAX_SIZE)
            {
                metafile.Compress();

                yield return metafile;

                metafile = new ItemMetaData(index++);
                size = 0;
            }

            var node = Nodes[i];
            metafile.AddNode(node);
            size += node.Length;
        }

        metafile.Compress();

        yield return metafile;
    }
}