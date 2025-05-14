#region
using Chaos.Common.Abstractions;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.Templates;
using Chaos.Networking.Entities.Server;
using Chaos.Schemas.Aisling;
using Chaos.Schemas.Data;
using Chaos.Schemas.Templates;
using Chaos.Scripting.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.TypeMapper.Abstractions;
#endregion

namespace Chaos.Services.MapperProfiles;

public sealed class ItemMapperProfile(ISimpleCache simpleCache, IScriptProvider scriptProvider, ITypeMapper mapper)
    : IMapperProfile<Item, ItemSchema>,
      IMapperProfile<Item, ItemInfo>,
      IMapperProfile<ItemTemplate, ItemTemplateSchema>,
      IMapperProfile<ItemRequirement, ItemRequirementSchema>,
      IMapperProfile<ItemDetails, ItemInfo>
{
    private readonly ITypeMapper Mapper = mapper;
    private readonly IScriptProvider ScriptProvider = scriptProvider;
    private readonly ISimpleCache SimpleCache = simpleCache;

    public Item Map(ItemInfo obj) => throw new NotImplementedException();

    ItemInfo IMapperProfile<Item, ItemInfo>.Map(Item obj)
        => new()
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
            Sprite = obj.ItemSprite.PanelSprite,
            Stackable = obj.Template.Stackable
        };

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

        if (obj.Color.HasValue)
            item.Color = obj.Color.Value;

        if (obj.PanelSprite.HasValue)
            item.ItemSprite.PanelSprite = obj.PanelSprite.Value;

        if (obj.DisplaySprite.HasValue)
            item.ItemSprite.DisplaySprite = obj.DisplaySprite.Value;

        if (obj.AccountBound.HasValue)
            item.AccountBound = obj.AccountBound.Value;

        if (obj.NoTrade.HasValue)
            item.NoTrade = obj.NoTrade.Value;

        if (obj.PreventBanking.HasValue)
            item.PreventBanking = obj.PreventBanking.Value;

        if (obj.ArmorUsesOvercoatSprites.HasValue)
            item.ArmorUsesOvercoatSprites = obj.ArmorUsesOvercoatSprites.Value;

        if (obj.OvercoatUsesArmorSprites.HasValue)
            item.OvercoatUsesArmorSprites = obj.OvercoatUsesArmorSprites.Value;

        item.CustomNameOverride = obj.CustomNameOverride;
        item.Prefix = obj.Prefix;
        item.Suffix = obj.Suffix;
        item.NotepadText = obj.NotepadText;

        return item;
    }

    public ItemSchema Map(Item obj)
    {
        var extraScriptKeys = obj.ScriptKeys
                                 .Except(obj.Template.ScriptKeys)
                                 .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var ret = new ItemSchema
        {
            UniqueId = obj.UniqueId,
            ElapsedMs = obj.Elapsed.HasValue ? Convert.ToInt32(obj.Elapsed.Value.TotalMilliseconds) : null,
            ScriptKeys = extraScriptKeys.Count != 0 ? extraScriptKeys : null,
            TemplateKey = obj.Template.TemplateKey,
            Color = obj.Template.Color == obj.Color ? null : obj.Color,
            Count = obj.Count,
            CurrentDurability = obj.CurrentDurability,
            Slot = obj.Slot,
            Prefix = obj.Prefix,
            Suffix = obj.Suffix,
            CustomNameOverride = obj.CustomNameOverride,
            Weight = obj.Weight == obj.Template.Weight ? null : obj.Weight,
            PanelSprite = obj.ItemSprite.PanelSprite == obj.Template.ItemSprite.PanelSprite ? null : obj.ItemSprite.PanelSprite,
            DisplaySprite = obj.ItemSprite.DisplaySprite == obj.Template.ItemSprite.DisplaySprite ? null : obj.ItemSprite.DisplaySprite,
            NotepadText = obj.NotepadText
        };

        return ret;
    }

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
            Sprite = item.ItemSprite.PanelSprite,
            Stackable = item.Template.Stackable
        };
    }

    /// <inheritdoc />
    ItemDetails IMapperProfile<ItemDetails, ItemInfo>.Map(ItemInfo obj) => throw new NotImplementedException();

    /// <inheritdoc />
    public ItemRequirement Map(ItemRequirementSchema obj)
        => new()
        {
            ItemTemplateKey = obj.ItemTemplateKey,
            AmountRequired = obj.AmountRequired
        };

    /// <inheritdoc />
    public ItemRequirementSchema Map(ItemRequirement obj) => throw new NotImplementedException();

    public ItemTemplate Map(ItemTemplateSchema obj)
        => new()
        {
            TemplateKey = obj.TemplateKey,
            Name = obj.Name,
            ScriptKeys = new HashSet<string>(obj.ScriptKeys, StringComparer.OrdinalIgnoreCase),
            AccountBound = obj.AccountBound,
            NoTrade = obj.NoTrade,
            PreventBanking = obj.PreventBanking,
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
            AbilityLevel = obj.AbilityLevel,
            Class = obj.Class,
            RequiresMaster = obj.RequiresMaster,
            AdvClass = obj.AdvClass,
            Category = obj.Category,
            EquipmentType = obj.EquipmentType,
            Gender = obj.Gender,
            OverridesHeadSprite = obj.OverridesHeadSprite,
            OverridesBootsSprite = obj.OverridesBootsSprite,
            ArmorUsesOvercoatSprites = obj.ArmorUsesOvercoatSprites,
            OvercoatUsesArmorSprites = obj.OvercoatUsesArmorSprites
        };

    public ItemTemplateSchema Map(ItemTemplate obj) => throw new NotImplementedException();
}