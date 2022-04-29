using System;
using System.Collections.Generic;
using Chaos.Core.Definitions;
using Chaos.Core.Geometry;
using Chaos.Core.Utilities;
using Chaos.DataObjects;
using Chaos.PanelObjects.Abstractions;
using Chaos.Templates;
using Chaos.WorldObjects;

namespace Chaos.PanelObjects;

public class Item : PanelObjectBase
{
    public Attributes Attributes { get; set; }
    public DisplayColor Color { get; set; }
    public int Count { get; set; } = 1;
    public int? CurrentDurability { get; set; }
    public int Weight { get; set; }
    public ItemSprite ItemSprite { get; set; }
    public string DisplayName => Color == DisplayColor.None ? Template.Name : $"{Color} {Template.Name}";
    public override ItemTemplate Template { get; }

    public Item(ItemTemplate template)
        : base(template)
    {
        Template = template;
        CurrentDurability = template.MaxDurability;
        Color = template.DefaultColor;
        Weight = template.Weight;
        ItemSprite = template.ItemSprite;
        Attributes = new Attributes();
    }

    public override bool CanUse(double cooldownReduction) =>
        (Elapsed.TotalMilliseconds >= CONSTANTS.GLOBAL_ITEM_COOLDOWN_MS) && base.CanUse(cooldownReduction);

    public Item Split(int count)
    {
        lock (this)
        {
            if (!Template.Stackable)
                throw new InvalidOperationException($"{DisplayName} is not a stackable item.");

            if (Count <= count)
                throw new InvalidOperationException($"Current count {Count} is not greater than split amount {count}.");

            Count -= count;

            var clone = ShallowCopy<Item>.Clone(this, Template);
            clone.Count = count;

            return clone;
        }
    }

    public GroundItem ToGroundItem(Point point) => new(this, default!, point);

    public GroundItem ToGroundItem() => new(this);
    
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
            var clone = ShallowCopy<Item>.Clone(this, Template);
            clone.Count = Template.MaxStacks;

            yield return clone;
        }

        if (extra > 0)
        {
            var clone = ShallowCopy<Item>.Clone(this, Template);
            clone.Count = extra;

            yield return clone;
        }
    }
}