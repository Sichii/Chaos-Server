using Chaos.Extensions.Common;
using Chaos.MetaData.ItemMetadata;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.Templates;
using Chaos.Scripting.ItemScripts.Abstractions;

namespace Chaos.Scripting.ItemScripts.Enchantments;

public class MagicPrefixScript : ItemScriptBase, IEnchantmentScript
{
    /// <inheritdoc />
    public MagicPrefixScript(Item subject)
        : base(subject)
    {
        Subject.Prefix = "Magic";

        var attributes = new Attributes
        {
            MaximumMp = 50
        };

        subject.Modifiers.Add(attributes);
    }

    /// <inheritdoc />
    public static IEnumerable<ItemMetaNode> Mutate(ItemMetaNode node, ItemTemplate template)
    {
        if (!node.Name.StartsWithI("Magic"))
            yield return node with { Name = $"Magic {node.Name}" };
    }
}