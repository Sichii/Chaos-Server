using Chaos.MetaData.Abstractions;

namespace Chaos.MetaData.ItemMetadata;

/// <summary>
///     Represents a compressible collection of <see cref="ItemMetaNode" />s
/// </summary>
public sealed class ItemMetaData : MetaDataBase<ItemMetaNode>
{
    private const string ITEM_METAFILE_NAME = "ItemInfo";

    /// <summary>
    ///     The size of the <see cref="ItemMetaData" /> in bytes
    /// </summary>
    public int Size => Nodes.Sum(node => node.Length);

    /// <inheritdoc />
    public ItemMetaData(int num)
        : base(ITEM_METAFILE_NAME + num) { }
}