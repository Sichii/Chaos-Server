using Chaos.Entities.Schemas.Templates;
using Chaos.Services.Mappers.Interfaces;
using Chaos.Templates.Abstractions;

namespace Chaos.Templates;

public class SkillTemplate : PanelObjectTemplateBase
{
    public bool IsAssail { get; init; }
    public override string TemplateKey { get; init; }

    public SkillTemplate(SkillTemplateSchema schema, ITypeMapper mapper)
        : base(schema, mapper)
    {
        IsAssail = schema.IsAssail;
        // ReSharper disable once VirtualMemberCallInConstructor
        TemplateKey = schema.TemplateKey;
    }
}