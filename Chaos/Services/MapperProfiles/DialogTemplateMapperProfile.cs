using Chaos.Common.Abstractions;
using Chaos.Objects.Menu;
using Chaos.Schemas.Templates;
using Chaos.Templates;
using Chaos.TypeMapper.Abstractions;

namespace Chaos.Services.MapperProfiles;

public class DialogTemplateMapperProfile : IMapperProfile<DialogTemplate, DialogTemplateSchema>
{
    private readonly ITypeMapper Mapper;

    public DialogTemplateMapperProfile(ITypeMapper mapper) => Mapper = mapper;

    /// <inheritdoc />
    public DialogTemplate Map(DialogTemplateSchema obj) => new()
    {
        TemplateKey = obj.TemplateKey,
        NextDialogKey = obj.NextDialogKey,
        Options = Mapper.MapMany<DialogOption>(obj.Options)
                        .ToList(),
        PrevDialogKey = obj.PrevDialogKey,
        ScriptKeys = new HashSet<string>(obj.ScriptKeys, StringComparer.OrdinalIgnoreCase),
        ScriptVars = new Dictionary<string, IScriptVars>(
            obj.ScriptVars.Select(kvp => new KeyValuePair<string, IScriptVars>(kvp.Key, kvp.Value)),
            StringComparer.OrdinalIgnoreCase),
        Text = obj.Text,
        TextBoxLength = obj.TextBoxLength,
        Type = obj.Type,
        Contextual = obj.Contextual
    };

    /// <inheritdoc />
    public DialogTemplateSchema Map(DialogTemplate obj) => throw new NotImplementedException();
}