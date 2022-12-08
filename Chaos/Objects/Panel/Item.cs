using Chaos.Common.Definitions;
using Chaos.Extensions.Common;
using Chaos.Objects.Panel.Abstractions;
using Chaos.Objects.World;
using Chaos.Scripting.Abstractions;
using Chaos.Scripts.ItemScripts.Abstractions;
using Chaos.Templates;
using Chaos.TypeMapper.Abstractions;

namespace Chaos.Objects.Panel;

/// <summary>
///     An object that exists within the inventory. This itemcan also be dropped to create a <see cref="Chaos.Objects.World.GroundItem" />
/// </summary>
public sealed class Item : PanelObjectBase, IScripted<IItemScript>
{
    public DisplayColor Color { get; set; }
    public int Count { get; set; }
    public int? CurrentDurability { get; set; }
    public string DisplayName { get; set; }
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
        DisplayName = Template.Name;
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

    public string ToAmountString(int amount) => $@"{amount} {DisplayName}{(amount > 1 ? "s" : string.Empty)}";

    /// <inheritdoc />
    public override string ToString() => $"{{ UId: {UniqueId}, Name: \"{DisplayName}\", Count: {Count} }}";

    public string ToString(int amount) => $"{{ UId: {UniqueId}, Name: \"{DisplayName}\", Count: {amount} }}";

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        base.Update(delta);
        Script.Update(delta);
    }

    public void Use(Aisling source)
    {
        Script.OnUse(source);
        BeginCooldown(source);
    }
}