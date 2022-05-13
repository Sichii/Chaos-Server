using System;
using System.Collections.Generic;
using Chaos.Core.Definitions;
using Chaos.Core.Geometry;
using Chaos.Objects.Panel.Abstractions;
using Chaos.Objects.World;
using Chaos.Scripts.Interfaces;
using Chaos.Templates;
using Chaos.Utilities;

namespace Chaos.Objects.Panel;

public class Item : PanelObjectBase, IScriptedItem
{
    public DisplayColor Color { get; set; }
    public int Count { get; set; }
    public int? CurrentDurability { get; set; }
    public IItemScript Script { get; set; } = null!;
    public string DisplayName => Color == DisplayColor.None ? Template.Name : $"{Color} {Template.Name}";
    public override ItemTemplate Template { get; }

    public Item(ItemTemplate template)
        : base(template)
    {
        Template = template;
        CurrentDurability = template.MaxDurability;
        Color = template.DefaultColor;
        Count = 1;
    }

    public IEnumerable<Item> FixStacks()
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
            var clone = ItemUtility.Instance.Clone(this);
            clone.Count = Template.MaxStacks;

            yield return clone;
        }

        if (extra > 0)
        {
            var clone = ItemUtility.Instance.Clone(this);
            clone.Count = extra;

            yield return clone;
        }
    }

    public Item Split(int count)
    {
        lock (this)
        {
            if (!Template.Stackable)
                throw new InvalidOperationException($"{DisplayName} is not a stackable item.");

            if (Count <= count)
                throw new InvalidOperationException($"Current count {Count} is not greater than split amount {count}.");

            Count -= count;

            var clone = ItemUtility.Instance.Clone(this);
            clone.Count = count;

            return clone;
        }
    }

    public GroundItem ToGroundItem(Point point) => new(this, default!, point);

    public GroundItem ToGroundItem() => new(this);

    public override string ToString() => $@"(Id: {UniqueId}, Name: {DisplayName}, Count: {Count})";
}