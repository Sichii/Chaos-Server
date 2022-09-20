using Chaos.Common.Definitions;
using Chaos.Objects.Panel.Abstractions;
using Chaos.Objects.World;
using Chaos.Scripts.ItemScripts.Abstractions;
using Chaos.Services.Scripting.Abstractions;
using Chaos.Services.Utility.Abstractions;
using Chaos.Templates;

namespace Chaos.Objects.Panel;

public sealed class Item : PanelObjectBase, IScriptedItem
{
    public DisplayColor Color { get; set; }
    public int Count { get; set; }
    public int? CurrentDurability { get; set; }
    public string DisplayName => Color == DisplayColor.None ? Template.Name : $"{Color} {Template.Name}";
    public IItemScript Script { get; }
    public override ItemTemplate Template { get; }

    public Item(
        ItemTemplate template,
        IScriptProvider scriptProvider,
        ICollection<string>? extraScriptKeys = null,
        ulong? uniqueId = null,
        int? elapsedMs = null
    )
        : base(template, uniqueId, elapsedMs)
    {
        Template = template;
        Color = template.Color;
        Count = 1;
        CurrentDurability = template.MaxDurability;
        //default slot is 0

        if (extraScriptKeys != null)
            ScriptKeys.AddRange(extraScriptKeys);

        Script = scriptProvider.CreateScript<IItemScript, Item>(ScriptKeys, this);
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
            if (Count <= count)
                throw new InvalidOperationException($"Current count {Count} is not greater than split amount {count}.");

            if (!Template.Stackable)
                throw new InvalidOperationException($"{DisplayName} is not a stackable item.");

            Count -= count;

            var clone = itemCloner.Clone(this);
            clone.Count = count;

            return clone;
        }
    }

    public override string ToString() => $@"(Id: {UniqueId}, Name: {DisplayName}, Count: {Count})";

    public void Use(Aisling source) => Script.OnUse(source);
}