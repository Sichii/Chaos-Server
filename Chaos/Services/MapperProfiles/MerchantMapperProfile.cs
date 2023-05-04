using Chaos.Collections.Common;
using Chaos.Common.Abstractions;
using Chaos.Models.Templates;
using Chaos.Schemas.Templates;
using Chaos.TypeMapper.Abstractions;

namespace Chaos.Services.MapperProfiles;

public class MerchantMapperProfile : IMapperProfile<MerchantTemplate, MerchantTemplateSchema>
{
    /// <inheritdoc />
    public MerchantTemplate Map(MerchantTemplateSchema obj) => new()
    {
        DialogKey = obj.DialogKey,
        Name = obj.Name,
        ScriptKeys = new HashSet<string>(obj.ScriptKeys, StringComparer.OrdinalIgnoreCase),
        ScriptVars = new Dictionary<string, IScriptVars>(
            obj.ScriptVars.Select(kvp => new KeyValuePair<string, IScriptVars>(kvp.Key, kvp.Value)),
            StringComparer.OrdinalIgnoreCase),
        Sprite = obj.Sprite,
        TemplateKey = obj.TemplateKey,
        ItemsForSale = new CounterCollection(
            obj.ItemsForSale.Select(details => new KeyValuePair<string, int>(details.ItemTemplateKey, details.Stock))),
        ItemsToBuy = obj.ItemsToBuy.ToList(),
        DefaultStock = obj.ItemsForSale.ToDictionary(details => details.ItemTemplateKey, details => details.Stock),
        SkillsToTeach = obj.SkillsToTeach.ToList(),
        SpellsToTeach = obj.SpellsToTeach.ToList(),
        RestockIntervalHours = obj.RestockIntervalHours,
        RestockPercent = obj.RestockPercent
    };

    /// <inheritdoc />
    public MerchantTemplateSchema Map(MerchantTemplate obj) => throw new NotImplementedException();
}