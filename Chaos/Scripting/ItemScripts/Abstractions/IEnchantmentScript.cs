using Chaos.MetaData.ItemMetaData;
using Chaos.Models.Templates;

namespace Chaos.Scripting.ItemScripts.Abstractions;

public interface IEnchantmentScript
{
    static abstract IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node, ItemTemplate template);
}