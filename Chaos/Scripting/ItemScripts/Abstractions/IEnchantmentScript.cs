using Chaos.MetaData.ItemMetadata;

namespace Chaos.Scripting.ItemScripts.Abstractions;

public interface IEnchantmentScript
{
    static abstract IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node);
}