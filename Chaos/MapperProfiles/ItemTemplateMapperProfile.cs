using Chaos.Common.Collections;
using Chaos.Data;
using Chaos.Schemas.Templates;
using Chaos.Templates;
using Chaos.TypeMapper.Abstractions;

namespace Chaos.MapperProfiles;

public sealed class ItemTemplateMapperProfile : IMapperProfile<ItemTemplate, ItemTemplateSchema>
{
    private readonly ITypeMapper Mapper;
    public ItemTemplateMapperProfile(ITypeMapper mapper) => Mapper = mapper;

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
        Value = obj.Value,
        Weight = obj.Weight,
        Cooldown = obj.CooldownMs == null ? null : TimeSpan.FromMilliseconds(obj.CooldownMs.Value),
        PanelSprite = obj.PanelSprite,
        PantsColor = obj.PantsColor,
        ScriptVars = new Dictionary<string, DynamicVars>(
            obj.ScriptVars ?? Enumerable.Empty<KeyValuePair<string, DynamicVars>>(),
            StringComparer.OrdinalIgnoreCase)
    };

    public ItemTemplateSchema Map(ItemTemplate obj) => throw new NotImplementedException();
}