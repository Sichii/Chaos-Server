using Chaos.Networking.Model.Server;
using Chaos.Objects.Panel.Abstractions;
using Chaos.Objects.Serializable;
using Chaos.Scripts.Interfaces;
using Chaos.Services.Caches.Interfaces;
using Chaos.Services.Factories.Interfaces;
using Chaos.Services.Utility.Interfaces;
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
    
    public IEnumerable<Item> FixStacks(ICloningService<Item> itemCloner)
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
            var clone = itemCloner.Clone(this);
            clone.Count = Template.MaxStacks;

            yield return clone;
        }

        if (extra > 0)
        {
            var clone = itemCloner.Clone(this);
            clone.Count = extra;

            yield return clone;
        }
    }

    public Item Split(int count, ICloningService<Item> itemCloner)
    {
        lock (this)
        {
            if (!Template.Stackable)
                throw new InvalidOperationException($"{DisplayName} is not a stackable item.");

            if (Count <= count)
                throw new InvalidOperationException($"Current count {Count} is not greater than split amount {count}.");

            Count -= count;

            var clone = itemCloner.Clone(this);
            clone.Count = count;

            return clone;
        }
    }

    public ItemInfo ToItemInfo() => new()
    {
        Class = Template.BaseClass ?? BaseClass.Peasant,
        Color = Color,
        Cost = Template.Value,
        Count = Count < 0
            ? throw new InvalidOperationException($"Item \"{DisplayName}\" has negative count of {Count}")
            : Convert.ToUInt32(Count),
        CurrentDurability = CurrentDurability ?? 0,
        GameObjectType = GameObjectType.Item,
        MaxDurability = Template.MaxDurability ?? 0,
        Name = DisplayName,
        Slot = Slot,
        Sprite = Template.ItemSprite.PanelSprite,
        Stackable = Template.Stackable
    };

    public override string ToString() => $@"(Id: {UniqueId}, Name: {DisplayName}, Count: {Count})";
}