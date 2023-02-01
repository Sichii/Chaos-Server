using Chaos.Networking.Metadata.ItemMetadata;
using Chaos.Services.Storage.Abstractions;

namespace Chaos.Services.Storage.Metadata.Mutators;

public sealed class EnchantmentMetaNodeMutator : IMetaNodeMutator<ItemMetaNode>
{
    /// <inheritdoc />
    public IEnumerable<ItemMetaNode> Mutate(ItemMetaNode obj)
    {
        yield return obj with
        {
            Name = $"Magic {obj.Name}"
        };

        yield return obj with
        {
            Name = $"Abundance {obj.Name}"
        };

        yield return obj with
        {
            Name = $"Blessed {obj.Name}"
        };

        yield return obj with
        {
            Name = $"Might {obj.Name}"
        };
    }
}