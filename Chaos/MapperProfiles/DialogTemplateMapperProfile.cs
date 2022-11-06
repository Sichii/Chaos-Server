using Chaos.Common.Collections;
using Chaos.Factories.Abstractions;
using Chaos.Objects.Menu;
using Chaos.Schemas.Content;
using Chaos.Templates;
using Chaos.TypeMapper.Abstractions;

namespace Chaos.MapperProfiles;

public class DialogTemplateMapperProfile : IMapperProfile<DialogTemplate, DialogTemplateSchema>
{
    private readonly IItemFactory ItemFactory;
    private readonly ITypeMapper Mapper;
    private readonly ISkillFactory SkillFactory;
    private readonly ISpellFactory SpellFactory;

    public DialogTemplateMapperProfile(
        ITypeMapper mapper,
        IItemFactory itemFactory,
        ISkillFactory skillFactory,
        ISpellFactory spellFactory
    )
    {
        Mapper = mapper;
        ItemFactory = itemFactory;
        SkillFactory = skillFactory;
        SpellFactory = spellFactory;
    }

    /// <inheritdoc />
    public DialogTemplate Map(DialogTemplateSchema obj) => new()
    {
        TemplateKey = obj.TemplateKey,
        NextDialogKey = obj.NextDialogKey,
        Options = Mapper.MapMany<DialogOption>(obj.Options)
                        .ToList(),
        PrevDialogKey = obj.PrevDialogKey,
        ScriptKeys = new HashSet<string>(obj.ScriptKeys, StringComparer.OrdinalIgnoreCase),
        ScriptVars = new Dictionary<string, DynamicVars>(obj.ScriptVars, StringComparer.OrdinalIgnoreCase),
        Text = obj.Text,
        TextBoxLength = obj.TextBoxLength,
        Type = obj.Type
    };

    /// <inheritdoc />
    public DialogTemplateSchema Map(DialogTemplate obj) => throw new NotImplementedException();
}