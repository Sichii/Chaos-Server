using Chaos.Core.Collections;
using Chaos.Data;
using Chaos.Entities.Schemas.Templates;
using Chaos.Services.Mappers.Abstractions;
using Chaos.Templates;

namespace Chaos.Services.Mappers;

public class ItemTemplateMapperProfile : IMapperProfile<ItemTemplate, ItemTemplateSchema>
{
    private readonly ITypeMapper Mapper;
    public ItemTemplateMapperProfile(ITypeMapper mapper) => Mapper = mapper;

    public ItemTemplate Map(ItemTemplateSchema obj) => new()
    {
        TemplateKey = obj.TemplateKey,
        Name = obj.Name,
        ScriptKeys = new HashSet<string>(obj.ScriptKeys, StringComparer.OrdinalIgnoreCase),
        AccountBound = obj.AccountBound,
        BaseClass = obj.BaseClass,
        AdvClass = obj.AdvClass,
        Color = obj.Color,
        EquipmentType = obj.EquipmentType,
        Gender = obj.Gender,
        ItemSprite = new ItemSprite(obj.PanelSprite, obj.DisplaySprite ?? 0),
        MaxDurability = obj.MaxDurability,
        MaxStacks = obj.MaxStacks,
        Modifiers = obj.Modifiers == null ? null : Mapper.Map<Attributes>(obj.Modifiers),
        Value = obj.Value,
        Weight = obj.Weight,
        Animation = obj.Animation == null ? null : Mapper.Map<Animation>(obj.Animation),
        Cooldown = obj.CooldownMs == null ? null : TimeSpan.FromMilliseconds(obj.CooldownMs.Value),
        BodyAnimationOverride = obj.BodyAnimationOverride,
        PanelSprite = obj.PanelSprite,
        ScriptVars = new Dictionary<string, DynamicVars>(
            obj.ScriptVars ?? Enumerable.Empty<KeyValuePair<string, DynamicVars>>(),
            StringComparer.OrdinalIgnoreCase)
    };

    public ItemTemplateSchema Map(ItemTemplate obj) => throw new NotImplementedException();
}