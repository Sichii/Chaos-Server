using Chaos.Common.Definitions;
using Chaos.Common.Utilities;
using Chaos.Extensions.Common;
using Chaos.Models.Abstractions;
using Chaos.Models.Data;
using Chaos.Models.Panel.Abstractions;
using Chaos.Models.Templates;
using Chaos.Models.World;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.ItemScripts.Abstractions;
using Chaos.TypeMapper.Abstractions;
using Chaos.Utilities;

namespace Chaos.Models.Panel;

/// <summary>
///     An object that exists within the inventory. This itemcan also be dropped to create a <see cref="GroundItem" />
/// </summary>
public sealed class Item : PanelEntityBase, IScripted<IItemScript>, IDialogSourceEntity
{
    private readonly NameComposer NameComposer;

    public DisplayColor Color
    {
        get => NameComposer.Color;
        set => NameComposer.SetColor(value);
    }

    public int Count { get; set; }
    public int? CurrentDurability { get; set; }
    public string? CustomDisplayName { get; set; }

    public string? CustomNameOverride
    {
        get => NameComposer.CustomName;
        set => NameComposer.SetCustomName(value);
    }

    public ItemSprite ItemSprite { get; set; }
    public int Level { get; set; }
    public Attributes Modifiers { get; set; }

    public string? Prefix
    {
        get => NameComposer.Prefix;
        set => NameComposer.SetPrefix(value);
    }

    public string? Suffix
    {
        get => NameComposer.Suffix;
        set => NameComposer.SetSuffix(value);
    }

    public int Weight { get; set; }
    public IItemScript Script { get; }

    public override ItemTemplate Template { get; }
    public string DisplayName => NameComposer.ComposedName;

    /// <inheritdoc />
    EntityType IDialogSourceEntity.EntityType => EntityType.Item;

    /// <inheritdoc />
    string IDialogSourceEntity.Name => DisplayName;

    ushort IDialogSourceEntity.Sprite => ItemSprite.PanelSprite;

    public Item(
        ItemTemplate template,
        IScriptProvider scriptProvider,
        ICollection<string>? extraScriptKeys = null,
        ulong? uniqueId = null,
        int? elapsedMs = null)
        : base(template, uniqueId, elapsedMs)
    {
        Template = template;
        NameComposer = new NameComposer(template.Name, template.IsDyeable);
        Color = template.Color;
        Count = 1;
        CurrentDurability = template.MaxDurability;
        Modifiers = template.Modifiers is null ? new Attributes() : ShallowCopy<Attributes>.Create(template.Modifiers);
        Weight = template.Weight;
        Level = template.Level;
        ItemSprite = template.ItemSprite;

        if (extraScriptKeys != null)
            ScriptKeys.AddRange(extraScriptKeys);

        Script = scriptProvider.CreateScript<IItemScript, Item>(ScriptKeys, this);
    }

    /// <inheritdoc />
    void IDialogSourceEntity.Activate(Aisling source) => Script.OnUse(source);

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

    public string ToAmountString(int amount) => $"{amount} {DisplayName}{(amount > 1 ? "s" : string.Empty)}";

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