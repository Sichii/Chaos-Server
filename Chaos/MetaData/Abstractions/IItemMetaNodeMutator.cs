using Chaos.MetaData.ItemMetaData;
using Chaos.Models.Templates;

namespace Chaos.MetaData.Abstractions;

public interface IItemMetaNodeMutator
{
    IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node, ItemTemplate template);
}