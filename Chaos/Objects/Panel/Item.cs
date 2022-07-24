using Chaos.Caches.Interfaces;
using Chaos.Factories.Interfaces;
using Chaos.Objects.Panel.Abstractions;
using Chaos.Objects.Serializable;
using Chaos.Scripts.Interfaces;
using Chaos.Templates;

namespace Chaos.Objects.Panel;

public class Item : PanelObjectBase, IScriptedItem
{
    public DisplayColor Color { get; set; }
    public int Count { get; set; }
    public int? CurrentDurability { get; set; }
    public IItemScript Script { get; }
    public string DisplayName => Color == DisplayColor.None ? Template.Name : $"{Color} {Template.Name}";
    public override ItemTemplate Template { get; }

    public Item(ItemTemplate template,
        IItemScriptFactory itemScriptFactory,
        ICollection<string>? extraScriptKeys = null,
        ulong? uniqueId = null)
        : base(template, uniqueId)
    {
        Template = template;
        Color = template.Color;
        Count = 1;
        CurrentDurability = template.MaxDurability;
        //default slot is 0

        if(extraScriptKeys != null)
            ScriptKeys.AddRange(extraScriptKeys);

        Script = itemScriptFactory.CreateScript(ScriptKeys, this);
    }

    public Item(
        SerializableItem serializableItem,
        ISimpleCache<ItemTemplate> itemTemplateCache,
        IItemScriptFactory itemScriptFactory
    ) : this(itemTemplateCache.GetObject(serializableItem.TemplateKey),
        itemScriptFactory,
        serializableItem.ScriptKeys,
        serializableItem.UniqueId)
    {
        Color = serializableItem.Color;
        Count = serializableItem.Count;
        CurrentDurability = serializableItem.CurrentDurability;
        Slot = serializableItem.Slot ?? 0;
    }
    
    public IEnumerable<Item> FixStacks(IItemFactory itemFactory)
    {
        if (Count <= Template.MaxStacks)
        {
            yield return this;

            yield break;
        }

        var stackCount = Count / Template.MaxStacks;
        var extra = Count % Template.MaxStacks;

        for (var i = 0; i < stackCount; i++)
        {
            var clone = itemFactory.CloneItem(this);
            clone.Count = Template.MaxStacks;

            yield return clone;
        }

        if (extra > 0)
        {
            var clone = itemFactory.CloneItem(this);
            clone.Count = extra;

            yield return clone;
        }
    }

    public Item Split(int count, IItemFactory itemFactory)
    {
        lock (this)
        {
            if (!Template.Stackable)
                throw new InvalidOperationException($"{DisplayName} is not a stackable item.");

            if (Count <= count)
                throw new InvalidOperationException($"Current count {Count} is not greater than split amount {count}.");

            Count -= count;

            var clone = itemFactory.CloneItem(this);
            clone.Count = count;

            return clone;
        }
    }

    public override string ToString() => $@"(Id: {UniqueId}, Name: {DisplayName}, Count: {Count})";
}