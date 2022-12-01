using Chaos.Common.Collections;
using Chaos.Schemas.Templates;
using Chaos.Templates;
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
        ScriptVars = new Dictionary<string, DynamicVars>(obj.ScriptVars, StringComparer.OrdinalIgnoreCase),
        Sprite = obj.Sprite,
        TemplateKey = obj.TemplateKey
    };

    /// <inheritdoc />
    public MerchantTemplateSchema Map(MerchantTemplate obj) => throw new NotImplementedException();
}