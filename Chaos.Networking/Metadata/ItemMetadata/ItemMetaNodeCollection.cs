using Chaos.Networking.Metadata.Abstractions;

namespace Chaos.Networking.Metadata.ItemMetadata;

public sealed class ItemMetaNodeCollection : MetaNodeCollection<ItemMetaNode>
{
    public IEnumerable<ItemMetaData> Split()
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