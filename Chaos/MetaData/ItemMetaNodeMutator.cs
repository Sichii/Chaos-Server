using Chaos.MetaData.Abstractions;
using Chaos.MetaData.ItemMetadata;
using Chaos.Models.Templates;

namespace Chaos.MetaData;

public abstract class ItemMetaNodeMutator : IItemMetaNodeMutator
{
    /// <inheritdoc />
    public abstract IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node, ItemTemplate template);

    public static IItemMetaNodeMutator Create(Func<ItemMetaNode, ItemTemplate, IEnumerable<ItemMetaNode>> mutator)
        => new LambdaItemMutator(mutator);

    private sealed class LambdaItemMutator : ItemMetaNodeMutator
    {
        private readonly Func<ItemMetaNode, ItemTemplate, IEnumerable<ItemMetaNode>> Mutator;

        internal LambdaItemMutator(Func<ItemMetaNode, ItemTemplate, IEnumerable<ItemMetaNode>> mutator) => Mutator = mutator;

        /// <inheritdoc />
        public override IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node, ItemTemplate template) => Mutator(node, template);
    }
}