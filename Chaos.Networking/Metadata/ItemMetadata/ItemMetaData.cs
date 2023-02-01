using Chaos.Networking.Metadata.Abstractions;

namespace Chaos.Networking.Metadata.ItemMetadata;

public sealed class ItemMetaData : MetaDataBase<ItemMetaNode>
{
    private const string ITEM_METAFILE_NAME = "ItemInfo";

    public int Size => Nodes.Sum(node => node.Length);

    /// <inheritdoc />
    public ItemMetaData(int num)
        : base(ITEM_METAFILE_NAME + num) { }
}