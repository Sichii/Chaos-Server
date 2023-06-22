using Chaos.Extensions.Common;
using Chaos.MetaData.ItemMetadata;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public class MagicPrefixScript : ItemScriptBase, IEnchantmentScript
{
    /// <inheritdoc />
    public MagicPrefixScript(Item subject)
        : base(subject)
    {
        if (!subject.DisplayName.StartsWithI("Magic"))
            subject.DisplayName = $"Magic {subject.DisplayName}";

        var attributes = new Attributes
        {
            MaximumMp = 50
        };

        subject.Modifiers.Add(attributes);
    }

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node)
    {
        if (!node.Name.StartsWithI("Magic"))
            yield return node with { Name = $"Magic {node.Name}" };
    }
}