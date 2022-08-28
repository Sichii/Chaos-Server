using Chaos.Common.Definitions;
using Chaos.Entities.Schemas.Templates;
using Chaos.Services.Mappers.Interfaces;
using Chaos.Templates.Abstractions;

namespace Chaos.Templates;

public class SpellTemplate : PanelObjectTemplateBase
{
    public byte CastLines { get; init; }
    public string? Prompt { get; set; }
    public SpellType SpellType { get; init; }
    public override string TemplateKey { get; init; }

    public SpellTemplate(SpellTemplateSchema schema, ITypeMapper mapper)
        : base(schema, mapper)
    {
        CastLines = schema.CastLines;
        Prompt = schema.Prompt;
        SpellType = schema.SpellType;

        // ReSharper disable once VirtualMemberCallInConstructor
        TemplateKey = schema.TemplateKey;
    }
}