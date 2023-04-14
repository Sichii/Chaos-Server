using Chaos.Common.Abstractions;
using Chaos.Data;
using Chaos.Networking.Entities.Server;
using Chaos.Objects.Panel;
using Chaos.Schemas.Aisling;
using Chaos.Schemas.Data;
using Chaos.Schemas.Templates;
using Chaos.Scripting.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Templates;
using Chaos.TypeMapper.Abstractions;

namespace Chaos.Services.MapperProfiles;

public sealed class ItemMapperProfile : IMapperProfile<Item, ItemSchema>,
                                        IMapperProfile<Item, ItemInfo>,
                                        IMapperProfile<ItemTemplate, ItemTemplateSchema>,
                                        IMapperProfile<ItemRequirement, ItemRequirementSchema>,
                                        IMapperProfile<ItemDetails, ItemInfo>
{
    private readonly ITypeMapper Mapper;
    private readonly IScriptProvider ScriptProvider;
    private readonly ISimpleCache SimpleCache;

    public ItemMapperProfile(
        ISimpleCache simpleCache,
        IScriptProvider scriptProvider,
        ITypeMapper mapper
    )
    {
        SimpleCache = simpleCache;
        ScriptProvider = scriptProvider;
        Mapper = mapper;
    }

    public Item Map(ItemSchema obj)
    {
        var template = SimpleCache.Get<ItemTemplate>(obj.TemplateKey);

        var item = new Item(
            template,
            ScriptProvider,
            obj.ScriptKeys,
            obj.UniqueId,
            obj.ElapsedMs)
        {
            Count = obj.Count,
            CurrentDurability = obj.CurrentDurability,
            Slot = obj.Slot ?? 0
        };

        if (template.IsModifiable && (obj.Modifiers != null))
            item.Modifiers = Mapper.Map<Attributes>(obj.Modifiers);

        if (obj.Color.HasValue)
            item.Color = obj.Color.Value;

        // ReSharper disable once MergeIntoPattern
        if (obj.PanelSprite.HasValue && obj.DisplaySprite.HasValue)
            item.ItemSprite = new ItemSprite(obj.PanelSprite.Value, obj.DisplaySprite.Value);

        if (!string.IsNullOrEmpty(obj.DisplayName))
            item.DisplayName = obj.DisplayName;

        return item;
    }

    public Item Map(ItemInfo obj) => throw new NotImplementedException();

    /// <inheritdoc />
    public ItemInfo Map(ItemDetails obj)
    {
        var item = obj.Item;

        return new ItemInfo
        {
            Color = item.Color,
            Cost = obj.Price,
            Count = item.Count < 0
                ? throw new InvalidOperationException($"Item \"{item.DisplayName}\" has negative count of {item.Count}")
                : Convert.ToUInt32(item.Count),
            CurrentDurability = item.CurrentDurability ?? 0,
            MaxDurability = item.Template.MaxDurability ?? 0,
            Name = item.DisplayName,
            Slot = item.Slot,
            Sprite = item.ItemSprite.OffsetPanelSprite,
            Stackable = item.Template.Stackable
        };
    }

    ItemInfo IMapperProfile<Item, ItemInfo>.Map(Item obj) => new()
    {
        Color = obj.Color,
        Cost = obj.Template.BuyCost,
        Count = obj.Count < 0
            ? throw new InvalidOperationException($"Item \"{obj.DisplayName}\" has negative count of {obj.Count}")
            : Convert.ToUInt32(obj.Count),
        CurrentDurability = obj.CurrentDurability ?? 0,
        MaxDurability = obj.Template.MaxDurability ?? 0,
        Name = obj.DisplayName,
        Slot = obj.Slot,
        Sprite = obj.ItemSprite.OffsetPanelSprite,
        Stackable = obj.Template.Stackable
    };

    public ItemSchema Map(Item obj)
    {
        var extraScriptKeys = obj.ScriptKeys.Except(obj.Template.ScriptKeys).ToHashSet(StringComparer.OrdinalIgnoreCase);

        var ret = new ItemSchema
        {
            UniqueId = obj.UniqueId,
            ElapsedMs = obj.Elapsed.HasValue ? Convert.ToInt32(obj.Elapsed.Value.TotalMilliseconds) : null,
            ScriptKeys = extraScriptKeys.Any() ? extraScriptKeys : null,
            TemplateKey = obj.Template.TemplateKey,
            Color = obj.Template.Color == obj.Color ? null : obj.Color,
            Count = obj.Count,
            CurrentDurability = obj.CurrentDurability,
            Slot = obj.Slot,
            DisplayName = obj.DisplayName == obj.Template.Name ? null : obj.DisplayName,
            Modifiers = obj.Template.IsModifiable && (obj.Modifiers != null) ? Mapper.Map<AttributesSchema>(obj.Modifiers) : null,
            Weight = obj.Weight == obj.Template.Weight ? null : obj.Weight,
            PanelSprite = obj.ItemSprite.PanelSprite == obj.Template.ItemSprite.PanelSprite ? null : obj.ItemSprite.PanelSprite,
            DisplaySprite = obj.ItemSprite.DisplaySprite == obj.Template.ItemSprite.DisplaySprite ? null : obj.ItemSprite.DisplaySprite
        };

        return ret;
    }

    public ItemTemplate Map(ItemTemplateSchema obj) => new()
    {
        TemplateKey = obj.TemplateKey,
        Name = obj.Name,
        ScriptKeys = new HashSet<string>(obj.ScriptKeys, StringComparer.OrdinalIgnoreCase),
        AccountBound = obj.AccountBound,
        Color = obj.Color,
        ItemSprite = new ItemSprite(obj.PanelSprite, obj.DisplaySprite ?? 0),
        MaxDurability = obj.MaxDurability,
        MaxStacks = obj.MaxStacks,
        Modifiers = obj.Modifiers == null ? null : Mapper.Map<Attributes>(obj.Modifiers),
        BuyCost = obj.BuyCost,
        SellValue = obj.SellValue,
        Weight = obj.Weight,
        Cooldown = obj.CooldownMs == null ? null : TimeSpan.FromMilliseconds(obj.CooldownMs.Value),
        PanelSprite = obj.PanelSprite,
        PantsColor = obj.PantsColor,
        ScriptVars = new Dictionary<string, IScriptVars>(
            obj.ScriptVars.Select(kvp => new KeyValuePair<string, IScriptVars>(kvp.Key, kvp.Value)),
            StringComparer.OrdinalIgnoreCase),
        Description = obj.Description,
        IsDyeable = obj.IsDyeable,
        IsModifiable = obj.IsModifiable,
        Level = obj.Level,
        Class = obj.Class,
        RequiresMaster = obj.RequiresMaster,
        AdvClass = obj.AdvClass,
        Category = obj.Category,
        EquipmentType = obj.EquipmentType,
        Gender = obj.Gender
    };

    public ItemTemplateSchema Map(ItemTemplate obj) => throw new NotImplementedException();

    /// <inheritdoc />
    public ItemRequirement Map(ItemRequirementSchema obj) => new()
    {
        ItemTemplateKey = obj.ItemTemplateKey,
        AmountRequired = obj.AmountRequired
    };

    /// <inheritdoc />
    public ItemRequirementSchema Map(ItemRequirement obj) => throw new NotImplementedException();

    /// <inheritdoc />
    ItemDetails IMapperProfile<ItemDetails, ItemInfo>.Map(ItemInfo obj) => throw new NotImplementedException();
}